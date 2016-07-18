using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeAdjustmentRepository : IRepository<EmployeeAdjustment>
    {
        IEnumerable<EmployeeAdjustment> GetByPayroll(IEnumerable<int> payrollIds);
    }
}
