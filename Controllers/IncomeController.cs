using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using BudgetBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BudgetBackend.Controllers;

[Route("api/income")]
[ApiController]
public class IncomeController(IChatCompletionService chatCompletionService) : ControllerBase
{
    private readonly IChatCompletionService _chatCompletionService = chatCompletionService;

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeIncome([FromBody] BudgetRecord budget)
    {
        var chatHistory = new ChatHistory();

        string userMessage = $"Analyze my budget: Salary - {budget.Salary:C}, Investments - {budget.Investments:C}";
        userMessage += $", Expenses - {budget.TotalExpenses:C}, Savings - {budget.SavingsBalance:C}, Debt - {budget.DebtBalance:C}.";

        chatHistory.AddUserMessage(userMessage);

        var response = await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: null
        );

        var jsonResponse = new
        {
            income_summary = new
            {
                budget.Salary,
                budget.Investments,
                budget.TotalIncome
            },
            expense_analysis = new
            {
                budget.RentMortgage,
                budget.Utilities,
                budget.Insurance,
                budget.LoanPayments,
                budget.Groceries,
                budget.Transportation,
                budget.Subscriptions,
                budget.Entertainment,
                budget.TotalExpenses
            },
            financial_health = new
            {
                budget.SavingsBalance,
                budget.InvestmentBalance,
                budget.DebtBalance,
                budget.NetWorth,
                budget.SavingsRate,
                budget.DebtToIncomeRatio
            },
            ai_analysis = response?.Content
        };

        return Ok(jsonResponse);
    }
}
