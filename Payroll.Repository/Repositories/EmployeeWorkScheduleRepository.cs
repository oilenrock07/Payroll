using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using System.Linq;

namespace Payroll.Repository.Repositories
{
    public class EmployeeWorkScheduleRepository : 
        Repository<EmployeeWorkSchedule>, IEmployeeWorkScheduleRepository
    {
        public EmployeeWorkScheduleRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeWorkSchedules;
        }

        public EmployeeWorkSchedule GetByEmployeeId(int employeeId)
        {
            return Find(e => e.IsActive && e.EmployeeId == employeeId)
                .OrderByDescending(e => e.EmployeeWorkScheduleId).Take(1).FirstOrDefault();
        }
    }
}
