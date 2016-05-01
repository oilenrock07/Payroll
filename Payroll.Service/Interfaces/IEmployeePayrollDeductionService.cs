using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeePayrollDeductionService
    {
        int GenerateDeductionsByPayroll(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate, IList<EmployeePayroll> employeePayrolls);
    }
}
