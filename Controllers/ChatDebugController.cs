using BudgetBackend.Models;
using BudgetBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BudgetBackend.Controllers
{
    [ApiController]
    [Route("api/debug/[controller]")]
    public class ChatDebugController : ControllerBase
    {
        private readonly FinancialChatService _chatService;

        public ChatDebugController(FinancialChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestChat([FromBody] TestChatRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message) || !request.UserId.HasValue)
            {
                return BadRequest("Message and UserId are required");
            }

            try
            {
                var agentName = request.AgentName ?? "FinancialAdvisor";
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

                var response = await _chatService.SendMessageToAgentAsync(
                    sessionId,
                    request.UserId.Value,
                    agentName,
                    request.Message
                );
                
                return Ok(new
                {
                    SessionId = sessionId,
                    Message = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpGet("agents")]
        public IActionResult GetAvailableAgents()
        {
            try
            {
                var agents = _chatService.GetAvailableAgents();
                return Ok(agents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }

    public class TestChatRequest
    {
        public string? SessionId { get; set; }
        public required string Message { get; set; }
        public string? AgentName { get; set; }
        public int? UserId { get; set; }
    }
}
