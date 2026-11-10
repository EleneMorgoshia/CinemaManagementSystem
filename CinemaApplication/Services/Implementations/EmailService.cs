using CinemaApplication.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration; // ამის დახმარებით ვკითხულობ appsettingsს
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var sender = _configuration["Email:Sender"] ?? throw new InvalidOperationException("Email:Sender is missing.");
            var appPassword = _configuration["Email:AppPassword"] ?? throw new InvalidOperationException("Email:AppPassword is missing.");

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sender, appPassword),
                EnableSsl = true
            };

            using var message = new MailMessage(sender, to, subject, body);
            await smtpClient.SendMailAsync(message);
        }
    }
}
