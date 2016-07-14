using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeePayrollItemService : IBaseEntityService<EmployeePayrollItem>
    {
        EmployeePayrollItem Find(int employeeId, DateTime date, RateType rateType);

        IList<EmployeePayrollItem> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        void GenerateEmployeePayrollItemByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate);

        IEnumerable<EmployeePayrollItem> GetByPayrollId(int payrollId);

        DataTable GetPayrollDetailsForExport(DateTime startDate, DateTime endDate);
    }
}
