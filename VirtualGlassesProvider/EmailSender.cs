using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;


public sealed class EmailSender : IEmailSender
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;


    public EmailSender(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUsername = smtpUsername;
        _smtpPassword = smtpPassword;
    }


    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient(_smtpServer))
        {
            client.Port = _smtpPort;
            client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            client.EnableSsl = true; // Enable SSL if your SMTP server requires it

            var message = new MailMessage
            {
                From = new MailAddress("thavraniharshal41@gmail.com", "Harshal From Vision Vogue Team"),
                ReplyTo = new MailAddress("thavraniharshal41@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            message.To.Add(email);

            await client.SendMailAsync(message);
        }
    }
}