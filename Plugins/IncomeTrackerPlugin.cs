using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace BudgetBackend.Plugins;

public class IncomeTrackerPlugin
{
    [KernelFunction]
    [Description("Analyzes and categorizes income sources.")]
    public string AnalyzeIncome(decimal salary, decimal investments, decimal businessIncome)
    {
        decimal totalIncome = salary + investments + businessIncome;
        return $"Your total income is {totalIncome:C}. Salary: {salary:C}, Investments: {investments:C}, Business: {businessIncome:C}.";
    }
}
