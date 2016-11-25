using System;
using System.Diagnostics.Tracing;

namespace Trafigura.ePuma.Core.Logging
{
    /// <summary>
    /// Event Source for Logic apps
    /// </summary>
    [EventSource(Name = "ePumaEventSourceManager")]
    public class EpumaEventSourceManager : EventSource
    {
        private static class Tasks
        {
            public const EventTask EventePuma = (EventTask)1;
        }

        private static Lazy<EpumaEventSourceManager> Instance = new Lazy<EpumaEventSourceManager>(() => new EpumaEventSourceManager());

        public static EpumaEventSourceManager Log { get { return Instance.Value; } }

        [Event(1,
                Message = "Info: {0}",
                Level = EventLevel.LogAlways,
                Task = Tasks.EventePuma)]
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
                Task = Tasks.EventePuma)]
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
                Task = Tasks.EventePuma)]
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
                Task = Tasks.EventePuma)]
        public void Warn(string message, string msgId = "", string workflowRunId = "", string trackingId = "", string step = "", string module = "")
        {
            if (this.IsEnabled())
            {
                WriteEvent(3, message, msgId, workflowRunId, trackingId, step, module);
            }
        }
    }
}
