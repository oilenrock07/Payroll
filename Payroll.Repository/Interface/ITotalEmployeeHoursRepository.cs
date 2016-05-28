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
    public interface ITotalEmployeeHoursRepository : IRepository<TotalEmployeeHours>
    {
        TotalEmployeeHours GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type);

        IList<TotalEmployeeHours> GetByDateRange(DateTime dateFrom, DateTime dateTo);

        IList<TotalEmployeeHours> GetByTypeAndDateRange(int employeeId, RateType rateType, DateTime payrollStartDate, DateTime payrollEndDate);
    }
}
