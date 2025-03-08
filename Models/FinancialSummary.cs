using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BudgetBackend.Models
{
    public class FinancialSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal SavingsBalance { get; set; }
        public decimal InvestmentBalance { get; set; }
        public decimal DebtBalance { get; set; }

        public int UserId { get; set; }

        [JsonIgnore] // Prevent circular serialization
        public User? User { get; set; }  // Make it nullable

        public Income? Income { get; set; }
        public Expenses? Expenses { get; set; }
    }

}
