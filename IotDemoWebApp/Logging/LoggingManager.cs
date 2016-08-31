using System.Configuration;
using System.Diagnostics.Tracing;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;

namespace OnActuate.Iot.Core.Logging
{
    public static class LoggingManager
    {
        public static void InitializeLogger(string instanceName, System.Diagnostics.Tracing.EventLevel logEventLevel)
        {
            var logListener = new ObservableEventListener();
            logListener.EnableEvents(IotDemoEventSourceManager.Log, logEventLevel, EventKeywords.None);

            logListener.LogToSqlDatabase(instanceName,
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                "dbo.Traces",
                Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility.Buffering.DefaultBufferingInterval,
                1,
                Timeout.InfiniteTimeSpan,
                500);
           
            IotDemoEventSourceManager.Log.Info(string.Format("Logger Initialized - Event Source : {0}, Log Level : {1}", instanceName, logEventLevel.ToString()));
        }
    }
}
