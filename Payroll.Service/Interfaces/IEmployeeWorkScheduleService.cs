using Payroll.Entities;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeWorkScheduleService
    {
        EmployeeWorkSchedule GetByEmployeeId(int employeeId);
        EmployeeWorkSchedule Add(int workScheduleId, int employeeId);
        void Update(int workScheduleId, int employeeId);
    }
}
