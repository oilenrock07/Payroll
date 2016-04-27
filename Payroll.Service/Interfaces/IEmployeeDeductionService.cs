using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeDeductionService
    {
        EmployeeDeduction GetByDeductionAndEmployee(int deductionId, int employeeId);
    }
}
