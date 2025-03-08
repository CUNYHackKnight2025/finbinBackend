namespace BudgetBackend.Models
{
    public class BudgetRecord
    {
        public int Id { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetWorth { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal DebtToIncomeRatio { get; set; }
        public decimal Salary { get; set; }
        public decimal Investments { get; set; }
        public decimal RentMortgage { get; set; }
        public decimal Utilities { get; set; }
        public decimal Insurance { get; set; }
        public decimal LoanPayments { get; set; }
        public decimal Groceries { get; set; }
        public decimal Transportation { get; set; }
        public decimal Subscriptions { get; set; }
        public decimal Entertainment { get; set; }
        public decimal SavingsBalance { get; set; }
        public decimal InvestmentBalance { get; set; }
        public decimal DebtBalance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
