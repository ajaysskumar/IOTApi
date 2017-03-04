using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AttendanceTracker.Portal.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() :base("DefaultConnection")
        {

        }

        public virtual DbSet<Host> Host { get; set; }
        public virtual DbSet<HostStatus> HostStatus { get; set; }
    }
}