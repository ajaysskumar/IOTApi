using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceTracker
{
    public class Host
    {
        public String Id { get; set; }
        public String HostName { get; set; }
        public DateTime LastStatusChecked { get; set; }
        public int Status { get; set; }
        public string LastAccessedIP { get; set; }
    }
}