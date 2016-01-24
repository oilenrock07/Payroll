using System;
using Payroll.Common.Enums;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IAttendanceRepository attendanceRepository, IUnitOfWork unitOfWork)
        {
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
        }

        public int CreateWorkSchedule(string employeeCode, AttendanceCode attCode, DateTime datetime)
        {
            try
            {
                if (attCode == AttendanceCode.ClockIn)
                {
                    var attendace = new Attendance()
                    {
                        ClockIn = datetime,
                        ClockOut = null,
                        EmployeeCode = employeeCode
                    };

                    _attendanceRepository.Add(attendace);
                }
                else if (attCode == AttendanceCode.ClockOut)
                {
                    var attendance = _attendanceRepository.GetLastAttendance(employeeCode);
                    attendance.ClockOut = datetime;

                    _attendanceRepository.Update(attendance);
                }

                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
