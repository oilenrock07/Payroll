using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Attendance GetLastAttendance(string employeeCode);
    }
}
