using BudgetBackend.Plugins;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BudgetBackend.Agents
{
    public class AgentFactory
    {
        private readonly BudgetPlugin _budgetPlugin;
        private readonly IChatCompletionService _chatService;
        private readonly ILogger<AgentFactory> _logger;

        public AgentFactory(BudgetPlugin budgetPlugin, IChatCompletionService chatService, ILogger<AgentFactory>? logger = null)
        {
            _budgetPlugin = budgetPlugin ?? throw new ArgumentNullException(nameof(budgetPlugin));
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentFactory>.Instance;
        }

        public FinancialAdvisorAgent CreateFinancialAdvisor()
        {
            _logger.LogInformation("Creating Financial Advisor agent");
            return new FinancialAdvisorAgent(_budgetPlugin, _chatService);
        }

        public BudgetAnalystAgent CreateBudgetAnalyst()
        {
            _logger.LogInformation("Creating Budget Analyst agent");
            return new BudgetAnalystAgent(_budgetPlugin, _chatService);
        }
    }

    public class BudgetAnalystAgent
    {
        private readonly BudgetPlugin _budgetPlugin;
        private readonly IChatCompletionService _chatService;

        public string Name { get; } = "BudgetAnalyst";
        public string Description { get; } = "I specialize in analyzing your spending patterns and suggesting ways to optimize your budget.";

        public BudgetAnalystAgent(BudgetPlugin budgetPlugin, IChatCompletionService chatService)
        {
            _budgetPlugin = budgetPlugin;
            _chatService = chatService;
        }

        public async Task<string> GenerateResponseAsync(int userId, string message)
        {
            // Focus on expense analysis and budget optimization
            if (message.Contains("where") || message.Contains("spending"))
            {
                return await _budgetPlugin.AnalyzeExpenses(userId);
            }
            else if (message.Contains("save") || message.Contains("reduce"))
            {
                return await _budgetPlugin.GetFinancialRecommendations(userId);
            }
            else
            {
                // General budget questions
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage("You are a budget optimization specialist. Focus on helping the user reduce unnecessary expenses and optimize their budget.");
                chatHistory.AddUserMessage(message);
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response?.Content ?? "I couldn't analyze your budget at this time.";
            }
        }
    }
}
