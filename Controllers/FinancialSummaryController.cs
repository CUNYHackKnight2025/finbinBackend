using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetBackend.Data;
using BudgetBackend.Models;

namespace BudgetBackend.Controllers;

[Route("api/financial-summary")]
[ApiController]
public class FinancialSummaryController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet("{userId}")]
    public IActionResult GetFinancialSummary(int userId)
    {
        var summary = _context.FinancialSummaries
            .Include(fs => fs.Income)
            .Include(fs => fs.Expenses)
            .FirstOrDefault(fs => fs.UserId == userId);

        if (summary == null)
        {
            return NotFound("Financial summary not found.");
        }

        return Ok(summary);
    }

    [HttpPost("add/{userId}")]
    public IActionResult AddFinancialSummary(int userId, [FromBody] FinancialSummary summary)
    {
        var user = _context.Users.Include(u => u.FinancialSummary).FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (user.FinancialSummary != null)
        {
            return BadRequest("User already has a financial summary.");
        }

        summary.UserId = userId;
        _context.FinancialSummaries.Add(summary);
        _context.SaveChanges();

        return Ok(summary);
    }

    [HttpPut("update/{userId}")]
    public IActionResult UpdateFinancialSummary(int userId, [FromBody] FinancialSummary updatedSummary)
    {
        var existingSummary = _context.FinancialSummaries
            .Include(fs => fs.Income)
            .Include(fs => fs.Expenses)
            .FirstOrDefault(fs => fs.UserId == userId);

        if (existingSummary == null)
        {
            return NotFound("Financial summary not found.");
        }

        existingSummary.SavingsBalance = updatedSummary.SavingsBalance;
        existingSummary.InvestmentBalance = updatedSummary.InvestmentBalance;
        existingSummary.DebtBalance = updatedSummary.DebtBalance;

        _context.SaveChanges();

        return Ok(existingSummary);
    }

    [HttpDelete("delete/{userId}")]
    public IActionResult DeleteFinancialSummary(int userId)
    {
        var summary = _context.FinancialSummaries.FirstOrDefault(fs => fs.UserId == userId);

        if (summary == null)
        {
            return NotFound("Financial summary not found.");
        }

        _context.FinancialSummaries.Remove(summary);
        _context.SaveChanges();

        return Ok("Financial summary deleted successfully.");
    }
}
