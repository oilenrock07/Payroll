using System;
using System.Linq;
using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class EmployeeLeaveRepository : Repository<EmployeeLeave>, IEmployeeLeaveRepository
    {
        public EmployeeLeaveRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeLeaves;
        }

        public IEnumerable<EmployeeLeave> GetEmployeeLeavesByDate(DateTime startDate, DateTime endDate, int employeeId)
        {
            var result = Find(x => x.IsActive && x.Employee.IsActive && x.StartDate >= startDate 
                         && x.EndDate <= endDate);

            if (employeeId > 0) result = result.Where(x => x.EmployeeId == employeeId);

            return result;
        }

        public IEnumerable<EmployeeLeave> GetEmployeePayableLeavesByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            return Find(x => x.IsActive && x.Employee.IsActive && x.Leave.IsPayable 
                        && x.LeaveStatus == Entities.Enums.LeaveStatus.Approved
                        && x.StartDate >= dateStart && x.EndDate <= dateEnd);
        }

        public int CountLeavesHolidayPayable(int employeeId, DateTime date)
        {
            return Find(x => x.IsActive && x.EmployeeId == employeeId 
                        && x.Employee.IsActive && x.Leave.IsHolidayAfterPayable
                        && x.LeaveStatus == Entities.Enums.LeaveStatus.Approved
                        && x.StartDate >= date && x.EndDate <= date).Count();
        }
    }
}
