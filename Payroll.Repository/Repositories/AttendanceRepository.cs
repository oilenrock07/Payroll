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

        public Attendance GetLastAttendance(string employeeCode)
        {
            return Find(e => e.ClockOut == null && e.ClockIn != null && e.EmployeeCode == employeeCode)
                   .OrderByDescending(e => e.AttendanceId).Take(1).FirstOrDefault();
        }
    }
}
