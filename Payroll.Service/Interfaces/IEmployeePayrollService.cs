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
        IList<EmployeePayroll> GeneratePayrollGrossPayByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo);

        void Update(EmployeePayroll employeePayroll);

        IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate);

        void GeneratePayroll(DateTime? date);

        void GeneratePayroll(DateTime payrollDate, DateTime dateFrom, DateTime dateTo);

        DateTime GetNextPayrollStartDate(FrequencyType frequency, DateTime? date);

        DateTime GetNextPayrollEndDate(FrequencyType frequency, DateTime payrollStartDate);
    
        /*
         * This will return payroll with payroll date between date start and date end
         */
        IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd);
    }
}
