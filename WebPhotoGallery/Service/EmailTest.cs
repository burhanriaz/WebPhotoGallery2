using System.Net.Mail;
using System.Net;

namespace WebPhotoGallery.Service
{
    public static class EmailTest
    {
        public static void SendEmail(string recipientEmail,bool approved)
        {
            var senderEmail = "burhanriaz35@gmail.com";
            string messageSuccess= "Welcome to our online photo gallery! We're thrilled to have you here and can't wait to share our beautiful collection of photographs with you. Our gallery features a wide range of stunning images from around the world, captured by talented photographers who are passionate about their craft.\r\n\r\nTake your time browsing through our collection and don't hesitate to let us know if you have any questions or comments. We're always happy to hear from our visitors and appreciate your feedback.\r\n\r\nThank you for visiting our web photo gallery and we hope you enjoy your experience with us!";
            string messageDecline = "Thank you for your interest in creating an account with us. Unfortunately, we are unable to proceed with your application at this time. We apologize for any inconvenience and thank you for considering our services.\"";
            var message = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = approved? "Account approved on web photo gallery": "Account Declined on web photo gallery",
                Body = approved ? messageSuccess : messageDecline
            };

            message.To.Add(recipientEmail);
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
