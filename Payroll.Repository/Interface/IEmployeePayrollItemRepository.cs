using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Interface
{
    public interface IEmployeePayrollItemRepository : IRepository<EmployeePayrollItem>
    {
        EmployeePayrollItem Find(int employeeId, DateTime date, RateType rateType);

        IList<EmployeePayrollItem> GetByDateRange(DateTime dateFrom, DateTime dateTo);
    }
}
