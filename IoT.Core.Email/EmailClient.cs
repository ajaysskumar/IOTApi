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
        public void SendEmail(decimal currentTemperature, decimal currentHumidity, int timeInMinutes,string emailAddress,string Name)
        {


            var fromAddress = new MailAddress("ajay.a338@gmail.com", "Ajay kumar");
            var toAddress = new MailAddress(emailAddress, Name);
            string fromPassword = Encoding.UTF8.GetString(Convert.FromBase64String("Z2l2ZSBtZSAkIGFnYWlu")); ;
            string subject = "Temperature Alert";
            string body = String.Format("Hello {0}, \nCurrent teperature is {1} degree celcius and current humidity is {2} percent \nData recorded for last {3} minutes",Name,currentTemperature,currentHumidity,timeInMinutes) ;

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
