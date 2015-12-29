using System;
using System.Web.Mvc;
using Payroll.Entities;
using Payroll.Entities.Enums;
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

        public void ClockInOut(string code)
        {
            var employee = _employeeRepository.GetByCode(code);
            if (employee != null)
            {
                var attendance = new Attendance()
                {
                    EmployeeId = employee.EmployeeId,
                    AttendanceType = (int)AttendanceTypes.ClockIn,
                    ClockInOut = DateTime.Now
                };

                _attendanceRepository.Add(attendance);
                _unitOfWork.Commit();
            }
        }
    }
}