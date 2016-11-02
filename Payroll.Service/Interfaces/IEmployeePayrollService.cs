using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Service.Models;
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

        void GeneratePayroll(DateTime dateFrom, DateTime dateTo);

        DateTime GetNextPayrollStartDate();

        DateTime GetNextPayrollStartDate(DateTime? date);

        DateTime GetLatestPayrollStartDate();

        DateTime GetNextPayrollEndDate(DateTime payrollStartDate);

        DateTime GetNextPayrollReleaseDate(DateTime payrollEndDate);

        /*
         * This will return payroll with payroll date between date start and date end
         */
        IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd);

        /*
         * Get all payrolls with same start date and end date
         */
        IList<EmployeePayroll> GetByPayrollDateRange(DateTime payrollStartDate, DateTime payrollEndDate);

        IEnumerable<PayrollDate> GetPayrollDates(int months, DateTime? endDate = null);

        EmployeePayroll GetById(int id);

        bool IsPayrollComputed(DateTime startDate, DateTime endDate);
    }
}
