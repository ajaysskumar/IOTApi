using System;
using System.Diagnostics.Tracing;

namespace IoT.Common.Logging
{
    /// <summary>
    /// Event Source for Logic apps
    /// </summary>
    [EventSource(Name = "IoTEventSourceManager")]
    public class IoTEventSourceManager : EventSource
    {
        private static class Tasks
        {
            public const EventTask EventIoT = (EventTask)1;
        }

        private static Lazy<IoTEventSourceManager> Instance = new Lazy<IoTEventSourceManager>(() => new IoTEventSourceManager());

        public static IoTEventSourceManager Log { get { return Instance.Value; } }

        [Event(1,
                Message = "{1}",
                Level = EventLevel.Critical,
                Task = Tasks.EventIoT)]
        public void Fatal(string message="",string msgId="")
        {
            if (this.IsEnabled())
            {
                WriteEvent(1,message, msgId);
            }
        }

        [Event(2,
                Message = "{1}",
                Level = EventLevel.Error,
                Task = Tasks.EventIoT)]
        public void Error(string message = "", string msgId = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(2, message, msgId);
            }
        }

        [Event(3,
                Message = "{1}",
                Level = EventLevel.Warning,
                Task = Tasks.EventIoT)]
        public void Warn(string message = "", string msgId = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(3, message, msgId);
            }
        }

        [Event(4,
                Message = "{1}",
                Level = EventLevel.Informational,
                Task = Tasks.EventIoT)]
        public void Info(string message = "", string msgId = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(4, message, msgId);
            }
        }

        [Event(5,
               Message = "{1}",
               Level = EventLevel.Verbose,
               Task = Tasks.EventIoT)]
        public void Debug(string message = "", string msgId = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(5, message, msgId);
            }
        }

        [Event(6,
               Message = "{1}",
               Level = EventLevel.LogAlways,
               Task = Tasks.EventIoT)]
        public void Trace(string message = "", string msgId = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(6, message, msgId);
            }
        }
    }
}
