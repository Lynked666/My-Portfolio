using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.PawsSevices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PAWS_NDV_PetLovers.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;
        private readonly EmailService _emailService;

        public AccountController(PAWS_NDV_PetLoversContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
                return BadRequest("User with the provided email does not exist.");

            var resetCode = new Random().Next(100000, 999999).ToString(); // Generate a 6-digit code
            var expiry = DateTime.UtcNow.AddMinutes(15); // Code expires in 15 minutes

            var passwordResetRequest = new PasswordResetRequest
            {
                ResetCode = resetCode,
                Expiry = expiry,
                UserAccountId = user.acc_Id
            };

            // Save the reset request to the database
            _context.PasswordResetRequests.Add(passwordResetRequest);
            await _context.SaveChangesAsync();

            await _emailService.SendResetCodeAsync(email, resetCode);

            return Ok("Password reset code has been sent to your email.");
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode(string email, string resetCode)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
                return BadRequest("User with the provided email does not exist.");

            var resetRequest = await _context.PasswordResetRequests
                .FirstOrDefaultAsync(r => r.UserAccountId == user.acc_Id && r.ResetCode == resetCode);

            if (resetRequest == null || resetRequest.Expiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired reset code.");

            return Ok("Code verified. You may now reset your password.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
                return BadRequest("User with the provided email does not exist.");

            // Hash password in production (plain text for demonstration)
            user.passWord = newPassword;

            // Delete all reset requests for this user after successful reset
            var resetRequests = _context.PasswordResetRequests.Where(r => r.UserAccountId == user.acc_Id);
            _context.PasswordResetRequests.RemoveRange(resetRequests);

            await _context.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }
    }
}
