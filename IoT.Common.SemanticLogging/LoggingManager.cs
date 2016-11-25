using System.Configuration;
using System.Diagnostics.Tracing;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;

namespace IoT.Common.Logging
{
    public static class LoggingManager
    {
        public static void InitializeLogger(string instanceName, System.Diagnostics.Tracing.EventLevel logEventLevel)
        {
            var logListener = new ObservableEventListener();
            logListener.EnableEvents(IoTEventSourceManager.Log, logEventLevel, EventKeywords.None);

            logListener.LogToSqlDatabase(instanceName,
               ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                "Traces",
                Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility.Buffering.DefaultBufferingInterval,
                1,
                Timeout.InfiniteTimeSpan,
                500);
           
            IoTEventSourceManager.Log.Info(string.Format("Logger Initialized - Event Source : {0}, Log Level : {1}", instanceName, logEventLevel.ToString()));
        }
    }
}
