using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            
        }

        public Attendance GetLastAttendance(int employeeId)
        {
            return Find(e => e.ClockOut == null && e.ClockIn != null && e.EmployeeId == employeeId)
                   .OrderByDescending(e => e.AttendanceId).Take(1).FirstOrDefault();
        }
    }
}
