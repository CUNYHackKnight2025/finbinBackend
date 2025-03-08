using Microsoft.SemanticKernel;
using BudgetBackend.Data;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BudgetBackend.Plugins;
public class BudgetPlugin(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [KernelFunction]
    [Description("Analyzes the user's income and provides insights.")]
    public async Task<string> AnalyzeIncome(int userId)
    {
        var summary = await _dbContext.FinancialSummaries
            .Include(s => s.Income)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (summary?.Income == null)
            return "No income data found.";

        return $"User's salary: {summary.Income.Salary:C}, Investments: {summary.Income.Investments:C}, Business Income: {summary.Income.BusinessIncome:C}.";
    }

    [KernelFunction]
    [Description("Analyzes the user's expenses and suggests optimizations.")]
    public async Task<string> AnalyzeExpenses(int userId)
    {
        var summary = await _dbContext.FinancialSummaries
            .Include(s => s.Expenses)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (summary == null || summary.Expenses == null)
            return "No expense data found.";

        decimal totalExpenses = 
            (summary.Expenses?.RentMortgage ?? 0m) +
            (summary.Expenses?.Utilities ?? 0m) +
            (summary.Expenses?.LoanPayments ?? 0m) +
            (summary.Expenses?.Insurance ?? 0m) +
            (summary.Expenses?.Groceries ?? 0m) +
            (summary.Expenses?.Transportation ?? 0m) +
            (summary.Expenses?.Subscriptions ?? 0m) +
            (summary.Expenses?.Entertainment ?? 0m);

        return $"Total Expenses: {totalExpenses:C}. Breakdown - Rent: {summary.Expenses?.RentMortgage:C}, Utilities: {summary.Expenses?.Utilities:C}, Loans: {summary.Expenses?.LoanPayments:C}.";
    }

    [KernelFunction]
    [Description("Analyzes the user's net worth and provides financial guidance.")]
    public async Task<string> AnalyzeFinancialSummary(int userId)
    {
        var summary = await _dbContext.FinancialSummaries
            .Include(s => s.Income)
            .Include(s => s.Expenses)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (summary == null)
            return "No financial summary found.";

        decimal totalExpenses = 
            (summary.Expenses?.RentMortgage ?? 0m) +
            (summary.Expenses?.Utilities ?? 0m) +
            (summary.Expenses?.LoanPayments ?? 0m) +
            (summary.Expenses?.Insurance ?? 0m) +
            (summary.Expenses?.Groceries ?? 0m) +
            (summary.Expenses?.Transportation ?? 0m) +
            (summary.Expenses?.Subscriptions ?? 0m) +
            (summary.Expenses?.Entertainment ?? 0m);

        decimal totalIncome = 
            (summary.Income?.Salary ?? 0m) +
            (summary.Income?.Investments ?? 0m) +
            (summary.Income?.BusinessIncome ?? 0m);

        decimal netWorth = 
            (summary?.InvestmentBalance ?? 0m) +
            (summary?.SavingsBalance ?? 0m) -
            (summary?.DebtBalance ?? 0m);

        return $"Net Worth: {netWorth:C}. Total Income: {totalIncome:C}, Total Expenses: {totalExpenses:C}.";
    }
}
