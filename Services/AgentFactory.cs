using BudgetBackend.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BudgetBackend.Services
{
    public class AgentFactory
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;

        public AgentFactory(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel;
        }

        public IAgent CreateFinancialAdvisor()
        {
            return new FinancialAdvisorAgent(_chatService, _kernel);
        }

        public IAgent CreateBudgetAnalyst()
        {
            return new BudgetAnalystAgent(_chatService, _kernel);
        }
    }

    public class FinancialAdvisorAgent : IAgent
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;
        
        public string Name => "FinancialAdvisor";
        public string Description => "An AI advisor that provides financial guidance and recommendations.";

        public FinancialAdvisorAgent(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel;
        }

        public async Task<string> GenerateResponseAsync(int userId, string message)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are a financial advisor. Help the user with financial decisions and recommendations.");
            chatHistory.AddUserMessage(message);
            
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            return response.Content ?? "I'm sorry, I couldn't generate a response.";
        }
    }

    public class BudgetAnalystAgent : IAgent
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;
        
        public string Name => "BudgetAnalyst";
        public string Description => "An AI analyst that helps analyze spending patterns and budget optimization.";

        public BudgetAnalystAgent(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel;
        }

        public async Task<string> GenerateResponseAsync(int userId, string message)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are a budget analyst. Help the user understand their spending patterns and optimize their budget.");
            chatHistory.AddUserMessage(message);
            
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            return response.Content ?? "I'm sorry, I couldn't generate a response.";
        }
    }
}
