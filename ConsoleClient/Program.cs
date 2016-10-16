using IoTOperations.Sensors;
using IoTOperations.ServiceHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://192.168.100.186";

            IoTApiClient client = new IoTApiClient(url);

            List<Relay> relays = client.GetRelayStatus().Result;
            client.ToggleSwitch();
        }
    }
}
