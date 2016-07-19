using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeHoursRepository : IRepository<EmployeeHours>
    {
        IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime dateFrom, DateTime dateTo);

        IList<EmployeeHours> GetForProcessingByDateRange(bool isManual, DateTime fromDate, DateTime toDate);

        IList<EmployeeHours> GetByDateRange(DateTime fromDate, DateTime toDate);
    }
}
