using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class AttendanceLogService : IAttendanceLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceLogRepository _attendanceLogRepository;

        public AttendanceLogService(IAttendanceLogRepository attendanceLogRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _attendanceLogRepository = attendanceLogRepository;
        }

        public IList<AttendanceLog> GetAttendanceLogsToBeProcessed(DateTime fromDate, DateTime toDate)
        {
            return _attendanceLogRepository.GetAttendanceLogs(fromDate, toDate, false);
        }

        public void Save(AttendanceLog attendanceLog)
        {
            if (attendanceLog.AttendanceLogId == null 
                    && attendanceLog.AttendanceLogId > 0)
            {
                _attendanceLogRepository.Add(attendanceLog);
            }
            else
            {
                _attendanceLogRepository.Update(attendanceLog);
            }

            _unitOfWork.Commit();
        }
    }
}
