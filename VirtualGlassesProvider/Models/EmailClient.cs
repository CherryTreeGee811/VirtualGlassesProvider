using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;

namespace VirtualGlassesProvider.Models
{
    public class EmailClient
    {
        private MailjetClient client = new MailjetClient("192819178f7b59eb5b0854c116c4b5a0", "7b528a6bb6ca716fbdee15cadd0b86b4");
        public async void Send(string subject, string body, string emailTo)
        {
            var email = new TransactionalEmailBuilder().
                WithFrom(new SendContact("Harshal@vv.com", "Harshal From CVGS")).
                WithSubject(subject).
                WithHtmlPart("<p>" + body + "<p>").
                WithTo(new SendContact(emailTo)).
                Build();

            var response = await client.SendTransactionalEmailAsync(email);
            Console.WriteLine(response);
        }
    }
}
