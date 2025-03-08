/*
We will Define our budget class here
Its gonna have category, cost, optional date???
Define how our AI analytics agent will process this
*/

namespace BudgetBackend.Models;

public class BudgetRecord
{
    public int Id { get; set; }
    public string Category { get; set; } = "";
    public decimal Cost { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}