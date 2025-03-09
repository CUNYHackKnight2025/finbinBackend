using BudgetBackend.Models;
using BudgetBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Security.Claims;

namespace BudgetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly FinancialChatService _chatService;

        public ChatController(FinancialChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequests request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }
            
            try
            {
                // For testing/debugging, allow user ID to be specified in the request
                int userId;
                
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
                    {
                        return Unauthorized("User not authenticated properly");
                    }
                }
                else if (request.UserId.HasValue)
                {
                    // For testing only
                    userId = request.UserId.Value;
                }
                else
                {
                    return Unauthorized("User not authenticated");
                }

                // Use the specified agent or default to FinancialAdvisor
                string agentName = request.AgentName ?? "FinancialAdvisor";
                
                var response = await _chatService.SendMessageToAgentAsync(
                    request.SessionId ?? Guid.NewGuid().ToString(),
                    userId,
                    agentName,
                    request.Message
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing message: {ex.Message}");
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
                return BadRequest($"Error retrieving agents: {ex.Message}");
            }
        }

        [HttpGet("history/{sessionId}")]
        public IActionResult GetChatHistory(string sessionId)
        {
            var history = _chatService.GetChatHistory(sessionId);
            
            // Convert the ChatHistory to a list of ChatMessage objects for the API response
            var messages = new List<ChatMessage>();
            
            foreach (var message in history) 
            {
                AuthorRole role = message.Role == AuthorRole.User ? AuthorRole.User : AuthorRole.Assistant;
                messages.Add(new ChatMessage(role, message.Content ?? string.Empty));
            }
            
            return Ok(messages);
        }

        [HttpDelete("history/{sessionId}")]
        public IActionResult ClearChatHistory(string sessionId)
        {
            _chatService.ClearChatHistory(sessionId);
            return Ok();
        }
    }

    public class ChatRequests
    {
        public string? SessionId { get; set; }
        public required string Message { get; set; }
        public string? AgentName { get; set; }
        public int? UserId { get; set; } // For testing/debugging
    }
}
