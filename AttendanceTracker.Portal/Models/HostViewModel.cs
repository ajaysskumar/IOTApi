using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AttendanceTracker.Portal.Models
{
    public class HostViewModel
    {
        public String Id { get; set; }

        [Display(Name = "Host Name")]
        public String HostName { get; set; }

        [NotMapped]
        [Display(Name = "Last Time Connected")]
        public DateTime LastStatusChecked { get; set; }

        [NotMapped]
        [Display(Name = "Last Status")]
        public int Status { get; set; }

        [NotMapped]
        [Display(Name = "IP Last Accessed")]
        public string LastAccessedIP { get; set; }

        [Display(Name = "Employee Name")]
        public String EmployeeName { get; set; }

        [Display(Name = "Online Hours(Current Day)")]
        public int TotalConnectedHoursToday { get; set; }
        [NotMapped]
        [Display(Name = "Login Time Today")]
        public string CurrentDayLoginTime { get; set; }

        public virtual ICollection<HostStatus> HostStatus { get; set; }
    }
}