namespace AttendanceTracker.Portal.Migrations
{
    using AttendanceTracker.Portal.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AttendanceTracker.Portal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            Database.SetInitializer<ApplicationDbContext>(new Initializer());
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AttendanceTracker.Portal.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }

    public class Initializer : IDatabaseInitializer<ApplicationDbContext>
    {

        public void InitializeDatabase(ApplicationDbContext context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
                context.SaveChanges();
            }
        }

        private void Seed(ApplicationDbContext context)
        {
            throw new NotImplementedException();
        }
    }
}
