using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceTracker.Portal.Models
{
    public class HostStatus
    {
        public int Id { get; set; }
        public string HostId { get; set; }
        [Display(Name = "Status Checked At")]
        public DateTime LastStatusChecked { get; set; }

        [Display(Name = "Last Status")]
        public int Status { get; set; }

        [Display(Name = "IP Last Accessed")]
        public string LastAccessedIP { get; set; }

        public virtual Host Host { get; set; }
    }
}