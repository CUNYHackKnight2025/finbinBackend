using BudgetBackend.Data;
using BudgetBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace BudgetBackend.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatCompletionService _chatService;

        public HistoryService(ApplicationDbContext context, IChatCompletionService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public async Task AddHistoryEntryAsync(int userId, string eventType, string description, string additionalData = null)
        {
            var historyEntry = new UserHistory
            {
                UserId = userId,
                EventType = eventType,
                Description = description,
                AdditionalData = additionalData,
                Timestamp = DateTime.UtcNow
            };

            _context.UserHistories.Add(historyEntry);
            await _context.SaveChangesAsync();

            // Check if we need to summarize history
            await CheckAndSummarizeHistoryAsync(userId);
        }

        public async Task<IEnumerable<UserHistory>> GetUserHistoryAsync(int userId, int limit = 50, int offset = 0)
        {
            return await _context.UserHistories
                .Where(h => h.UserId == userId && !h.IsSummarized)
                .OrderByDescending(h => h.Timestamp)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistorySummary>> GetHistorySummariesAsync(int userId, int limit = 10, int offset = 0)
        {
            return await _context.HistorySummaries
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ToDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> CheckAndSummarizeHistoryAsync(int userId, int thresholdCount = 100)
        {
            // Count unsummarized history entries
            var count = await _context.UserHistories
                .Where(h => h.UserId == userId && !h.IsSummarized)
                .CountAsync();

            if (count >= thresholdCount)
            {
                // Determine the time period to summarize (we'll take the oldest entries up to threshold)
                var entries = await _context.UserHistories
                    .Where(h => h.UserId == userId && !h.IsSummarized)
                    .OrderBy(h => h.Timestamp)
                    .Take(thresholdCount)
                    .ToListAsync();

                if (entries.Any())
                {
                    var fromDate = entries.Min(h => h.Timestamp);
                    var toDate = entries.Max(h => h.Timestamp);

                    // Generate summary
                    await SummarizeHistoryPeriodAsync(userId, fromDate, toDate);
                    return true;
                }
            }

            return false;
        }

        public async Task<HistorySummary> SummarizeHistoryPeriodAsync(int userId, DateTime fromDate, DateTime toDate)
        {
            // Get all entries in the time period
            var entries = await _context.UserHistories
                .Where(h => h.UserId == userId && 
                       !h.IsSummarized && 
                       h.Timestamp >= fromDate && 
                       h.Timestamp <= toDate)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();

            if (!entries.Any())
                return null;

            // Build context for the AI summary
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Financial history events to summarize:");
            
            foreach (var entry in entries)
            {
                builder.AppendLine($"- {entry.Timestamp.ToString("yyyy-MM-dd")}: [{entry.EventType}] {entry.Description}");
            }

            // Use AI to generate summary
            var prompt = "Summarize these financial events into a concise paragraph highlighting key patterns, " +
                         "achievements, financial decisions, and overall progress. Focus on actionable insights " +
                         "and notable changes in financial behavior:";
                         
            var chatHistory = new ChatHistory(prompt);
            chatHistory.AddUserMessage(builder.ToString());

            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            string summaryText = response.Content;

            // Create summary record
            var summary = new HistorySummary
            {
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate,
                SummaryText = summaryText,
                SummarizedEntryIds = JsonSerializer.Serialize(entries.Select(e => e.Id))
            };

            _context.HistorySummaries.Add(summary);

            // Mark entries as summarized
            foreach (var entry in entries)
            {
                entry.IsSummarized = true;
            }

            await _context.SaveChangesAsync();
            return summary;
        }
    }
}
