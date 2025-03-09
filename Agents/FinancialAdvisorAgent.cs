using BudgetBackend.Models;
using BudgetBackend.Plugins;
using BudgetBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BudgetBackend.Agents
{
    public class FinancialAdvisorAgent
    {
        private readonly BudgetPlugin _budgetPlugin;
        private readonly IChatCompletionService _chatService;

        public string Name { get; } = "FinancialAdvisor";
        public string Description { get; } = "I'm your personal financial advisor. I can analyze your spending, suggest budget improvements, and help you reach your financial goals.";

        public FinancialAdvisorAgent(BudgetPlugin budgetPlugin, IChatCompletionService chatService)
        {
            _budgetPlugin = budgetPlugin;
            _chatService = chatService;
        }

        public async Task<string> GenerateResponseAsync(int userId, string message)
        {
            // Determine intent from message
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are a financial assistant that categorizes user queries. Respond with only one of these categories: RECOMMENDATIONS, INCOME, EXPENSES, SUMMARY, TRANSACTIONS, or GENERAL.");
            chatHistory.AddUserMessage(message);
            
            var intentResponse = await _chatService.GetChatMessageContentAsync(chatHistory);
            var intent = intentResponse?.Content?.Trim().ToUpperInvariant() ?? "GENERAL";

            // Call appropriate plugin function based on intent
            string response = intent switch
            {
                "RECOMMENDATIONS" => await _budgetPlugin.GetFinancialRecommendations(userId),
                "INCOME" => await _budgetPlugin.AnalyzeIncome(userId),
                "EXPENSES" => await _budgetPlugin.AnalyzeExpenses(userId),
                "SUMMARY" => await _budgetPlugin.AnalyzeFinancialSummary(userId),
                "TRANSACTIONS" => await _budgetPlugin.AutoCategorizeTransactions(userId),
                _ => await _budgetPlugin.AskFinancialQuestion(userId, message)
            };

            return response;
        }
    }
}
