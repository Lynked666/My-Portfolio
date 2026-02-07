using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.PawsSevices;
using System.Net.Mail;

namespace PAWS_NDV_PetLovers.Controllers.Login
{
  
    public class LoginsController : Controller
    {

        private readonly PAWS_NDV_PetLoversContext _context;
        private readonly EmailService _emailService; // Inject the EmailService

        public LoginsController(PAWS_NDV_PetLoversContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> Login()
        {
            await UserAccount();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? userName, string? passWord)
        {
  
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(passWord))
            {
                ModelState.AddModelError("", "Please enter your username and password.");
                return View();
            }

            var hashPassword = HashingService.HashData(passWord);
            var userAccount = await _context.UserAccounts
                                    .FirstOrDefaultAsync(a => a.userName == userName && a.passWord == hashPassword);

            if (userAccount != null)
            {
                // Set authentication session and role
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserName", userName); //optional for dashboar display
                HttpContext.Session.SetString("UserRole", userAccount.userType ?? ""); // Store user role
                HttpContext.Session.SetInt32("UserId", userAccount.acc_Id);

                HttpContext.Session.SetString("fullName", $"{userAccount.fname} {userAccount.lname}");

                if (!userAccount.IsActive)
                {
                    ModelState.AddModelError("", "Account has been deactivated");
                    return View();
                }

                // Check if the password has been changed
                if (!userAccount.IsPasswordChanged)
                {
                    HttpContext.Session.SetString("PasswordChanged", "false");
                    return RedirectToAction("SignUp", "UserAccounts");
                }
                else
                {
                    HttpContext.Session.SetString("PasswordChanged", "true");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return View();
            }
        }


        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login", "Logins");

        }


        //funtion auto craete Admin
        public async Task UserAccount()
        {
            var userAccount = await _context.UserAccounts.ToListAsync
                ();


            var adminCheck = userAccount.Any(a => a.userType == "Admin");

            if (!userAccount.Any() || !adminCheck)
            {
                const string username = "admin";
                string password = "admin";

                string HashPassword = HashingService.HashData(password);
                password = HashPassword;

                var adminEntry = new UserAccount
                {
                    userName = username,
                    passWord = password,
                    userType = "Admin",
                    IsActive = true,
                    dateCreated = DateTime.Now
                };

                _context.UserAccounts.Add(adminEntry);

                await _context.SaveChangesAsync();
            }
         
        }
        // Forgot Password GET action
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot Password POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Find the user by email
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with that email address.");
                return View();
            }

            // Generate a reset code and set expiration time
            var resetCode = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15);

            // Save reset request in the PasswordResetRequest table
            var passwordResetRequest = new PasswordResetRequest
            {
                ResetCode = resetCode,
                Expiry = expiry,
                UserAccountId = user.acc_Id
            };
            _context.PasswordResetRequests.Add(passwordResetRequest);
            await _context.SaveChangesAsync();

            // Try to send the reset code to the user's email
            try
            {
                await _emailService.SendResetCodeAsync(email, resetCode);
                ViewBag.Message = "A password reset link has been sent to your email.";
                return View("ForgotPasswordConfirmation"); // Confirmation view
            }
            catch (Exception ex) when (ex is SmtpException || ex is HttpRequestException)
            {
                // Notify the user
                ModelState.AddModelError("", "Please check your internet connection and try again.");
                return View();
            }
        }

        // Forgot Password Confirmation GET action - shows confirmation message
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: Enter Reset Code
        [HttpGet]
        public IActionResult EnterResetCode()
        {
            return View();
        }

        // POST: Verify Reset Code and Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResetCode(string resetCode, string newPassword)
        {
            // Find the reset request with the provided code that hasn't expired
            var resetRequest = await _context.PasswordResetRequests
                .Include(r => r.UserAccount)
                .FirstOrDefaultAsync(r => r.ResetCode == resetCode && r.Expiry > DateTime.UtcNow);

            if (resetRequest == null)
            {
                ModelState.AddModelError("", "Invalid or expired reset code.");
                return View("EnterResetCode");
            }

            // Update the user's password and clear all reset requests for that user
            var user = resetRequest.UserAccount;

            // Hash the new password (hashing implementation should be in your service layer)
            user.passWord = HashingService.HashData(newPassword);

            // Remove any existing reset requests for this user to clear data
            var existingRequests = _context.PasswordResetRequests.Where(r => r.UserAccountId == user.acc_Id);
            _context.PasswordResetRequests.RemoveRange(existingRequests);

            // Save changes
            await _context.SaveChangesAsync();

            // Redirect to the Password Reset Success view
            return RedirectToAction("PasswordResetSuccess");
        }

        // GET: Password Reset Success
        [HttpGet]
        public IActionResult PasswordResetSuccess()
        {
            return View();
        }
    }
}

