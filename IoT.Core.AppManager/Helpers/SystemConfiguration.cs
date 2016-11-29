using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Core.AppManager.Helpers
{
    public static class SystemConfiguration
    {
        public static String MqttServerAddress { get { return "TCP://m13.cloudmqtt.com:19334"; } }
        public static int MqttServerPort { get { return 19334; } }
        public static String MqttServerUserName { get { return "cbaeasea"; } }
        public static String MqttServerPassword { get {return "KiYFQP0Q1gbe"; } }
        
    }
}
