using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoT.Common.Logging
{
    public static class LoggingManager
    {
        public static object ConfigurationManager { get; private set; }

        public static void InitializeLogger(string instanceName, EventLevel logEventLevel)
        {
            var logListener = new ObservableEventListener();
            logListener.EnableEvents(IotAppEventSource.Log, logEventLevel,Keywords.All);

            logListener.LogToSqlDatabase(instanceName,
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                "Traces",
                Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility.Buffering.DefaultBufferingInterval,
                1,
                Timeout.InfiniteTimeSpan,
                500);
            //IotAppEventSource.Log.IsEnabled()
            IotAppEventSource.Log.Info("NA","Logger initialized");

        }
    }
}
