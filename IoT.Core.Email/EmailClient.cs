using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace IoT.Core.Email
{
    public class EmailClient
    {
        public void SendEmail(decimal currentTemperature)
        {


            var fromAddress = new MailAddress("ajay.a338@gmail.com", "Ajay kumar");
            var toAddress = new MailAddress("kingajay007@gmail.com", "Ajay kumar");
            string fromPassword = Encoding.UTF8.GetString(Convert.FromBase64String("Z2l2ZSBtZSAkIGFnYWlu")); ;
            string subject = "Test email";
            string body = String.Format("Current teperature is {0}",currentTemperature) ;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            ServicePointManager.ServerCertificateValidationCallback =
    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    { return true; };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
