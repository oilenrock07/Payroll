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
    public class AttendanceLogService : IAttendanceLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceLogRepository _attendanceLogRepository;

        public AttendanceLogService(IUnitOfWork unitOfWork, IAttendanceLogRepository attendanceLogRepository)
        {
            _unitOfWork = unitOfWork;
            _attendanceLogRepository = attendanceLogRepository;
        }

        public IList<AttendanceLog> GetAttendanceLogsToBeProcessed(DateTime fromDate, DateTime toDate)
        {
            return _attendanceLogRepository.GetAttendanceLogs(fromDate, toDate, false);
        }

        public void Add(AttendanceLog attendanceLog)
        {
             _attendanceLogRepository.Add(attendanceLog);
            _unitOfWork.Commit();
        }

        public void Update(AttendanceLog attendanceLog)
        {
            _attendanceLogRepository.Update(attendanceLog);
        }
    }
}
