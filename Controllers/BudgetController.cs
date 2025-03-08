using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Data;
using BudgetBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetBackend.Controllers;

[Route("api/budget")]
[ApiController]
public class BudgetController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _context.BudgetRecords.ToListAsync();
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> AddRecord([FromBody] BudgetRecord record)
    {
        if (record == null)
        {
            return BadRequest("Invalid budget data.");
        }

        _context.BudgetRecords.Add(record);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = record.Id }, record);
    }
}
