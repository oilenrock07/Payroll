using Payroll.Entities.Payroll;
using System.Collections.Generic;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeDeductionService
    {
        EmployeeDeduction GetByDeductionAndEmployee(int deductionId, int employeeId);
        IEnumerable<EmployeeDeduction> GetEmployeeDeduction(int employeeId);
        void UpdateEmployeeDeduction(IEnumerable<EmployeeDeduction> employeeDeductions, int employeeId);
    }
}
