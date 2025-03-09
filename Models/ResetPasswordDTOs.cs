using System.ComponentModel.DataAnnotations;

namespace BudgetBackend.Models
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class VerifyResetTokenDTO
    {
        [Required]
        public string Token { get; set; }
    }

    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
        
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
