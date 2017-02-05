using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IoTSensorPolling
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            IoT.Common.Logging.LoggingManager.InitializeLogger("IoTEventSourceManager", System.Diagnostics.Tracing.EventLevel.LogAlways,"WindowsServiceLog");

            IoT.Common.Logging.IoTEventSourceManager.Log.Debug("Service in Main", "WindowsServiceLog");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new IoTPollService(),
                new IoTSensorSubscriptionService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
