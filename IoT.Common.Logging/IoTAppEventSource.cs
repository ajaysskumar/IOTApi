using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Logging
{
    [EventSource(Name = "IotAppEventSource")]
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

        private static Lazy<IotAppEventSource> Instance = new Lazy<IotAppEventSource>(() => new IotAppEventSource());

        public static IotAppEventSource Log { get { return Instance.Value; } }

        private IotAppEventSource() { }

        [Event(5, Message = "Error: {0}",
        Level = EventLevel.Critical, Keywords = Keywords.Diagnostic)]
        public void Error(String msgId, string message)
        {
            this.WriteEvent(5, msgId, message, DateTime.UtcNow);
        }

        [Event(1, Message = "Info: {0}",
        Level = EventLevel.Informational, 
        Task = Tasks.Page, Opcode = EventOpcode.Start)]
        public void Info(String msgId="", string message="", string methodName="", string Error="", string input="")
        {
            if (this.IsEnabled())
                this.WriteEvent(1, msgId, message, methodName,Error,input);
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
            if (this.IsEnabled()) this.WriteEvent(5, msgId, DateTime.UtcNow, message );
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
