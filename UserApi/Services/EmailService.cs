using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApi.Services
{
    public class EmailService
    {
        private IConfiguration _configuration;
        private string _userApiEmail = System.Environment.GetEnvironmentVariable("UserApiEmail");

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendConfirmationEmail(int userId, string userEmail, string activationCode)
        {
            var emailMessage = CreateEmailMessage(userId, userEmail, activationCode);

            using var client = new SmtpClient();
            try
            {
                client.Connect(_configuration.GetValue<string>("EmailSettings:SmtpServer"), _configuration.GetValue<int>("EmailSettings:Port"), true);
                client.AuthenticationMechanisms.Remove("XOUATH2");
                client.Authenticate(_userApiEmail, System.Environment.GetEnvironmentVariable("UserApiEmailPassword"));
                client.Send(emailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private MimeMessage CreateEmailMessage(int userId, string userEmail, string activationCode)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", _userApiEmail));
            emailMessage.To.Add(new MailboxAddress("", userEmail));
            emailMessage.Subject = "Confirmation Link";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"https://localhost:6001/activate/{userId}/{activationCode}"
            };
            return emailMessage;
        }
    }
}
