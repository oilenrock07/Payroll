using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class EmployeeInfoHistoryRepository : Repository<EmployeeInfoHistory>, IEmployeeInfoHistoryRepository
    {
        public EmployeeInfoHistoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeInfoHistories;
        }
    }
}
