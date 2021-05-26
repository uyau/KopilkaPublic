using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace kopilka
{
    public class EmailService
    {
        public void SendEmailPassword(string email, string message)
        {
          MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Kopipika", "HiThere@Kopipika.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Password from Kopipika";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using (SmtpClient client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("messagekopipika@gmail.com", "wtfwamble14");
                client.Send(emailMessage);
                client.Disconnect(true);
            }


        }
    }
}
