using System.Configuration;
using System.Data.Entity;
using Payroll.Entities.Users;

namespace Payroll.Entities.Contexts
{
    public class PayrollContext : DbContext
    {
        public PayrollContext()
            : base(ConnectionString)
        {
            Database.SetInitializer<PayrollContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        static string ConnectionString
        {

            get
            {
                string cs = "";
                switch (ConfigurationManager.AppSettings["DatabaseType"])
                {
                    case "MsSql":
                        cs = "ConnectionString.MsSql";
                        break;
                    case "MySql":
                        cs = "ConnectionString.MySql";
                        break;
                }

                return cs;
            }
        }
    }
}
