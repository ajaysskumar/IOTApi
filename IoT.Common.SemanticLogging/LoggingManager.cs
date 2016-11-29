using System.Configuration;
using System.Diagnostics.Tracing;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

namespace IoT.Common.Logging
{
    public static class LoggingManager
    {
        public static void InitializeLogger(string instanceName, System.Diagnostics.Tracing.EventLevel logEventLevel)
        {
            var logListener1 = new ObservableEventListener();
            logListener1.EnableEvents(IoTEventSourceManager.Log, logEventLevel, EventKeywords.None);

            logListener1.LogToSqlDatabase(instanceName,
               ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                "Traces",
                Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility.Buffering.DefaultBufferingInterval,
                1,
                Timeout.InfiniteTimeSpan,
                500);

            var logListener2 = new ObservableEventListener();
            logListener2.EnableEvents(IoTEventSourceManager.Log, logEventLevel, EventKeywords.None);

            logListener2.LogToFlatFile("semanticLogs.json", new JsonEventTextFormatter(EventTextFormatting.Indented), true);

            IoTEventSourceManager.Log.Info(string.Format("Logger Initialized - Event Source : {0}, Log Level : {1}", instanceName, logEventLevel.ToString()));
        }
    }
}
