using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Plugins;

namespace BudgetBackend.Controllers;

[Route("api/ai-chat")]
[ApiController]
public class FinancialChatController(BudgetPlugin budgetPlugin) : ControllerBase
{
    private readonly BudgetPlugin _budgetPlugin = budgetPlugin;

    [HttpPost("{userId}")]
    public async Task<IActionResult> AskFinancialQuestion(int userId, [FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Question cannot be empty.");

        var response = await _budgetPlugin.AskFinancialQuestion(userId, request.Question);
        return Ok(new { response });
    }
}

public class ChatRequest
{
    public string? Question { get; set; }
}
