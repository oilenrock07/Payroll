using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Service.Implementations
{
    public class AttendanceLogService : BaseEntityService<AttendanceLog>, IAttendanceLogService
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;

        public AttendanceLogService(IAttendanceLogRepository attendanceLogRepository) 
            : base(attendanceLogRepository)
        {
            _attendanceLogRepository = attendanceLogRepository;
        }

        public IList<AttendanceLog> GetAttendanceLogsToBeProcessed()
        {
            return _attendanceLogRepository.GetAttendanceLogs(false);
        }

        public IList<AttendanceLog> GetAttendanceLogsToBeProcessed(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            return _attendanceLogRepository.GetAttendanceLogs(fromDate, toDate, false);
        }

    }
}
