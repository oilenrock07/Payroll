using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeHoursService
    {
        int GenerateEmployeeHours(DateTime fromDate, DateTime toDate);

        void ComputeEmployeeHours(DateTime day, int employeeId);

        IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime fromDate, DateTime toDate);

        IList<EmployeeHours> GetForProcessingByEmployeeAndDate(int employeeId, DateTime date);

        IList<EmployeeHours> GetForProcessingByDateRange(bool isManual, DateTime fromDate, DateTime toDate);

        IList<EmployeeHours> GetByDateRange(DateTime fromDate, DateTime toDate);

        void Update(EmployeeHours employeeHours);
    }
}
