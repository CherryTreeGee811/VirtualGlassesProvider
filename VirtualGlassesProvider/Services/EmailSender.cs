using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;


namespace VirtualGlassesProvider.Services
{
    public sealed class EmailSender(
        string smtpServer, 
        int smtpPort, 
        string smtpUsername, 
        string smtpPassword) : IEmailSender
    {
        private readonly string _smtpServer = smtpServer;
        private readonly int _smtpPort = smtpPort;
        private readonly string _smtpUsername = smtpUsername;
        private readonly string _smtpPassword = smtpPassword;


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress("thavraniharshal41@gmail.com", "Harshal From Vision Vogue Team"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            message.ReplyToList.Add(new MailAddress("thavraniharshal41@gmail.com"));
            message.To.Add(email);

            await client.SendMailAsync(message);
        }
    }
}