using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;

namespace BudgetBackend.Plugins;

public class IncomeTrackerPlugin(IChatCompletionService chatCompletionService, Kernel kernel)
{
    private readonly IChatCompletionService _chatCompletionService = chatCompletionService;
    private readonly Kernel _kernel = kernel;

    [KernelFunction]
    [Description("Analyzes and categorizes income sources using AI.")]
    public async Task<string> AnalyzeIncome(decimal salary, decimal investments, decimal businessIncome)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage($"Analyze my income breakdown: Salary - {salary:C}, Investments - {investments:C}, Business - {businessIncome:C}. Provide insights.");

        var response = await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            kernel: _kernel
        );

        return response?.Content ?? "No response received.";
    }
}
