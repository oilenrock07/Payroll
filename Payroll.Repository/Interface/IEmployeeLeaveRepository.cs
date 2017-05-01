using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeLeaveRepository : IRepository<EmployeeLeave>
    {
        IEnumerable<EmployeeLeave> GetEmployeeLeavesByDate(int month, int year);

        IEnumerable<EmployeeLeave> GetEmployeePayableLeavesByDateRange(DateTime dateStart, DateTime dateEnd);

        int CountLeavesHolidayPayable(int employeeId, DateTime date);
    }
}
