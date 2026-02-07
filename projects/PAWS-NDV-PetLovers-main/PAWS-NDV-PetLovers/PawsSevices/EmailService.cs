using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PAWS_NDV_PetLovers.PawsSevices
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendResetCodeAsync(string email, string resetCode)
        {
            using var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"], int.Parse(_configuration["EmailSettings:SMTPPort"]))
            {
                Credentials = new NetworkCredential(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
                Subject = "Password Reset Code",
                Body = $"Your password reset code is {resetCode}. This code will expire in 15 minutes.",
                IsBodyHtml = true,
            };
            message.To.Add(email);

            await smtpClient.SendMailAsync(message);
        }
    } 
}
