using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBackend.Models
{
    public class HistorySummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }
        
        [Required]
        public string? SummaryText { get; set; }
        
        // Optional: IDs of original history entries that were summarized
        public string? SummarizedEntryIds { get; set; }
    }
}
