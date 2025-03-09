using BudgetBackend.Models;
using BudgetBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserHistory(int limit = 50, int offset = 0)
        {
            int userId = GetUserId();
            var history = await _historyService.GetUserHistoryAsync(userId, limit, offset);
            return Ok(history);
        }

        [HttpGet("summaries")]
        public async Task<IActionResult> GetHistorySummaries(int limit = 10, int offset = 0)
        {
            int userId = GetUserId();
            var summaries = await _historyService.GetHistorySummariesAsync(userId, limit, offset);
            return Ok(summaries);
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> ManualSummarize(DateTime fromDate, DateTime toDate)
        {
            int userId = GetUserId();
            var summary = await _historyService.SummarizeHistoryPeriodAsync(userId, fromDate, toDate);
            
            if (summary == null)
                return NotFound("No history entries found in the specified date range.");
                
            return Ok(summary);
        }

        private int GetUserId()
        {
            // Get the user ID from the JWT token claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");
                
            return int.Parse(userIdClaim);
        }
    }
}
