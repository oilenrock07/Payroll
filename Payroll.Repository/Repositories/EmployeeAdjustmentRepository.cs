using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class EmployeeAdjustmentRepository : Repository<EmployeeAdjustment>, IEmployeeAdjustmentRepository
    {
        public EmployeeAdjustmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeAdjustments;
        }
    }
}
