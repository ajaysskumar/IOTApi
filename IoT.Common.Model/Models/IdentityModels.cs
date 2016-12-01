using IoT.Common.Model.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Model.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<MotionSensor> MotionsSensor { get; set; }

        public virtual DbSet<WifiSensor> WifiSensor { get; set; }

        //public virtual DbSet<Trace> Trace { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<RelayGroup> RelayGroup { get; set; }
        public virtual DbSet<Relay> Relay { get; set; }
        public virtual DbSet<RequestLog> RequestLog { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<IoT.Common.Model.Models.SystemConfiguration> SystemConfiguration { get; set; }
    }
}
