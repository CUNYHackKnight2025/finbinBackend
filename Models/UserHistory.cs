using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBackend.Models
{
    public class UserHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public required string EventType { get; set; }  // Transaction, BucketCreated, GoalAchieved, etc.
        
        [MaxLength(500)]
        public required string Description { get; set; }
        
        // Additional data stored as JSON if needed
        public string? AdditionalData { get; set; }
        
        // Flag to indicate if this entry has been summarized
        public bool IsSummarized { get; set; } = false;
    }
}
