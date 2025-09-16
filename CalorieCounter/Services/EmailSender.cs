namespace CalorieCounter.Services
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using MailKit.Security;

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config) => _config = config;

       
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DineWithFriends", _config["Email:Sender"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            System.Diagnostics.Debug.WriteLine("Sender: " + _config["Email:Sender"]);
            System.Diagnostics.Debug.WriteLine("Host: " + _config["Email:SmtpHost"]);
            System.Diagnostics.Debug.WriteLine("Port: " + _config["Email:SmtpPort"]);
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["Email:Sender"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
