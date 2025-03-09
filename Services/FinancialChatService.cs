using BudgetBackend.Data;
using BudgetBackend.Models;
using BudgetBackend.Plugins;
using BudgetBackend.Agents; // Add explicit import for the Agents namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BudgetBackend.Services
{
    // Add interface for agents
    public interface IAgent
    {
        string Name { get; }
        string Description { get; }
        Task<string> GenerateResponseAsync(int userId, string message);
    }

    public class FinancialChatService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly ConcurrentDictionary<string, ChatHistory> _chatHistories = new();
        private readonly BudgetPlugin _budgetPlugin;
        private readonly Dictionary<string, IAgent> _agents = new();
        private readonly BudgetBackend.Agents.AgentFactory _agentFactory; // Fully qualified name

        public FinancialChatService(ApplicationDbContext dbContext, 
                                   Kernel kernel,
                                   IChatCompletionService chatService,
                                   BudgetPlugin budgetPlugin,
                                   BudgetBackend.Agents.AgentFactory agentFactory) // Fully qualified name
        {
            _dbContext = dbContext;
            _kernel = kernel;
            _chatService = chatService;
            _budgetPlugin = budgetPlugin;
            _agentFactory = agentFactory ?? throw new ArgumentNullException(nameof(agentFactory));
            
            // Initialize available agents
            InitializeAgents();
        }

        private void InitializeAgents()
        {
            try 
            {
                var financialAdvisor = _agentFactory.CreateFinancialAdvisor();
                var budgetAnalyst = _agentFactory.CreateBudgetAnalyst();
                
                _agents[financialAdvisor.Name] = (IAgent)financialAdvisor;
                _agents[budgetAnalyst.Name] = (IAgent)budgetAnalyst;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error initializing agents: {ex.Message}");
                throw;
            }
        }

        public async Task<ChatMessage> SendMessageAsync(string sessionId, int userId, string message)
        {
            // Get or create chat history for this session
            var chatHistory = _chatHistories.GetOrAdd(sessionId, _ => new ChatHistory());
            
            // Add the user message to history
            chatHistory.AddUserMessage(message);

            // Use the default agent (FinancialAdvisor) for simple message handling
            try 
            {
                string agentName = "FinancialAdvisor";
                if (_agents.ContainsKey(agentName)) 
                {
                    string response = await _agents[agentName].GenerateResponseAsync(userId, message);
                    chatHistory.AddAssistantMessage(response);
                    return new ChatMessage(AuthorRole.Assistant, response);
                }
                
                // Fallback to old behavior if agent not found
                string fallbackResponse;
                
                if (message.Contains("recommend") || message.Contains("advice"))
                {
                    fallbackResponse = await _budgetPlugin.GetFinancialRecommendations(userId);
                }
                else if (message.Contains("income") || message.Contains("earning"))
                {
                    fallbackResponse = await _budgetPlugin.AnalyzeIncome(userId);
                }
                else if (message.Contains("expense") || message.Contains("spending"))
                {
                    fallbackResponse = await _budgetPlugin.AnalyzeExpenses(userId);
                }
                else if (message.Contains("categorize") || message.Contains("transaction"))
                {
                    fallbackResponse = await _budgetPlugin.AutoCategorizeTransactions(userId);
                }
                else
                {
                    fallbackResponse = await _budgetPlugin.AskFinancialQuestion(userId, message);
                }

                chatHistory.AddAssistantMessage(fallbackResponse);
                return new ChatMessage(AuthorRole.Assistant, fallbackResponse);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Sorry, I encountered an error: {ex.Message}";
                chatHistory.AddAssistantMessage(errorMessage);
                return new ChatMessage(AuthorRole.Assistant, errorMessage);
            }
        }

        public async Task<ChatMessage> SendMessageToAgentAsync(string sessionId, int userId, string agentName, string message)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                agentName = "FinancialAdvisor"; // Default agent
            }
            
            if (!_agents.ContainsKey(agentName))
            {
                throw new ArgumentException($"Agent '{agentName}' not found. Available agents: {string.Join(", ", _agents.Keys)}");
            }
            
            // Get or create chat history for this session
            var chatHistory = _chatHistories.GetOrAdd(sessionId, _ => new ChatHistory());
            
            // Add the user message to history
            chatHistory.AddUserMessage(message);
            
            try
            {
                // Get response from the specified agent
                var agent = _agents[agentName];
                string response = await agent.GenerateResponseAsync(userId, message);
                
                // Add the assistant's response to history
                chatHistory.AddAssistantMessage(response);

                return new ChatMessage(AuthorRole.Assistant, response);
            }
            catch (Exception ex)
            {
                // Add error message to chat history
                chatHistory.AddAssistantMessage($"Sorry, I encountered an error: {ex.Message}");
                return new ChatMessage(AuthorRole.Assistant, $"Sorry, I encountered an error: {ex.Message}");
            }
        }

        public IEnumerable<AgentInfo> GetAvailableAgents()
        {
            return _agents.Select(a => new AgentInfo 
            { 
                Name = a.Key, 
                Description = a.Value.Description 
            });
        }

        public ChatHistory GetChatHistory(string sessionId)
        {
            return _chatHistories.GetOrAdd(sessionId, _ => new ChatHistory());
        }

        public void ClearChatHistory(string sessionId)
        {
            _chatHistories.TryRemove(sessionId, out _);
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ChatMessage(AuthorRole role, string content)
        {
            Role = role.ToString();
            Content = content;
        }
    }

    public class AgentInfo
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
