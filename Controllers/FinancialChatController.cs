using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Plugins;

namespace BudgetBackend.Controllers;

[Route("api/ai-chat")]
[ApiController]
public class FinancialChatController : ControllerBase
{
    private readonly BudgetPlugin _budgetPlugin;

    public FinancialChatController(BudgetPlugin budgetPlugin)
    {
        _budgetPlugin = budgetPlugin;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> AskFinancialQuestion(int userId, [FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Question cannot be empty.");

        var response = await _budgetPlugin.AskFinancialQuestion(userId, request.Question);
        return Ok(new { response });
    }

    [HttpPost("suggest-adjustments/{userId}")]
    public async Task<IActionResult> SuggestBudgetAdjustments(int userId)
    {
        var response = await _budgetPlugin.GetFinancialRecommendations(userId);
        return Ok(new { response });
    }
}

public class ChatRequest
{
    public string? Question { get; set; }
}
