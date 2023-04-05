    using System.IO;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Extensions.Hosting.Internal;
    using Microsoft.VisualBasic;
namespace WebPhotoGallery.Service
{
    public class EmailService : IEmailService
    {
        public string GetEmailHtmlTemplateBody(string templateName)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(Email email)
        {
            await Task.Run(() =>
            {
                SendEmail(email);
            });
        }

       

        private void SendEmail(Email email)
        {
            var message = new MailMessage
            {
                //From = new MailAddress(email.From, email.DisplayName),
                From = new MailAddress("burhanriaz35@gmail.com", email.DisplayName),
                Subject = email.Subject,
                Body = email.Content,
                IsBodyHtml = email.IsContentHtml
            };

            foreach (var receiver in email.To)
            {
                message.To.Add(new MailAddress(receiver));
            }

      

            //using (var smtp = new SmtpClient("localhost"))
            using (var smtp = new SmtpClient())
            {

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(message.From.Address, "dsscuuaewqlwrkyt");

                smtp.Send(message);
            }
        }
    }
}
