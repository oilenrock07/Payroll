using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class WorkScheduleRepository : Repository<WorkSchedule>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().WorkSchedules;
        }
    }
}
