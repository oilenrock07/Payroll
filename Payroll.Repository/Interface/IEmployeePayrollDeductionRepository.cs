using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeePayrollDeductionRepository : IRepository<EmployeePayrollDeduction>
    {
        IList<EmployeePayrollDeduction> GetByPayroll(int payrollId);

        IEnumerable<EmployeePayrollDeduction> GetByPayroll(IEnumerable<int> payrollIds);
    }
}
