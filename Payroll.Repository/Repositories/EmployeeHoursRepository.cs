using Payroll.Repository.Interface;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Repositories
{
    public class EmployeeHoursRepository : Repository<EmployeeHours>, IEmployeeHoursRepository
    {
        public EmployeeHoursRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeHours;
        }

    }
}
