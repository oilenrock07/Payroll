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

        public IList<Attendance> GetAttendanceByDateRange(string employeeCode, DateTime fromDate, DateTime toDate)
        {
            return Find(a => a.EmployeeCode == employeeCode &&
                a.ClockIn >= fromDate && a.ClockIn < toDate).OrderBy(a => a.ClockIn).ToList();
        }

        public Attendance GetLastAttendance(string employeeCode)
        {
            return Find(a => a.ClockOut == null && a.ClockIn != null && a.EmployeeCode == employeeCode)
                   .OrderByDescending(a => a.AttendanceId).Take(1).FirstOrDefault();
        }
    }
}
