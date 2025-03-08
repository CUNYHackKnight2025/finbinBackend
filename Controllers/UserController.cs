using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetBackend.Data;
using BudgetBackend.Models;

namespace BudgetBackend.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] User user)
    {
        if (user.FinancialSummary != null)
        {
            user.FinancialSummary.UserId = user.Id; // Ensure proper linkage
            user.FinancialSummary.User = user;
        }

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }


    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _context.Users
            .Include(u => u.FinancialSummary)
            .ThenInclude(fs => fs!.Income)
            .Include(u => u.FinancialSummary!.Expenses)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}
