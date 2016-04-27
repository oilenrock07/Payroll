using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeLeaveRepository : IRepository<EmployeeLeave>
    {
        IEnumerable<EmployeeLeave> GetEmployeeLeavesByDate(int month, int year);
    }
}
