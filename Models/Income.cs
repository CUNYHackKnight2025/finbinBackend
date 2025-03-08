using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBackend.Models
{

public class Income
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public decimal Salary { get; set; }
    public decimal Investments { get; set; }
    public decimal BusinessIncome { get; set; }

    // Foreign key to FinancialSummary
    public int FinancialSummaryId { get; set; }
    public FinancialSummary? FinancialSummary { get; set; }
}

}
