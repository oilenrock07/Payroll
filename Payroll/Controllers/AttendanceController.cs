using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Payroll.Service.Interfaces;
using Payroll.Entities.Payroll;

namespace Payroll.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeePayrollRepository _employeePayrollRepository;
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly ITotalEmployeeHoursService _totalEmployeeHoursService;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ITotalEmployeeHoursPerCompanyRepository _totalEmployeeHoursPerCompanyRepository;
        private readonly ICompanyRepository _companyRepository;

        public AttendanceController(IAttendanceLogRepository attendanceLogRepository,
            IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork,
            IEmployeePayrollRepository employeePayrollRepository, IEmployeeHoursRepository employeeHoursRepository, IAttendanceService attendanceService,
            IHolidayRepository holidayRepository, IEmployeeHoursService employeeHoursService, ITotalEmployeeHoursService totalEmployeeHoursService,
            ITotalEmployeeHoursPerCompanyRepository totalEmployeeHoursPerCompanyRepository, ICompanyRepository companyRepository)
        {
            _attendanceLogRepository = attendanceLogRepository;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _employeePayrollRepository = employeePayrollRepository;
            _employeeHoursRepository = employeeHoursRepository;
            _attendanceService = attendanceService;
            _holidayRepository = holidayRepository;
            _employeeHoursService = employeeHoursService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _totalEmployeeHoursPerCompanyRepository = totalEmployeeHoursPerCompanyRepository;
            _companyRepository = companyRepository;
        }

        public virtual ActionResult CreateAttendance()
        {
            return View(new CreateAttendanceViewModel());
        }

        [HttpPost]
        public virtual ActionResult CreateAttendance(CreateAttendanceViewModel viewModel)
        {
            var clockIn = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockIn.ToShortDateString(), viewModel.ClockInTime));
            var clockOut = Convert.ToDateTime(String.Format("{0} {1}", viewModel.ClockOut.ToShortDateString(), viewModel.ClockOutTime));

            //validate clock in should not be greater than or equal to clock out
            if (clockIn >= clockOut)
            {
                if (viewModel.EmployeeId > 0)
                {
                    var employee = _employeeRepository.GetById(viewModel.EmployeeId);
                    ViewData["Name"] = employee.FullName;
                }
                
                ModelState.AddModelError("", ErrorMessages.ATTENDANCE_CLOCKIN_GREATER_THAN_CLOCKOUT);
                return View("CreateAttendance", viewModel);
            }

            //validate clockin and clockout date, should not be a future date
            if (clockIn > DateTime.Now || clockOut > DateTime.Now)
            {
                if (viewModel.EmployeeId > 0)
                {
                    var employee = _employeeRepository.GetById(viewModel.EmployeeId);
                    ViewData["Name"] = employee.FullName;
                }

                ModelState.AddModelError("", ErrorMessages.ATTENDANCE_INVALID_FUTUREDATE);
                return View("CreateAttendance", viewModel);
            }

            var attendance = viewModel.MapItem<Attendance>();
            attendance.IsManuallyEdited = true;
            attendance.ClockIn = clockIn;
            attendance.ClockOut = clockOut;

            _attendanceRepository.Add(attendance);
            _unitOfWork.Commit();
            RecomputeEmployeeHours(attendance.ClockIn, attendance.ClockOut.Value, attendance.EmployeeId);
            RecomputeTotalEmployeeHours(attendance.ClockIn, attendance.ClockOut.Value, attendance.EmployeeId);

            ViewData["CreateSuccess"] = "Attendance successfully created";
            return View(new CreateAttendanceViewModel());
        }

        public virtual ActionResult EditAttendance(int id)
        {
            var attendance = _attendanceRepository.GetById(id);
            var viewModel = attendance.MapItem<CreateAttendanceViewModel>();
            viewModel.EmployeeId = attendance.EmployeeId;
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

            //delete employee hours
            var employeeHours = _employeeHoursRepository.Find(x => x.OriginAttendanceId == attendance.AttendanceId);
            if (employeeHours != null && employeeHours.Any())
            {
                foreach (var employeeHour in employeeHours)
                {
                    if (employeeHour.IsIncludedInTotal)
                    {
                        var employeeTotalHours = _totalEmployeeHoursService.GetByEmployeeDateAndType(employeeHour.EmployeeId, employeeHour.Date, employeeHour.Type);
                        if (employeeTotalHours != null)
                        {
                            _totalEmployeeHoursService.Update(employeeTotalHours);
                            employeeTotalHours.Hours -= employeeHour.Hours;
                        }
                    }

                    _employeeHoursRepository.Update(employeeHour);
                    employeeHour.IsActive = false;
                }
            }

            _unitOfWork.Commit();
            RecomputeEmployeeHours(attendance.ClockIn, attendance.ClockOut.Value, attendance.EmployeeId);
            RecomputeTotalEmployeeHours(attendance.ClockIn, attendance.ClockOut.Value, attendance.EmployeeId);
            ViewData["EditSuccess"] = "Attendance successfully updated";
            return RedirectToAction("Attendance");
        }

        private void RecomputeEmployeeHours(DateTime clockin, DateTime clockout, int employeeId)
        {
            //recompute employee hours            
            _employeeHoursService.ComputeEmployeeHours(clockin, employeeId);
            if (clockin.Date != clockout.Date)
                _employeeHoursService.ComputeEmployeeHours(clockout, employeeId);
        }

        private void RecomputeTotalEmployeeHours(DateTime clockin, DateTime clockout, int employeeId)
        {
            _totalEmployeeHoursService.GenerateTotalByDateAndEmployee(employeeId, clockin);
            if (clockin.Date != clockout.Date)
                _totalEmployeeHoursService.GenerateTotalByDateAndEmployee(employeeId, clockout);
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
        public virtual PartialViewResult AttendanceContent(string startDate, string endDate, int employeeId)
        {
            var viewModel = GetAttendance(startDate, endDate, employeeId);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.EmployeeId = employeeId;
            return PartialView(viewModel);
        }

        protected virtual IEnumerable<AttendanceViewModel> GetAttendance(string startDate, string endDate, int employeeId)
        {
            //do not display the edit link if attendance date < last payroll date
            var nextPayrollDate = _employeePayrollRepository.GetNextPayrollStartDate(); //this is actually the last payroll date
            var lastPayrollDate = nextPayrollDate ?? DateTime.MinValue;
            var sDate = startDate.ToDateTime();
            var eDate = endDate.ToDateTime();

            var holidays = _holidayRepository.Find(x => x.IsActive && x.Date >= sDate && x.Date <= eDate).ToList();

            Func<IEnumerable<EmployeeHours>, bool> isNotEmpty = x => x != null && x.Any(y => y != null);
            Func<DateTime, bool> isHoliday = x => holidays.Any(y => y.Date == x.Date);
            Func<DateTime, bool> isRegularHoliday = x =>
            {
                var holiday = holidays.FirstOrDefault(y => y.Date == x.Date);
                return holiday != null && holiday.IsRegularHoliday;
            };

            var result = _attendanceService.GetAttendanceAndHoursByDate(sDate, eDate, employeeId).ToList();
            var viewModel = result.MapCollection<AttendanceDao, AttendanceViewModel>((s, d) =>
            {
                d.Editable = s.ClockIn > lastPayrollDate;
                d.RegularHours = isNotEmpty(s.EmployeeHours) ? s.EmployeeHours.Where(x => x.Type == RateType.Regular).Sum(x => x.Hours) : 0;
                d.Overtime = isNotEmpty(s.EmployeeHours) ? s.EmployeeHours.Where(x => x.Type == RateType.OverTime).Sum(x => x.Hours) : 0;
                d.NightDifferential = isNotEmpty(s.EmployeeHours) ? s.EmployeeHours.Where(x => x.Type == RateType.NightDifferential).Sum(x => x.Hours) : 0;
                d.IsHoliday = isHoliday(s.ClockIn);
                d.IsRegularHoliday = isRegularHoliday(s.ClockIn);
                d.Breakdown = s.EmployeeHours.Where(x => x != null).ToList().GroupBy(x => x.Date).Select(x => new AttendanceBreakdownViewModel
                {                    
                    Date = x.Key,
                    NightDifferential = isNotEmpty(x) ? x.Where(y => y.Type == RateType.NightDifferential).Sum(y => y.Hours) : 0,
                    Overtime = isNotEmpty(x) ?  x.Where(y => y.Type == RateType.OverTime).Sum(y => y.Hours) : 0,
                    RegularHours = isNotEmpty(x) ? x.Where(y => y.Type == RateType.Regular).Sum(y => y.Hours) : 0,
                    IsHoliday = isHoliday(x.Key),
                    IsRegularHoliday = isRegularHoliday(x.Key)
                });
            });

            return viewModel;
        }

        public void ExportToExcel(string startDate, string endDate, int employeeId)
        {
            var viewModel = GetAttendance(startDate, endDate, employeeId);
            var fileName = String.Format("Attendance_Report_{0}-{1}", startDate.ToDateTime().SerializeShort(), endDate.ToDateTime().SerializeShort());

            var dt = new DataTable();
            dt.Columns.Add("First Name", typeof(string));
            dt.Columns.Add("Middle Name", typeof(string));
            dt.Columns.Add("Last Name", typeof(string));
            dt.Columns.Add("Clock In", typeof(string));
            dt.Columns.Add("Clock Out", typeof(string));
            dt.Columns.Add("Regular Hours", typeof(string));
            dt.Columns.Add("Overtime", typeof(string));
            dt.Columns.Add("Night Differential", typeof(string));

            foreach (var item in viewModel)
            {
                var row = dt.NewRow();

                row["First Name"] = item.FirstName;
                row["Middle Name"] = item.MiddleName;
                row["Last Name"] = item.LastName;
                row["Clock In"] = item.ClockIn;
                row["Clock Out"] = item.ClockOut;
                row["Regular Hours"] = item.RegularHours;
                row["Overtime"] = item.Overtime;
                row["Night Differential"] = item.NightDifferential;

                dt.Rows.Add(row);
                if (item.Breakdown != null && item.Breakdown.Count() > 1)
                {
                    row = dt.NewRow();
                    row["Clock In"] = "Date";

                    dt.Rows.Add(row);

                    foreach (var breakdown in item.Breakdown)
                    {
                        row = dt.NewRow();
                        row["Clock In"] = breakdown.Date.ToShortDateString();
                        row["Regular Hours"] = breakdown.RegularHours;
                        row["Overtime"] = breakdown.Overtime;
                        row["Night Differential"] = breakdown.NightDifferential;

                        dt.Rows.Add(row);
                    }
                }
            }

            Export.ToExcel(Response, dt, fileName);
        }

        public virtual ActionResult AttendanceLog()
        {
            return View();
        }

        [HttpPost]
        public virtual PartialViewResult AttendanceLogContent(string startDate, string endDate, int employeeId = 0)
        {
            var result = _attendanceLogRepository.GetAttendanceLogsWithName(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), employeeId);
            var viewModel = result.MapCollection<AttendanceLogDao, AttendanceLogViewModel>((s, d) =>
            {
                d.Datetime = String.Format("{0} {1}", s.ClockInOut.ToLongDateString(), s.ClockInOut.ToLongTimeString());
                d.IsRecorded = s.IsRecorded ? "Yes" : "No";
                d.FullName = String.Format("{0}, {1} {2}", s.LastName, s.FirstName, s.MiddleName);
                d.Type = s.Type == AttendanceType.ClockIn ? "Clock in" : "Clock out";
            });

            return PartialView(viewModel);
        }

        public virtual void ClockIn(int employeeId)
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

        public virtual void ClockOut(int employeeId)
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

        public virtual ActionResult HoursPerCompany()
        {
            return View();
        }

        public virtual PartialViewResult HoursPerCompanyContent(string startDate, string endDate, int employeeId)
        {
            var viewModel = GetTotalEmployeeHours(startDate.ToDateTime(), endDate.ToDateTime(), employeeId);
            return PartialView(viewModel);
        }

        [HttpPost]
        public virtual JsonResult ViewHoursPerCompanyModal(int employeeId, string date)
        {
            var deserializedDate = date.DeserializeDate();
            var employeeTotalHours = GetTotalEmployeeHours(deserializedDate, deserializedDate, employeeId);
            if (employeeTotalHours != null)
            {
                var endDate = deserializedDate.Date.AddDays(1);
                var employeeTotalHoursPerCompany = _totalEmployeeHoursPerCompanyRepository
                    .Find(x => x.TotalEmployeeHours.Date > deserializedDate && x.TotalEmployeeHours.Date < endDate
                    && x.TotalEmployeeHours.EmployeeId == employeeId).ToList();

                var companies = _companyRepository.GetAllActive().ToList();

                var emp = employeeTotalHours.First();

                var viewModel = new CreateHoursPerCompanyViewModel
                {
                    Companies = companies,
                    EmployeeTotalHoursViewModel = emp,
                    RegularHoursPerCompany = employeeTotalHoursPerCompany.Where(x => x.TotalEmployeeHoursId == emp.TotalRegularHoursId),
                    OvertimePerCompany = employeeTotalHoursPerCompany.Where(x => x.TotalEmployeeHoursId == emp.TotalOvertimeId),
                    NightDifferentialPerCompany = employeeTotalHoursPerCompany.Where(x => x.TotalEmployeeHoursId == emp.TotalNightDifferentialId).ToList(),
                    ModalTitle = String.Format("{0} {1} {2} - {3}", emp.FirstName, emp.MiddleName, emp.LastName, deserializedDate.ToShortDateString())
                };
                return Json(viewModel);
            }

            return null;
        }

        [HttpPost]
        public virtual JsonResult CreateHoursPerCompany(CreateHoursPerCompanyViewModel viewModel)
        {
            try
            {
                var toDeleteIds = new List<int>();
                if (viewModel.RegularHoursPerCompany != null)
                    toDeleteIds.AddRange(viewModel.RegularHoursPerCompany.Where(x => x.TotalEmployeeHoursPerCompanyId > 0).Select(x => x.TotalEmployeeHoursPerCompanyId));
                if (viewModel.OvertimePerCompany != null)
                    toDeleteIds.AddRange(viewModel.OvertimePerCompany.Where(x => x.TotalEmployeeHoursPerCompanyId > 0).Select(x => x.TotalEmployeeHoursPerCompanyId));
                if (viewModel.NightDifferentialPerCompany != null)
                    toDeleteIds.AddRange(viewModel.NightDifferentialPerCompany.Where(x => x.TotalEmployeeHoursPerCompanyId > 0).Select(x => x.TotalEmployeeHoursPerCompanyId));

                //delete first all the existing records
                _totalEmployeeHoursPerCompanyRepository.DeleteByTotalEmployeeHoursPerCompanyIds(toDeleteIds);

                //add them again
                if (viewModel.RegularHoursPerCompany != null)
                {
                    foreach (var item in viewModel.RegularHoursPerCompany)
                    {
                        item.TotalEmployeeHours = null;
                        item.Company = null;
                        _totalEmployeeHoursPerCompanyRepository.Add(item);
                    }
                }

                if (viewModel.OvertimePerCompany != null)
                {
                    foreach (var item in viewModel.OvertimePerCompany)
                    {
                        item.TotalEmployeeHours = null;
                        item.Company = null;
                        _totalEmployeeHoursPerCompanyRepository.Add(item);
                    }
                }

                if (viewModel.NightDifferentialPerCompany != null)
                {
                    foreach (var item in viewModel.NightDifferentialPerCompany)
                    {
                        item.TotalEmployeeHours = null;
                        item.Company = null;
                        _totalEmployeeHoursPerCompanyRepository.Add(item);
                    }
                }

                _unitOfWork.Commit();
                return Json(new {Success = true});
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        private IEnumerable<EmployeeTotalHoursViewModel> GetTotalEmployeeHours(DateTime startDate, DateTime endDate, int employeeId)
        {
            var result = _totalEmployeeHoursService.GetEmployeeHoursTotal(startDate, endDate.AddDays(1), employeeId);
            var viewModel = result.MapCollection<HoursPerCompanyDao, EmployeeTotalHoursViewModel>((s, d) =>
            {
                var regularHours = s.TotalEmployeeHours.FirstOrDefault(x => x.Type == RateType.Regular);
                var overtime = s.TotalEmployeeHours.FirstOrDefault(x => x.Type == RateType.OverTime);
                var nightDifferential = s.TotalEmployeeHours.FirstOrDefault(x => x.Type == RateType.NightDifferential);

                d.RegularHours = regularHours != null ? regularHours.Hours : 0;
                d.TotalRegularHoursId = regularHours != null ? regularHours.TotalEmployeeHoursId : 0;                
                d.Overtime = overtime != null ? overtime.Hours : 0;
                d.TotalOvertimeId = overtime != null ? overtime.TotalEmployeeHoursId : 0;
                d.NightDifferential = nightDifferential != null ? nightDifferential.Hours : 0;
                d.TotalNightDifferentialId = nightDifferential != null ? nightDifferential.TotalEmployeeHoursId : 0;
            });

            return viewModel;
        }
    }
}