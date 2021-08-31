using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using System.Net.Mail;
using System.Net;

namespace kopilka
{
    public class EmailService
    {
        public void SendEmailPassword(string email, string message)
        {
            MailMessage emailMessage = new MailMessage();
            emailMessage.IsBodyHtml = true;
            emailMessage.From = new MailAddress("messagekopipika@gmail.com", "HiThere@Kopipika.com");
            emailMessage.To.Add(email);
            emailMessage.Subject = "Пароль от учетной записи koPIPIka";
            emailMessage.Body = message;
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("messagekopipika@gmail.com", "wtfwamble14");
                client.Port = 587;
                client.Send(emailMessage);
            }
        }
    }
}
