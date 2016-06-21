using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class SchedulerLogRepository : Repository<SchedulerLog>, ISchedulerLogRepository
    {
        public SchedulerLogRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().SchedulerLogs;
        }
    }
}
