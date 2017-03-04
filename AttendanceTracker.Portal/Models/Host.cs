using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AttendanceTracker.Portal.Models
{
    public class Host
    {
        public String Id { get; set; }

        [Display(Name = "Host Name")]
        public String HostName { get; set; }

        [NotMapped]
        [Display(Name = "Last Connected Time")]
        public DateTime LastStatusChecked { get; set; }

        [NotMapped]
        [Display(Name = "Last Status")]
        public int Status { get; set; }

        [NotMapped]
        [Display(Name = "IP Last Accessed")]
        public string LastAccessedIP { get; set; }

        //[NotMapped]
        [Display(Name = "Employee Name")]
        public String EmployeeName { get; set; }
        

        public virtual ICollection<HostStatus> HostStatus { get; set; }
    }
}