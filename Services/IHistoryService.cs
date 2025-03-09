using BudgetBackend.Models;

namespace BudgetBackend.Services
{
    public interface IHistoryService
    {
        Task AddHistoryEntryAsync(int userId, string eventType, string description, string additionalData = null);
        
        Task<IEnumerable<UserHistory>> GetUserHistoryAsync(int userId, int limit = 50, int offset = 0);
        
        Task<IEnumerable<HistorySummary>> GetHistorySummariesAsync(int userId, int limit = 10, int offset = 0);
        
        Task<bool> CheckAndSummarizeHistoryAsync(int userId, int thresholdCount = 100);
        
        // Optional: Manually trigger summarization
        Task<HistorySummary> SummarizeHistoryPeriodAsync(int userId, DateTime fromDate, DateTime toDate);
    }
}
