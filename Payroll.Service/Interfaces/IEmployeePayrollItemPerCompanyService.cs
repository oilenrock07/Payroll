using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeePayrollItemPerCompanyService : IBaseEntityService<EmployeePayrollItemPerCompany>
    {
      
        IList<EmployeePayrollItemPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        IEnumerable<EmployeePayrollItemPerCompany> GetByCutoffDates(DateTime dateFrom, DateTime dateTo);

        void GenerateEmployeePayrollItemByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate);

        IEnumerable<EmployeePayrollItemPerCompany> GetByPayrollId(int payrollId);

        DataTable GetPayrollDetailsForExport(DateTime startDate, DateTime endDate);

    }
}
