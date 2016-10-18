using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Logging
{
    [EventSource(Name = "IoTEventSource")]
    public class IotAppEventSource : EventSource
    {
        public class Keywords
        {
            public const EventKeywords Page = (EventKeywords)1;
            public const EventKeywords DataBase = (EventKeywords)2;
            public const EventKeywords Diagnostic = (EventKeywords)4;
            public const EventKeywords Perf = (EventKeywords)8;
        }

        public class Tasks
        {
            public const EventTask Page = (EventTask)1;
            public const EventTask DBQuery = (EventTask)2;
        }

        private static IotAppEventSource _log = new IotAppEventSource();
        private IotAppEventSource() { }
        public static IotAppEventSource Log { get { return _log; } }

        [Event(1, Message = "Error: {0}",
        Level = EventLevel.Critical, Keywords = Keywords.Diagnostic)]
        public void Error(String msgId, string message)
        {
            this.WriteEvent(1, msgId, message, DateTime.UtcNow);
        }

        [Event(2, Message = "Info: {0}", Keywords = Keywords.Perf,
        Level = EventLevel.Informational)]
        public void Info(String msgId, string message)
        {
            this.WriteEvent(2, msgId, message, DateTime.UtcNow);
        }

        [Event(3, Message = "Warning: {0}",
        Opcode = EventOpcode.Start,
        Task = Tasks.Page, Keywords = Keywords.Page,
        Level = EventLevel.Warning)]
        public void Warn(String msgId, string message)
        {
            if (this.IsEnabled()) this.WriteEvent(3, msgId, message, DateTime.UtcNow);
        }

        [Event(4, Message = "Critical: {0}",
        Opcode = EventOpcode.Start,
        Task = Tasks.Page, Keywords = Keywords.Page,
        Level = EventLevel.Critical)]
        public void Critical(String msgId, string message)
        {
            if (this.IsEnabled()) this.WriteEvent(4, msgId, message, DateTime.UtcNow);
        }

        [Event(5, Message = "Verbose: {0}",
        Opcode = EventOpcode.Start,
        Task = Tasks.Page, Keywords = Keywords.Page,
        Level = EventLevel.Verbose)]
        public void Verbose(String msgId, string message)
        {
            if (this.IsEnabled()) this.WriteEvent(5, msgId, message, DateTime.UtcNow);
        }

        [Event(0, Message = "Debug: {0}",
        Opcode = EventOpcode.Start,
        Task = Tasks.Page, Keywords = Keywords.Page,
        Level = EventLevel.LogAlways)]
        public void Debug(String msgId,string message)
        {
            if (this.IsEnabled()) this.WriteEvent(0, msgId, message,DateTime.UtcNow);
        }

    }
}
