using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Mailjet.Client.TransactionalEmails;

public class EmailSender : IEmailSender
{
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string smtpUsername;
    private readonly string smtpPassword;

    public EmailSender(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
    {
        this.smtpServer = smtpServer;
        this.smtpPort = smtpPort;
        this.smtpUsername = smtpUsername;
        this.smtpPassword = smtpPassword;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient(smtpServer))
        {
            client.Port = smtpPort;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
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