using System;
using System.Diagnostics.Tracing;

namespace OnActuate.Iot.Core.Logging
{
    /// <summary>
    /// Event Source for Logic apps
    /// </summary>
    [EventSource(Name = "IotEventSourceManager")]
    public class IotDemoEventSourceManager : EventSource
    {
        private static class Tasks
        {
            public const EventTask EventIot = (EventTask)1;
        }

        private static Lazy<IotDemoEventSourceManager> Instance = new Lazy<IotDemoEventSourceManager>(() => new IotDemoEventSourceManager());

        public static IotDemoEventSourceManager Log { get { return Instance.Value; } }

        [Event(1,
                Message = "Info: {0}",
                Level = EventLevel.LogAlways,
                Task = Tasks.EventIot)]
        public void Info(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(1, message, msgId, workflowRunId, trackingId, step, module);
            }
        }

        [Event(5,
                Message = "Error: {0}",
                Level = EventLevel.Error,
                Task = Tasks.EventIot)]
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
                Task = Tasks.EventIot)]
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
                Task = Tasks.EventIot)]
        public void Warn(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(3, message, msgId, workflowRunId, trackingId, step, module);
            }
        }
    }
}
