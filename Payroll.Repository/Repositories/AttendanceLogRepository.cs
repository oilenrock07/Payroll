using System;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Models;

namespace Payroll.Repository.Repositories
{
    public class AttendanceLogRepository : Repository<AttendanceLog>, IAttendanceLogRepository
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AttendanceLogRepository(IDatabaseFactory databaseFactory, IEmployeeRepository employeeRepository)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().AttendanceLog;
            _employeeRepository = employeeRepository;
        }

        public IList<AttendanceLog> GetAttendanceLogs(DateTime fromDate, DateTime toDate, bool isRecorded)
        {
            return Find(a => a.IsRecorded == isRecorded
                && a.ClockInOut >= fromDate && a.ClockInOut < toDate)
                    .OrderBy(a => a.EmployeeId).ThenBy(a => a.ClockInOut).ToList();
        }

        public IEnumerable<AttendanceLogDao> GetAttendanceLogsWithName(DateTime fromDate, DateTime toDate)
        {
            var attendanceLog = Find(a => a.ClockInOut >= fromDate && a.ClockInOut < toDate && a.IsActive);

            var result = from attendance in attendanceLog
                        join employee in _employeeRepository.GetAll() on attendance.EmployeeId equals employee.EmployeeId
                        where employee.IsActive
                        select new AttendanceLogDao()
                        {
                            AttendanceLogId = attendance.AttendanceLogId,
                            EmployeeId = attendance.EmployeeId,
                            ClockInOut = attendance.ClockInOut,
                            IsRecorded = attendance.IsRecorded,
                            Type = attendance.Type,
                            LastName = employee.LastName,
                            FirstName = employee.FirstName,
                            MiddleName = employee.MiddleName,
                            IpAddress = attendance.IpAddress,
                            MachineId = attendance.MachineId
                        };

            return result.ToList();
        }

    }
}
