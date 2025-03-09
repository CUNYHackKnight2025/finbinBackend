using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using BudgetBackend.Data;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using BudgetBackend.Models;

namespace BudgetBackend.Plugins;

public class BudgetPlugin
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatCompletionService _chatService;

    public BudgetPlugin(ApplicationDbContext dbContext, IChatCompletionService chatService)
    {
        _dbContext = dbContext;
        _chatService = chatService;
    }

    [KernelFunction]
    [Description("Provides AI-powered recommendations for optimizing savings and expenses.")]
    public async Task<string> GetFinancialRecommendations(int userId)
    {
        try
        {
            var summary = await _dbContext.FinancialSummaries
                .Include(s => s.Income)
                .Include(s => s.Expenses)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (summary == null)
                return "User financial data not found. Please set up your financial profile first.";

            decimal totalIncome = summary.TotalIncome;
            decimal totalExpenses = summary.TotalExpenses;
            decimal savings = summary.SavingsBalance;

            if (totalIncome <= 0)
            {
                return $"It looks like your income information is not set up correctly. Please update your income details.";
            }

            // Step 1: Identify high spending categories
            string spendingAnalysis = AnalyzeSpending(summary.Expenses!, totalIncome);

            // Step 2: Suggest savings adjustments
            var buckets = await _dbContext.Buckets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.PriorityScore)
                .ToListAsync();

            string savingsStrategy = SuggestSavingsAdjustments(buckets, savings);

            // Step 3: Generate AI response
            string prompt = $@"
            The user has a total income of {totalIncome:C} and expenses of {totalExpenses:C}.
            Their savings balance is {savings:C}.
            They have {buckets.Count} savings goals.
            Spending Analysis: {spendingAnalysis}
            Savings Strategy: {savingsStrategy}
            Based on this, generate a detailed, actionable financial recommendation.";

            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(prompt);

            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            return response?.Content ?? "No recommendations available.";
        }
        catch (Exception ex)
        {
            return $"I encountered an error analyzing your finances: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Provides AI-powered responses to financial questions.")]
    public async Task<string> AskFinancialQuestion(int userId, string question)
    {
        var user = await _dbContext.Users
            .Include(u => u.FinancialSummary)
                .ThenInclude(fs => fs!.Income)
            .Include(u => u.FinancialSummary)
                .ThenInclude(fs => fs!.Expenses)
            .Include(u => u.Buckets)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.FinancialSummary == null)
            return "User financial data not found.";

        string financialContext = $@"
        The user has a total income of {user.FinancialSummary.TotalIncome:C}.
        Their total expenses amount to {user.FinancialSummary.TotalExpenses:C}.
        Savings balance: {user.FinancialSummary.SavingsBalance:C}.
        They have {user.Buckets.Count} savings goals.";

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You are an empathetic yet impactful AI financial assistant.");
        chatHistory.AddUserMessage($"{financialContext}\nUser Question: {question}");

        var response = await _chatService.GetChatMessageContentAsync(chatHistory);
        return response?.Content ?? "No response available.";
    }

    private string AnalyzeSpending(Expenses expenses, decimal totalIncome)
    {
        if (totalIncome == 0)
        {
            return "Total income is zero, unable to analyze spending percentages.";
        }

        var highSpendingCategories = new List<string>();

        if (expenses.RentMortgage / totalIncome > 0.35m)
            highSpendingCategories.Add("Rent/Mortgage (above 35% of income)");

        if (expenses.Entertainment / totalIncome > 0.1m)
            highSpendingCategories.Add("Entertainment (above 10% of income)");

        if (expenses.Subscriptions / totalIncome > 0.05m)
            highSpendingCategories.Add("Subscriptions (above 5% of income)");

        if (!highSpendingCategories.Any())
            return "No excessive spending detected.";

        return "High spending detected in: " + string.Join(", ", highSpendingCategories);
    }

    private string SuggestSavingsAdjustments(List<Bucket> buckets, decimal savings)
    {
        if (!buckets.Any())
            return "No savings goals found.";

        var recommendations = new List<string>();

        foreach (var bucket in buckets)
        {
            decimal requiredAmount = bucket.TargetAmount - bucket.CurrentSavedAmount;
            decimal suggestedAllocation = Math.Min(bucket.PriorityScore * savings, requiredAmount);

            if (suggestedAllocation > 0)
            {
                recommendations.Add($"Allocate {suggestedAllocation:C} to **{bucket.Name}** (Priority: {bucket.PriorityScore:P}).");
            }
        }

        return recommendations.Any() ? string.Join(" ", recommendations) : "No savings adjustments needed.";
    }

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

        decimal totalExpenses = summary.TotalExpenses;
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

        decimal totalExpenses = summary.TotalExpenses;
        decimal totalIncome = summary.TotalIncome;
        decimal netWorth = 
            (summary?.InvestmentBalance ?? 0m) +
            (summary?.SavingsBalance ?? 0m) -
            (summary?.DebtBalance ?? 0m);

        return $"Net Worth: {netWorth:C}. Total Income: {totalIncome:C}, Total Expenses: {totalExpenses:C}.";
    }

    [KernelFunction]
    [Description("Auto-categorizes transactions based on predefined rules.")]
    public async Task<string> AutoCategorizeTransactions(int userId)
    {
        var transactions = await _dbContext.Transactions
            .Where(t => t.UserId == userId && string.IsNullOrEmpty(t.Category))
            .ToListAsync();

        if (!transactions.Any())
            return "No uncategorized transactions found.";

        foreach (var transaction in transactions)
        {
            transaction.Category = CategorizeTransaction(transaction);
        }

        await _dbContext.SaveChangesAsync();
        return "Transactions have been auto-categorized.";
    }

    private string CategorizeTransaction(Transaction transaction)
    {
        // Implement categorization logic here
        // For example, based on transaction description or amount
        return "Uncategorized";
    }

    [KernelFunction]
    [Description("Generates a financial report for the user.")]
    public async Task<string> GenerateFinancialReport(int userId)
    {
        var summary = await _dbContext.FinancialSummaries
            .Include(s => s.Income)
            .Include(s => s.Expenses)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (summary == null)
            return "No financial summary found.";

        // Generate report logic here
        string report = $"Financial Report for User {userId}:\n" +
                        $"Total Income: {summary.TotalIncome:C}\n" +
                        $"Total Expenses: {summary.TotalExpenses:C}\n" +
                        $"Net Worth: {(summary.InvestmentBalance + summary.SavingsBalance - summary.DebtBalance):C}";

        return report;
    }

    [KernelFunction]
    [Description("Sets budget limits dynamically based on user preferences.")]
    public async Task<string> SetBudgetLimits(int userId, decimal newLimit)
    {
        var user = await _dbContext.Users
            .Include(u => u.Buckets)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return "User not found.";

        foreach (var bucket in user.Buckets)
        {
            bucket.TargetAmount = newLimit;
        }

        await _dbContext.SaveChangesAsync();
        return $"Budget limits have been set to {newLimit:C} for all buckets.";
    }
}
