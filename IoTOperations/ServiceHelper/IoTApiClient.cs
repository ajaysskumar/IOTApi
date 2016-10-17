using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using IoTOperations.Sensors;
using System.Xml.Serialization;
using System.IO;
using IoT.Common.Model.Utility;
using Newtonsoft.Json;

namespace IoTOperations.ServiceHelper
{
    public class IoTApiClient 
    {
        private HttpClient httpClient;
        private string baseUrl;

        public IoTApiClient(string baseUri)
        {
            httpClient = new HttpClient();
            baseUrl = baseUri;
        }

        public async Task<List<Sensors.Relay>> GetRelayStatus(int relayGroupId=0,string relativeUrl="/")
        {
            var parameters = new Dictionary<string, string>();
            //parameters["text"] = text;
            var response = await httpClient.PostAsync(baseUrl, new FormUrlEncodedContent(parameters));
            var contents = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Relays));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
                Relays resultingMessage = (Relays)serializer.Deserialize(memStream);
                return resultingMessage.Relay;
                //Items = JsonConvert.DeserializeObject<List<TodoItem>>(content);
            }

            else throw new Exception("Request Failed");
        }

        public async void ToggleSwitch(int relayGroupId = 0, string SwtichId="Socket")
        {

            List<Sensors.Relay> relayList = GetRelayStatus().Result;

            Sensors.Relay relay = relayList.Where(x => x.RelayName == "Relay1").FirstOrDefault();

            if (relay.RelayStatus == "CKT_OPEN")
            {
                SwtichId = "socket1Off";
            }
            else
            {
                SwtichId = "socket1On";
            }

            var parameters = new Dictionary<string, string>();
            
            var response = await httpClient.PostAsync(string.Format("{0}/{1}",baseUrl,SwtichId), new FormUrlEncodedContent(parameters));
           
        }

        public async Task<RequestModel> GetRequestToProcess(string url)
        {
            var parameters = new Dictionary<string, string>();
            //parameters["text"] = text;
            var response = await httpClient.GetAsync(string.Format("{0}{1}",baseUrl,url));
            var contents = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                //XmlSerializer serializer = new XmlSerializer(typeof(RequestModel));
                //MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
                //RequestModel resultingMessage = (RequestModel)serializer.Deserialize(memStream);

                var resultingMessage = JsonConvert.DeserializeObject<RequestModel>(contents);
                
                return resultingMessage;
                //Items = JsonConvert.DeserializeObject<List<TodoItem>>(content);
            }

            else throw new Exception("Request Failed");
        }

    }
}
