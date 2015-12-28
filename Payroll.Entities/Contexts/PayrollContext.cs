using System.Data.Entity;
using Payroll.Entities.Users;

namespace Payroll.Entities.Contexts
{
    public class PayrollContext : DbContext
    {
        public PayrollContext()
            : base("Payroll.ConnectionString")
        {
            Database.SetInitializer<PayrollContext>(null);
        }

        public DbSet<User> Users { get; set; }
    }
}
