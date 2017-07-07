using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using Payroll.Entities.Enums;

namespace Payroll.Repository.Interface
{
    public interface ITotalEmployeeHoursPerCompanyRepository : IRepository<TotalEmployeeHoursPerCompany>
    {
        void DeleteByTotalEmployeeHoursPerCompanyIds(IEnumerable<int> ids);

        IList<TotalEmployeeHoursPerCompany> GetByEmployeeDate(int employeeId, DateTime date);

        IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        IList<TotalEmployeeHoursPerCompany> GetByTypeAndDateRange(int employeeId, RateType rateType, DateTime payrollStartDate, DateTime payrollEndDate);

        IList<TotalEmployeeHoursPerCompany> GetByDateRange(int employeeId, DateTime payrollStartDate, DateTime payrollEndDate);

        double CountTotalHours(int employeeId, DateTime date);
    }
}
