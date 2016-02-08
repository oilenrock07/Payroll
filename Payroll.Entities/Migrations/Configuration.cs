using Payroll.Entities.Contexts;

namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PayrollContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(PayrollContext context)
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
            context.Settings.AddOrUpdate(
                    s => s.SettingId,
                        new Setting { SettingId = 1, SettingKey = "SCHEDULE_NIGHTDIF_TIME_START", Value= "10:00:00 PM", Description="Night Differential Start Time", Category= "SCHEDULE" },
                        new Setting { SettingId = 2, SettingKey = "SCHEDULE_NIGHTDIF_TIME_END", Value = "7:59:00 AM", Description = "Night Differential End Time", Category = "SCHEDULE" },
                        new Setting { SettingId = 3, SettingKey = "RATE_OT", Value = "1.25", Description = "OT Rate", Category = "RATE" },
                        new Setting { SettingId = 4, SettingKey = "RATE_NIGHTDIF", Value = "0.8", Description = "Night Dif", Category = "RATE" },
                        new Setting { SettingId = 5, SettingKey = "RATE_REST_DAY", Value = "1.3", Description = "Rest Day", Category = "RATE" },
                        new Setting { SettingId = 6, SettingKey = "RATE_HOLIDAY_SPECIAL", Value = "1.3", Description = "Special Holiday", Category = "RATE" },
                        new Setting { SettingId = 6, SettingKey = "RATE_HOLIDAY_REGULAR", Value = "2", Description = "Regular Holiday", Category = "RATE" }
             );

        }
    }
}
