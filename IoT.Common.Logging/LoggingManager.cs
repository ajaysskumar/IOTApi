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

        public static void InitializeLogger(string instanceName, System.Diagnostics.Tracing.EventLevel logEventLevel)
        {
            var logListener = new ObservableEventListener();
            logListener.EnableEvents(IotAppEventSource.Log, logEventLevel, EventKeywords.None);

            logListener.LogToSqlDatabase(instanceName,
                "Data Source=6e7f98f6-ca3b-421b-b927-a65c00b12b90.sqlserver.sequelizer.com;Initial Catalog=db6e7f98f6ca3b421bb927a65c00b12b90;User Id=rmpcsyazqmfphpig;Password=tpJHMTKCiR2AbByiByrr2esHbVEr7ow85Zo5tso52g7beG4mefiZtcvp5bFYosE3;MultipleActiveResultSets=True;",
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
