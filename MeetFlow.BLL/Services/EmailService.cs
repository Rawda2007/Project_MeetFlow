using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow.BLL.Services
{
    // Sends emails over plain SMTP using System.Net.Mail (no extra NuGet package needed).
    // All the sender-specific values are read from appsettings.json under "EmailSettings" —
    // fill those in with your own mailbox details, nothing is hardcoded here.
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendPasswordResetCodeAsync(string toEmail, string recipientName, string code)
        {
            var host = _config["EmailSettings:SmtpHost"];
            var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];
            var senderName = _config["EmailSettings:SenderName"] ?? "MeetFlow";
            var enableSsl = bool.Parse(_config["EmailSettings:EnableSsl"] ?? "true");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(senderEmail))
            {
                // Email isn't configured yet — fail loudly in dev instead of silently pretending it sent.
                throw new InvalidOperationException(
                    "Email settings are missing. Fill in EmailSettings in appsettings.json (SmtpHost, SenderEmail, SenderPassword).");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "MeetFlow — Password Reset Code",
                Body = $"Hi {recipientName},\n\n" +
                       $"Your password reset code is: {code}\n" +
                       $"This code expires in 15 minutes.\n\n" +
                       $"If you didn't request this, you can safely ignore this email.",
                IsBodyHtml = false
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl
            };

            await client.SendMailAsync(message);
        }
    }
}
