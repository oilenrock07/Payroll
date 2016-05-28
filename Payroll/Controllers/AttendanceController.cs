﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Helper;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Attendance;
using Payroll.Repository.Interface;
using Payroll.Repository.Models;
using System.Linq;
using Payroll.Resources;

namespace Payroll.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IAttendanceLogRepository attendanceLogRepository,
            IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            _attendanceLogRepository = attendanceLogRepository;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public virtual ActionResult CreateAttendance()
        {
            var viewModel = new CreateAttendanceViewModel
            {
                Employees = GetEmployeeNames()
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult CreateAttendance(CreateAttendanceViewModel viewModel)
        {
            var clockIn = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockIn.ToShortDateString(), viewModel.ClockInTime));
            var clockOut = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockOut.ToShortDateString(), viewModel.ClockOutTime));
            viewModel.Employees = GetEmployeeNames();

            //validate clockin and clockout date, should not be a future date
            if (clockIn > DateTime.Now || clockOut > DateTime.Now)
            {
                ModelState.AddModelError("", ErrorMessages.ATTENDANCE_INVALID_FUTUREDATE);
                return View("CreateAttendance", viewModel);
            }

            var attendance = viewModel.MapItem<Attendance>();
            attendance.IsManuallyEdited = true;
            attendance.ClockIn = clockIn;
            attendance.ClockOut = clockOut;

            _attendanceRepository.Add(attendance);
            _unitOfWork.Commit();

            ViewData["CreateSuccess"] = "Attendance successfully created";
            return View(new CreateAttendanceViewModel {Employees = viewModel.Employees});
        }

        public virtual ActionResult EditAttendance(int id)
        {
            var attendance = _attendanceRepository.GetById(id);
            var viewModel = attendance.MapItem<CreateAttendanceViewModel>();
            viewModel.ClockIn = attendance.ClockIn;
            viewModel.ClockOut = attendance.ClockOut.HasValue ? attendance.ClockOut.Value : DateTime.MinValue;
            viewModel.ClockInTime = attendance.ClockIn.ToShortTimeString();
            viewModel.ClockOutTime = attendance.ClockOut.HasValue ? attendance.ClockOut.Value.ToShortTimeString() : "";
            viewModel.FullName = attendance.Employee.FullName;

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult EditAttendance(CreateAttendanceViewModel viewModel)
        {
            var clockIn = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockIn.ToShortDateString(), viewModel.ClockInTime));
            var clockOut = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockOut.ToShortDateString(), viewModel.ClockOutTime));
            viewModel.Employees = GetEmployeeNames();

            //validate clockin and clockout date, should not be a future date
            if (clockIn > DateTime.Now || clockOut > DateTime.Now)
            {
                ModelState.AddModelError("", ErrorMessages.ATTENDANCE_INVALID_FUTUREDATE);
                return View("EditAttendance", viewModel);
            }

            var oldAttendance = _attendanceRepository.GetById(viewModel.AttendanceId);
            _attendanceRepository.Update(oldAttendance);
            oldAttendance.IsActive = false;

            var attendance = viewModel.MapItem<Attendance>();
            attendance.IsManuallyEdited = true;
            attendance.ClockIn = clockIn;
            attendance.ClockOut = clockOut;

            _attendanceRepository.Add(attendance);
            _unitOfWork.Commit();

            ViewData["EditSuccess"] = "Attendance successfully updated";
            return RedirectToAction("Attendance");
        }

        protected IEnumerable<SelectListItem> GetEmployeeNames()
        {
            return _employeeRepository.GetEmployeeNames()
                    .Select(x => new SelectListItem
                    {
                        Value = x.EmployeeId.ToString(),
                        Text = x.FullName
                    });
        }

        public virtual ActionResult Attendance()
        {
            return View();
        }

        [HttpPost]
        public virtual PartialViewResult AttendanceContent(string startDate, string endDate)
        {
            var viewModel = GetAttendance(startDate, endDate);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return PartialView(viewModel);
        }

        protected virtual IEnumerable<AttendanceViewModel> GetAttendance(string startDate, string endDate)
        {
            //do not display the edit link if attendance date < last payroll date
            var lastPayrollDate = new DateTime(2016, 05, 22);

            var result = _attendanceRepository.GetAttendanceByDateRange(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            var viewModel = result.MapCollection<Attendance, AttendanceViewModel>((s, d) =>
            {
                d.ClockOut = s.ClockOut.Value;
                d.FirstName = s.Employee.FirstName;
                d.LastName = s.Employee.LastName;
                d.MiddleName = s.Employee.MiddleName;
                d.Editable = s.ClockIn > lastPayrollDate;
            });

            return viewModel;
        }

        public void ExportToExcel(string startDate, string endDate)
        {
            var viewModel = GetAttendance(startDate, endDate);
            var fileName = String.Format("Attendance_Report_{0}-{1}", Convert.ToDateTime(startDate).Serialize(), Convert.ToDateTime(endDate).Serialize());
            Export.ToExcel(Response, viewModel, fileName);
        }

        public virtual ActionResult AttendanceLog()
        {
            return View();
        }

        [HttpPost]
        public virtual PartialViewResult AttendanceLogContent(string startDate, string endDate)
        {
            var result = _attendanceLogRepository.GetAttendanceLogsWithName(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1));
            var viewModel = result.MapCollection<AttendanceLogDao, AttendanceLogViewModel>((s, d) =>
            {
                d.Datetime = String.Format("{0} {1}", s.ClockInOut.ToLongDateString(), s.ClockInOut.ToLongTimeString());
                d.IsRecorded = s.IsRecorded ? "Yes" : "No";
                d.FullName = String.Format("{0}, {1} {2}", s.LastName, s.FirstName, s.MiddleName);
                d.Type = s.Type == AttendanceType.ClockIn ? "Clock in" : "Clock out";
            });

            return PartialView(viewModel);
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