using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;

namespace Payroll.Service.Interfaces
{
   public interface ITotalEmployeeHoursPerCompanyService
    {
        IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        IList<TotalEmployeeHoursPerCompany> GetByTypeAndDateRange(int employeeId, RateType? rateType, DateTime payrollStartDate, DateTime payrollEndDate);

        IList<TotalEmployeeHoursPerCompany> GetByEmployeeDate(int employeeId, DateTime date);

        double CountTotalHours(int employeeId, DateTime date);
    }
}
