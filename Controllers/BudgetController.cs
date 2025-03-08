/*
Going to expose tthe endpoints here for adding and getting some budget data~
GET all buckets
POST a bucket
*/

using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Data;
using BudgetBackend.Models;

[Route("api/budget")]
[ApiController]
public class BudgetController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BudgetController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var records = _context.BudgetRecords.ToList();
        return Ok(records);
    }

    [HttpPost]
    public IActionResult AddRecord([FromBody] BudgetRecord record)
    {
        _context.BudgetRecords.Add(record);
        _context.SaveChanges();
        return Ok(record);
    }
}
