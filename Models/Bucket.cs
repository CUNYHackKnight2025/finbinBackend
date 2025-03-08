using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBackend.Models
{
    public class Bucket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public User? User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentSavedAmount { get; set; } = 0;

        [Range(0.0, 1.0)]
        [Column(TypeName = "decimal(3,2)")] // Stores up to 0.99 precision
        public decimal PriorityScore { get; set; } = 0.5m; // Default 50% importance

        public DateTime Deadline { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Completed, Canceled
    }
}
