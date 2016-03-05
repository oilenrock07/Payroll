using System;
using System.Web.Mvc;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public void ClockIn(int employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            if (employee != null)
            {
                var attendance = new Attendance()
                {
                    EmployeeId = employeeId,
                    ClockIn = DateTime.Now,
                    ClockOut = null
                };

                _attendanceRepository.Add(attendance);
                _unitOfWork.Commit();
            }
        }

        public void ClockOut(int employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            
            if (employee != null)
            {
                var lastClockIn = _attendanceRepository.GetLastAttendance(employeeId);

                if (lastClockIn != null)
                {
                    _attendanceRepository.Update(lastClockIn);
                    lastClockIn.ClockOut = DateTime.Now;
                    _unitOfWork.Commit();
                }
                else
                {
                    //employee did not clocked in
                }
            }
        }
    }
}