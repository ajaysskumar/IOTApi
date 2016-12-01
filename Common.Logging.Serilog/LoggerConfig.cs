using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public static class LoggerConfig
    {
        public static void InitializeLogger(string connectionString)
        {
            var log = new LoggerConfiguration().WriteTo.MSSqlServer(connectionString, "Logs");
        }

        public static void WriteLog()
        {
            
        }
    }
}
