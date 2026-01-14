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
        private readonly MailSettings _emailService;
        public EmailServices(IConfiguration configuration) 
        {
            _emailService = configuration.GetSection("EmailService").Get<MailSettings>();
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 1. Configure Smtp Client
            var client = new SmtpClient(_emailService.Host, _emailService.Port)
            {
                Credentials = new NetworkCredential(_emailService.Mail, _emailService.Password),
                EnableSsl = true
            };

            // 2 . Create Mail Message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailService.Mail, _emailService.DisplayName),
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
