using System.Security.Cryptography;
using System.Text;
using BudgetBackend.Data;
using BudgetBackend.Models;
using BudgetBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthController(ApplicationDbContext context, TokenService tokenService, IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Email))
                return BadRequest("Email is already registered");

            // Create password hash and salt
            using var hmac = new HMACSHA512();

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _tokenService.CreateToken(user);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null)
                return Unauthorized("Invalid email");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Compare computed hash with stored hash
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }

            // Generate token
            var token = _tokenService.CreateToken(user);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token
            };
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Email == forgotPasswordDto.Email);

            // Always return OK even if the email doesn't exist (security best practice)
            if (user == null)
                return Ok(new { message = "If your email is registered, you will receive a password reset link." });

            // Generate random reset token
            string resetToken = GenerateResetToken();
            
            // Set expiration time (e.g., 24 hours from now)
            DateTime expirationTime = DateTime.UtcNow.AddHours(24);

            // Save token to the user record
            user.ResetToken = resetToken;
            user.ResetTokenExpires = expirationTime;
            await _context.SaveChangesAsync();

            // Send password reset email
            string resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?token={resetToken}";
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                "Reset Your FinBin Password",
                $"Please reset your password by clicking <a href='{resetUrl}'>here</a>. " +
                $"This link will expire in 24 hours."
            );

            return Ok(new { message = "If your email is registered, you will receive a password reset link." });
        }

        [HttpPost("verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenDTO verifyResetTokenDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.ResetToken == verifyResetTokenDto.Token);

            if (user == null || user.ResetTokenExpires <= DateTime.UtcNow)
                return BadRequest("Invalid or expired token");

            return Ok(new { valid = true });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.ResetToken == resetPasswordDto.Token);

            if (user == null || user.ResetTokenExpires <= DateTime.UtcNow)
                return BadRequest("Invalid or expired token");

            // Create new password hash
            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDto.NewPassword));
            user.PasswordSalt = hmac.Key;

            // Clear reset token fields
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully" });
        }

        private string GenerateResetToken()
        {
            // Generate a cryptographically secure random token
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }
    }
}
