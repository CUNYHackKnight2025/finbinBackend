using System.ComponentModel.DataAnnotations;

namespace BudgetBackend.Models
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }

    public class VerifyResetTokenDTO
    {
        [Required]
        public required string Token { get; set; }
    }

    public class ResetPasswordDTO
    {
        [Required]
        public required string Token { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public required string NewPassword { get; set; }
        
        [Required]
        [Compare("NewPassword")]
        public required string ConfirmPassword { get; set; }
    }
}
