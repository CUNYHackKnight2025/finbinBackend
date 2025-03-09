using Microsoft.AspNetCore.Mvc;
using BudgetBackend.Data;
using BudgetBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetBackend.Controllers;

[Route("api/buckets")]
[ApiController]
public class BucketController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBucketsByUser(int userId)
    {
        var buckets = await _context.Buckets
            .Where(b => b.UserId == userId)
            .Include(b => b.User) // Ensures user relationship is loaded
            .ToListAsync();

        if (!buckets.Any())
        {
            return NotFound($"No buckets found for user ID {userId}.");
        }

        return Ok(buckets);
    }

    [HttpGet("{userId}/{bucketId}")]
    public async Task<IActionResult> GetBucketById(int userId, int bucketId)
    {
        var bucket = await _context.Buckets
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bucketId && b.UserId == userId);

        if (bucket == null)
        {
            return NotFound("Bucket not found.");
        }

        return Ok(bucket);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBucket([FromBody] Bucket bucket)
    {
        // Ensure the user exists
        var user = await _context.Users.FindAsync(bucket.UserId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        // Attach the user to the bucket
        bucket.User = user;

        _context.Buckets.Add(bucket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBucketById), new { userId = bucket.UserId, bucketId = bucket.Id }, bucket);
    }

    [HttpPut("{userId}/{bucketId}")]
    public async Task<IActionResult> UpdateBucket(int userId, int bucketId, [FromBody] Bucket updatedBucket)
    {
        var existingBucket = await _context.Buckets
            .FirstOrDefaultAsync(b => b.Id == bucketId && b.UserId == userId);

        if (existingBucket == null)
            return NotFound("Bucket not found.");

        existingBucket.Name = updatedBucket.Name;
        existingBucket.TargetAmount = updatedBucket.TargetAmount;
        existingBucket.CurrentSavedAmount = updatedBucket.CurrentSavedAmount;
        existingBucket.PriorityScore = updatedBucket.PriorityScore;
        existingBucket.Status = updatedBucket.Status;
        existingBucket.Deadline = updatedBucket.Deadline;

        await _context.SaveChangesAsync();
        return Ok(existingBucket);
    }

    [HttpDelete("{userId}/{bucketId}")]
    public async Task<IActionResult> DeleteBucket(int userId, int bucketId)
    {
        var bucket = await _context.Buckets
            .FirstOrDefaultAsync(b => b.Id == bucketId && b.UserId == userId);

        if (bucket == null)
            return NotFound("Bucket not found.");

        _context.Buckets.Remove(bucket);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{userId}/{bucketId}/priority")]
    public async Task<IActionResult> UpdatePriorityScore(int userId, int bucketId, [FromBody] decimal newPriorityScore)
    {
        if (newPriorityScore < 0.0m || newPriorityScore > 1.0m)
        {
            return BadRequest("Priority score must be between 0.0 and 1.0.");
        }

        var bucket = await _context.Buckets
            .FirstOrDefaultAsync(b => b.Id == bucketId && b.UserId == userId);

        if (bucket == null)
        {
            return NotFound("Bucket not found.");
        }

        bucket.PriorityScore = newPriorityScore;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Priority score updated successfully.", bucket });
    }

    [HttpGet("{userId}/adjust-expenses")]
    public async Task<IActionResult> GetExpenseAdjustments(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FinancialSummary)
            .ThenInclude(fs => fs!.Expenses)
            .Include(u => u.Buckets)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.FinancialSummary == null || user.FinancialSummary.Expenses == null)
        {
            return NotFound("User financial data not found.");
        }

        var expenses = user.FinancialSummary.Expenses;
        var totalIncome = user.FinancialSummary.TotalIncome;
        var totalExpenses = user.FinancialSummary.TotalExpenses;
        var availableFunds = totalIncome - totalExpenses;
        var buckets = user.Buckets.OrderByDescending(b => b.PriorityScore).ToList();

        if (buckets.Count == 0)
        {
            return Ok(new { message = "No active savings goals found." });
        }

        var adjustments = new List<object>();

        foreach (var bucket in buckets)
        {
            decimal suggestedAllocation = bucket.PriorityScore * availableFunds;
            suggestedAllocation = Math.Min(suggestedAllocation, bucket.TargetAmount - bucket.CurrentSavedAmount);

            if (suggestedAllocation > 0)
            {
                adjustments.Add(new
                {
                    bucketId = bucket.Id,
                    bucketName = bucket.Name,
                    suggestedAllocation
                });
            }
        }

        return Ok(new
        {
            totalAvailableFunds = availableFunds,
            suggestedAdjustments = adjustments
        });
    }
}
