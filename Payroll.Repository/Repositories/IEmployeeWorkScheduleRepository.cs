using Payroll.Entities;

namespace Payroll.Repository.Repositories
{
    public interface IEmployeeWorkScheduleRepository
    {
        EmployeeWorkSchedule GetByEmployeeId(int employeeId);
    }
}