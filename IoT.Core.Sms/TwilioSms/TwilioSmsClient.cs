using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;

namespace IoT.Core.Sms.TwilioSms
{
    public class TwilioSmsClient
    {
        private string accountSid;
        private string authToken;
        public TwilioSmsClient(string accountSid, string authToken)
        {
            this.accountSid = accountSid;
            this.authToken = authToken;
        }

        public void SendSMS(string smsNumber, string smsBody)
        {

            //TwilioClient client = new TwilioClient();

            try
            {
                var twilio = new TwilioRestClient(accountSid, authToken);
                

                var message = twilio.SendMessage("+13475073559", smsNumber, smsBody, new string[] { }, statusCallback: null);
                Console.WriteLine(message.Sid);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
