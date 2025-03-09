using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BudgetBackend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }
        
        // New password related fields
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];

        // Properties for password reset functionality
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public FinancialSummary? FinancialSummary { get; set; }  
        public ICollection<Bucket> Buckets { get; set; } = [];
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
