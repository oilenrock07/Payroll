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
        Decimal GenerateDeductionsByPayroll(EmployeePayroll employeePayroll);

        EmployeePayrollDeduction Add(EmployeePayrollDeduction employeePayrollDeduction);

        IList<EmployeePayrollDeduction> GetByPayroll(int payrollId);

        bool proceedDeduction(DateTime payrollStartDate, DateTime payrollEndDate);

        decimal ComputeTax(int payrollId, EmployeeInfo employeeInfo, decimal totalTaxableIncome);
    }
}
