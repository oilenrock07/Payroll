using Payroll.Entities.Contexts;
using Payroll.Entities.Seeder;

namespace Payroll.Entities.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PayrollContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        public void RunSeeds(PayrollContext context)
        {
            Seed(context);
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
            context.Settings.AddOrUpdate(s => s.SettingId, new SettingSeeds().GetDefaultSeeds().ToArray());
            context.Frequencies.AddOrUpdate(s => s.FrequencyId, new FrequencySeeds().GetDefaultSeeds().ToArray());
            context.Departments.AddOrUpdate(s => s.DepartmentId, new DepartmentSeeds().GetDefaultSeeds().ToArray());
            context.Holidays.AddOrUpdate(s => s.HolidayId, new HolidaySeeds().GetDefaultSeeds().ToArray());
            context.Leaves.AddOrUpdate(s => s.LeaveId, new LeaveSeeds().GetDefaultSeeds().ToArray());
            context.Taxes.AddOrUpdate(t => t.TaxId, new TaxSeeds().GetDefaultSeeds().ToArray());
            context.Deductions.AddOrUpdate(d => d.DeductionId, new DeductionSeeds().GetDefaultSeeds().ToArray());
            context.DeductionAmounts.AddOrUpdate(d => d.DeductionAmountId, new DeductionAmountSeeds().GetDefaultSeeds().ToArray());
            context.Users.AddOrUpdate(d => d.Id, new UserSeeds().GetDefaultSeeds().ToArray());
            context.Roles.AddOrUpdate(d => d.Id, new RoleSeeds().GetDefaultSeeds().ToArray());
            context.UserRoles.AddOrUpdate(d => d.UserRoleId, new UserRoleSeeds().GetDefaultSeeds().ToArray());
        }
    }
}
