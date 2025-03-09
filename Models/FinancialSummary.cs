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
        [NotMapped]
        public decimal TotalIncome => (Income?.Salary ?? 0) + (Income?.Investments ?? 0) + (Income?.BusinessIncome ?? 0);
        
        [NotMapped]
        public decimal TotalExpenses =>
            (Expenses?.RentMortgage ?? 0) +
            (Expenses?.Utilities ?? 0) +
            (Expenses?.Insurance ?? 0) +
            (Expenses?.LoanPayments ?? 0) +
            (Expenses?.Groceries ?? 0) +
            (Expenses?.Transportation ?? 0) +
            (Expenses?.Subscriptions ?? 0) +
            (Expenses?.Entertainment ?? 0);
    }
}
