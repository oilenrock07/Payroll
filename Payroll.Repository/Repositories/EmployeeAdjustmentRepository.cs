using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Repository.Repositories
{
    public class EmployeeAdjustmentRepository : Repository<EmployeeAdjustment>, IEmployeeAdjustmentRepository
    {
        public EmployeeAdjustmentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeAdjustments;
        }

        public virtual IEnumerable<EmployeeAdjustment> GetByPayroll(IEnumerable<int> payrollIds)
        {
            return GetAllActive().Where(x => payrollIds.Contains(x.PayrollId.Value));
        }
    }
}
