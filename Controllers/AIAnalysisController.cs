using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Plugins;

namespace BudgetBackend.Controllers;

[Route("api/ai-analysis")]
[ApiController]
public class AIAnalysisController(BudgetPlugin budgetPlugin) : ControllerBase
{
    private readonly BudgetPlugin _budgetPlugin = budgetPlugin;

    /// <summary>
    /// Provides AI-powered recommendations for optimizing savings and expenses.
    /// </summary>
    [HttpGet("recommendations/{userId}")]
    public async Task<IActionResult> GetFinancialRecommendations(int userId)
    {
        var result = await _budgetPlugin.GetFinancialRecommendations(userId);
        return Ok(new { recommendations = result });
    }

    /// <summary>
    /// Analyzes the user's income and provides insights.
    /// </summary>
    [HttpGet("income/{userId}")]
    public async Task<IActionResult> AnalyzeIncome(int userId)
    {
        var result = await _budgetPlugin.AnalyzeIncome(userId);
        return Ok(new { analysis = result });
    }

    /// <summary>
    /// Analyzes the user's expenses and suggests optimizations.
    /// </summary>
    [HttpGet("expenses/{userId}")]
    public async Task<IActionResult> AnalyzeExpenses(int userId)
    {
        var result = await _budgetPlugin.AnalyzeExpenses(userId);
        return Ok(new { analysis = result });
    }


    /// <summary>
    /// Analyzes the user's financial summary and provides guidance.
    /// </summary>
    [HttpGet("summary/{userId}")]
    public async Task<IActionResult> AnalyzeFinancialSummary(int userId)
    {
        var result = await _budgetPlugin.AnalyzeFinancialSummary(userId);
        return Ok(new { summary = result });
    }
}
