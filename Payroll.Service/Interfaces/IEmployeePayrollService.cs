using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeePayrollService
    {
        IList<EmployeePayroll> GeneratePayrollNetPayByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo);

        void Update(EmployeePayroll employeePayroll);

        IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate);

        void GeneratePayroll();

        void GeneratePayroll(DateTime payrollDate, DateTime dateFrom, DateTime dateTo);
    }
}
