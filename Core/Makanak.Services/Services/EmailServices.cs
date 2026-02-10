using Makanak.Abstraction.IServices;
using Makanak.Domain.Models.ResetPassword.SendMail;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Makanak.Services.Services
{
    public class EmailServices : IEmailService
    {
        private readonly MailSettings _emailSettings;
        public EmailServices(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 1. Configure Smtp Client
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = true
            };

            // 2 . Create Mail Message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };


            // Add Recipient
            mailMessage.To.Add(toEmail);

            // 3. Send (Async)
            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }
    }
}
