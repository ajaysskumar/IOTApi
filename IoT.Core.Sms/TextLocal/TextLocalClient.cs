using System;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;
using RestSharp.Extensions.MonoHttp;
using IoT.Core.Sms;
using System.Configuration;

namespace IoT.Core.Sms.TextLocal
{
    public class TextLocalSMSClient
    {
        private string username;
        private string apiHash;
        private List<TextModel> Sms;
        private string sendor;
        private string apiUrl;
        private bool isTest;

        public TextLocalSMSClient(string username, string apiHash,
             string apiUrl, string sendor="OnActuate", bool isTest=false)
        {
            this.username = username;
            this.apiHash = apiHash;
            this.sendor = sendor;
            this.apiUrl = apiUrl;
            this.isTest = isTest;
        }
        public string SendSMS(string number, string message)
        {
            String messageToSend = HttpUtility.UrlEncode("This is your message");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues(apiUrl, new NameValueCollection()
                {
                {"username" , username},
                {"hash" , apiHash},
                {"numbers" , number},
                {"message" , messageToSend},
                {"sender" , sendor},
                //{"test" , isTest.ToString()}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        public IEnumerable<String> SendBulkSMS(List<TextModel> textModels) {

            List<String> results = new List<string>();

            if (textModels==null)
            {
                throw new Exception("No numbers defined");
            }
            try
            {
                foreach (var textItem in textModels)
                {
                    string result = SendSMS(textItem.MobileNumber,textItem.Message);
                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}