using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Repositories
{
    public interface IEmployeeWorkScheduleRepository : IRepository<EmployeeWorkSchedule>
    {
        EmployeeWorkSchedule GetByEmployeeId(int employeeId);
    }
}