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
                Message = "{0}",
                Level = EventLevel.LogAlways,
                Task = Tasks.EventIoT)]
        public void Info(string msgId="",string message="")
        {
            if (this.IsEnabled())
            {
                WriteEvent(1,msgId, message);
            }
        }

        [Event(5,
                Message = "Error: {0}",
                Level = EventLevel.Error,
                Task = Tasks.EventIoT)]
        public void Error(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(5, message, msgId, workflowRunId, trackingId, step, module);
            }
        }

        [Event(4,
                Message = "Debug: {0}",
                Level = EventLevel.Verbose,
                Task = Tasks.EventIoT)]
        public void Debug(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(4, message, msgId, workflowRunId, trackingId, step, module);
            }
        }

        [Event(3,
                Message = "Warn: {0}",
                Level = EventLevel.Warning,
                Task = Tasks.EventIoT)]
        public void Warn(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(3, message, msgId, workflowRunId, trackingId, step, module);
            }
        }
    }
}
