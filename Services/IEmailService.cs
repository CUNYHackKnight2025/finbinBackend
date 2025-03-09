namespace BudgetBackend.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string subject, string message);
    }
}
