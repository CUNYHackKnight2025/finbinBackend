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
            .Include(b => b.User) // Ensure user is included
            .ToListAsync();

        return Ok(buckets);
    }


   [HttpGet("{userId}/{bucketId}")]
    public async Task<IActionResult> GetBucketById(int userId, int bucketId)
    {
        var bucket = await _context.Buckets
            .Include(b => b.User) // Ensure user is included
            .FirstOrDefaultAsync(b => b.Id == bucketId && b.UserId == userId);

        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket);
    }


    [HttpPost]
    public async Task<IActionResult> CreateBucket([FromBody] Bucket bucket)
    {
        // Ensure the user exists in the database
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
}
