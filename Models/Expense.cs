using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBackend.Models
{
    public class Expenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal RentMortgage { get; set; }
        public decimal Utilities { get; set; }
        public decimal Insurance { get; set; }
        public decimal LoanPayments { get; set; }
        public decimal Groceries { get; set; }
        public decimal Transportation { get; set; }
        public decimal Subscriptions { get; set; }
        public decimal Entertainment { get; set; }

        // Foreign key to FinancialSummary
        public int FinancialSummaryId { get; set; }
        public FinancialSummary? FinancialSummary { get; set; }
    }
}
