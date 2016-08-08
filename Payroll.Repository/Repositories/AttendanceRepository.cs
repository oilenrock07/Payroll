using System;
using System.Collections.Generic;
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
            DbSet = databaseFactory.GetContext().Attendances;
        }

        public IList<Attendance> GetAttendanceByDateRange(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            return Find(a => a.IsActive && a.ClockIn >= fromDate && a.ClockIn <= toDate).ToList();
        }

        public IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return Find(a => a.IsActive && a.EmployeeId == employeeId &&
                ((a.ClockIn >= fromDate && a.ClockIn < toDate) ||
                    (a.ClockOut >= fromDate && a.ClockOut < toDate))).OrderBy(a => a.ClockIn).ToList();
        }

        public IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate, bool isHoursCounted)
        {
            return Find(a => a.IsActive && a.EmployeeId == employeeId && a.IsHoursCounted == isHoursCounted &&
                ((a.ClockIn >= fromDate && a.ClockIn < toDate) ||
                    (a.ClockOut >= fromDate && a.ClockOut < toDate))).OrderBy(a => a.ClockIn).ToList();
        }

        public IList<Attendance> GetAttendanceByProcessing(int employeeId, DateTime fromDate, DateTime toDate, bool isHoursCounted)
        {
            return Find(a => a.IsActive && a.EmployeeId == employeeId && a.IsHoursCounted == isHoursCounted && a.ClockOut != null &&
                ((a.ClockIn >= fromDate && a.ClockIn < toDate) ||
                    (a.ClockOut >= fromDate && a.ClockOut < toDate))).OrderBy(a => a.ClockIn).ToList();
        }

        public Attendance GetLastAttendance(int employeeId)
        {
            return Find(a => a.IsActive && a.EmployeeId == employeeId)
                   .OrderByDescending(a => a.AttendanceId).Take(1).FirstOrDefault();
        }
    }
}
