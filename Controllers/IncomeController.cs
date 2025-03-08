using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using BudgetBackend.Models;
using BudgetBackend.Data;
using System.Threading.Tasks;
using System.Linq;

namespace BudgetBackend.Controllers;

[Route("api/income")]
[ApiController]
public class IncomeController(IChatCompletionService chatCompletionService, ApplicationDbContext dbContext) : ControllerBase
{
    private readonly IChatCompletionService _chatCompletionService = chatCompletionService;
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpPost("analyze/{userId}")]
    public async Task<IActionResult> AnalyzeIncome(int userId)
    {
        var income = _dbContext.Incomes.FirstOrDefault(i => i.FinancialSummaryId == userId);
        if (income == null)
        {
            return NotFound(new { message = "Income data not found for the user." });
        }

        var chatHistory = new ChatHistory();
        string userMessage = $"Analyze my income: Salary - {income.Salary:C}, Investments - {income.Investments:C}, Business Income - {income.BusinessIncome:C}.";

        chatHistory.AddUserMessage(userMessage);

        var response = await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: null
        );

        var jsonResponse = new
        {
            income_summary = new
            {
                income.Salary,
                income.Investments,
                income.BusinessIncome,
                TotalIncome = income.Salary + income.Investments + income.BusinessIncome
            },
            ai_analysis = response?.Content
        };

        return Ok(jsonResponse);
    }
}
