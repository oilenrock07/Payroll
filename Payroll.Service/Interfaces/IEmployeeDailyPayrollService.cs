using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeDailyPayrollService
    {
        void GenerateEmployeeDailySalaryByDateRange(DateTime dateFrom, DateTime dateTo);

        IList<EmployeeDailyPayroll> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        void GenerateEmployeeHolidayPay(DateTime payrollStartDate, DateTime payrollEndDate);

        IList<EmployeeDailyPayroll> GetByTypeAndDateRange(RateType rateType, DateTime dateFrom, DateTime dateTo);
    }
}
