using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
