using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Enums;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Maintenance;
using Payroll.Repository.Interface;
using Payroll.Repository.Models.Employee;
using Payroll.Resources;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingRepository _settingRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IEmployeeMachineService _emplyeeMachineService;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IWebService _webService;

        public MaintenanceController(IUnitOfWork unitOfWork, ISettingRepository settingRepository, IPositionRepository positionRepository, IPaymentFrequencyRepository paymentFrequencyRepository, 
            IHolidayRepository holidayRepository, IDepartmentRepository departmentRepository, ILeaveRepository leaveRepository, ILoanRepository loanRepository, 
            IMachineRepository machineRepository, IWebService webService,
            IEmployeeMachineService emplyeeMachineService, IWorkScheduleRepository workScheduleRepository)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
            _positionRepository = positionRepository;
            _paymentFrequencyRepository = paymentFrequencyRepository;
            _departmentRepository = departmentRepository;
            _holidayRepository = holidayRepository;
            _leaveRepository = leaveRepository;
            _loanRepository = loanRepository;
            _machineRepository = machineRepository;
            _webService = webService;
            _emplyeeMachineService = emplyeeMachineService;
            _workScheduleRepository = workScheduleRepository;
        }

        #region Positions
        public virtual ActionResult Position()
        {
            var positions = _positionRepository.Find(x => x.IsActive);
            return View(positions);
        }

        public virtual ActionResult CreatePosition()
        {
            return View(new Position());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreatePosition(Position model)
        {
            model.IsActive = true;
            _positionRepository.Add(model);
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }
        
        public virtual ActionResult EditPosition(int id)
        {
            var position = _positionRepository.GetById(id);
            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditPosition(Position model)
        {
            var position = new Position { PositionId = model.PositionId };
            _positionRepository.Update(position);
            position.InjectFrom(model);
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }

        public virtual ActionResult DeletePosition(int id)
        {
            var position = _positionRepository.GetById(id);
            _positionRepository.Update(position);
            position.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }
        #endregion

        #region Payment Frequency
        public virtual ActionResult PaymentFrequency()
        {
            var paymentFrequencies = _paymentFrequencyRepository.Find(x => x.IsActive).ToList();
            return View(paymentFrequencies);
        }

        public virtual ActionResult DeletePaymentFrequency(int id)
        {
            var paymentFrequency = _paymentFrequencyRepository.GetById(id);
            _paymentFrequencyRepository.Update(paymentFrequency);
            paymentFrequency.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("PaymentFrequency");
        }

        public virtual ActionResult CreatePaymentFrequency()
        {
            
            var frequencies = new List<SelectListItem>();
            foreach (FrequencyType val in Enum.GetValues(typeof(FrequencyType)))
            {
                frequencies.Add(new SelectListItem
                {
                    Text = val.ToString(),
                    Value = ((int)val).ToString()
                });
            }



            var viewModel = new PaymentFrequencyViewModel
            {
                Frequencies = frequencies,
                DayOfWeeks = GetDayOfWeeks(),
                PaymentFrequency = new PaymentFrequency { FrequencyId = 1, MonthlyStartDay = 15, MonthlyEndDay = 30, WeeklyStartDayOfWeek = 3}
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreatePaymentFrequency(PaymentFrequencyViewModel viewModel)
        {
            var paymentFrequency = viewModel.PaymentFrequency.MapItem<PaymentFrequency>();
            paymentFrequency.IsActive = true;
            _paymentFrequencyRepository.Add(paymentFrequency);
            _unitOfWork.Commit();

            return RedirectToAction("PaymentFrequency");
        }
        #endregion

        #region Departments
        public virtual ActionResult Department()
        {
            var departments = _departmentRepository.Find(x => x.IsActive);
            return View(departments);
        }

        public virtual ActionResult EditDepartment(int id)
        {
            var department = _departmentRepository.GetById(id);
            return View(department);
        }

        public virtual ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateDepartment(Department department)
        {
            _departmentRepository.Add(department);
            _unitOfWork.Commit();
            return RedirectToAction("Department");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditDepartment(Department model)
        {
            var department = new Department { DepartmentId = model.DepartmentId };
            _departmentRepository.Update(department);
            department.InjectFrom(model);
            _unitOfWork.Commit();
            return RedirectToAction("Department");
        }

        public virtual ActionResult DeleteDepartment(int id)
        {
            var department = _departmentRepository.GetById(id);
            _departmentRepository.Update(department);
            department.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Department");
        }
        #endregion

        #region Holiday
        public virtual ActionResult Holiday()
        {
            var holidays = _holidayRepository.GetHolidaysByCurrentYear();
            return View(holidays);
        }

        public virtual ActionResult CreateHoliday()
        {
            return View(new Holiday());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateHoliday(Holiday holiday)
        {
            holiday.Year = DateTime.Now.Year;
            holiday.IsActive = true;

            _holidayRepository.Add(holiday);
            _unitOfWork.Commit();

            return RedirectToAction("Holiday");
        }

        public virtual ActionResult EditHoliday(int id)
        {
            var holiday = _holidayRepository.GetById(id);
            return View(holiday);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditHoliday(Holiday model)
        {
            var holiday = _holidayRepository.GetById(model.HolidayId);
            _holidayRepository.Update(holiday);

            holiday.InjectFrom(model);
            holiday.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("Holiday");
        }

        public virtual ActionResult DeleteHoliday(int id)
        {
            var holiday = _holidayRepository.GetById(id);
            _holidayRepository.Update(holiday);
            holiday.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Holiday");
        }
        #endregion

        #region Leaves
        public virtual ActionResult Leave()
        {
            var leaves = _leaveRepository.Find(x => x.IsActive);
            ViewBag.SupportRefundable = _settingRepository.GetSettingValue("SUPPORT_REFUNDABLE_LEAVE", "false");

            return View(leaves);
        }

        public virtual ActionResult CreateLeave()
        {
            ViewBag.SupportRefundable = _settingRepository.GetSettingValue("SUPPORT_REFUNDABLE_LEAVE", "false");
            return View(new Leave());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateLeave(Leave leave)
        {
            leave.IsActive = true;

            _leaveRepository.Add(leave);
            _unitOfWork.Commit();

            return RedirectToAction("Leave");
        }

        public virtual ActionResult EditLeave(int id)
        {
            ViewBag.SupportRefundable = _settingRepository.GetSettingValue("SUPPORT_REFUNDABLE_LEAVE", "false");
            var leave = _leaveRepository.GetById(id);
            return View(leave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditLeave(Leave model)
        {
            var leave = _leaveRepository.GetById(model.LeaveId);
            _leaveRepository.Update(leave);

            leave.InjectFrom(model);
            leave.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("Leave");
        }

        public virtual ActionResult DeleteLeave(int id)
        {
            var leave = _leaveRepository.GetById(id);
            _leaveRepository.Update(leave);
            leave.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Leave");
        }
        #endregion

        #region Loans
        public virtual ActionResult Loan()
        {
            var loan = _loanRepository.Find(x => x.IsActive);
            return View(loan);
        }

        public virtual ActionResult CreateLoan()
        {
            var loanPeriods = new List<SelectListItem> {new SelectListItem {Text = "Not Specified", Value = "0"}};
            foreach (LoanPaymentPeriod val in Enum.GetValues(typeof(Common.Enums.LoanPaymentPeriod)))
            {
                loanPeriods.Add(new SelectListItem
                {
                    Text = val.ToString(),
                    Value = ((int)val).ToString()
                });
            } 
            
            var viewModel = new LoanViewModel
            {
                Loan = new Loan(),
                LoanPaymentPeriod = loanPeriods
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateLoan(Loan loan)
        {
            loan.IsActive = true;

            _loanRepository.Add(loan);
            _unitOfWork.Commit();

            return RedirectToAction("Loan");
        }

        public virtual ActionResult EditLoan(int id)
        {
            var loanPeriods = new List<SelectListItem> { new SelectListItem { Text = "Not Specified", Value = "0" } };
            foreach (LoanPaymentPeriod val in Enum.GetValues(typeof(LoanPaymentPeriod)))
            {
                loanPeriods.Add(new SelectListItem
                {
                    Text = val.ToString(),
                    Value = ((int)val).ToString()
                });
            } 

            var loan = _loanRepository.GetById(id);
            var viewModel = new LoanViewModel
            {
                Loan = loan,
                LoanPaymentPeriod = loanPeriods
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditLoan(LoanViewModel viewModel)
        {
            var loan = _loanRepository.GetById(viewModel.Loan.LoanId);
            _loanRepository.Update(loan);

            loan.InjectFrom(viewModel.Loan);
            loan.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("Loan");
        }

        public virtual ActionResult DeleteLoan(int id)
        {
            var loan = _loanRepository.GetById(id);
            _loanRepository.Update(loan);
            loan.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Loan");
        }
        #endregion

        #region Machines
        public virtual ActionResult Machine()
        {
            var machine = _machineRepository.Find(x => x.IsActive);
            return View(machine);
        }

        public virtual ActionResult CreateMachine()
        {
            return View(new Machine());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateMachine(Machine machine)
        {
            //Validate if machine already exists
            var existingMaching = _machineRepository.Find(x => x.IpAddress == machine.IpAddress && x.IsActive).FirstOrDefault();
            if (existingMaching != null)
            {
                ModelState.AddModelError("", ErrorMessages.MACHINE_EXISTS);
                return View(machine);
            }

            machine.IsActive = true;

            _machineRepository.Add(machine);
            _unitOfWork.Commit();

            return RedirectToAction("Machine");
        }

        public virtual ActionResult SearchEmployee(string query, int id)
        {
            var employees = _emplyeeMachineService.GetEmployees(id).OrderBy(x => x.Enrolled);

            var result = new List<EmployeeMachineDao>();
            var firstNames = employees.Where(x => x.FirstName.Contains(query)).ToList();
            var lastNames = employees.Where(x => x.LastName.Contains(query)).ToList();
            var codes = employees.Where(x => x.EmployeeCode == query).ToList();
            
            int employeeId;
            if (int.TryParse(query, out employeeId))
            {
                var employee = employees.Where(x => x.EmployeeId == employeeId).ToList();
                result.AddRange(employee);
            }


            result.AddRange(firstNames);
            result.AddRange(lastNames);
            result.AddRange(codes);

            ViewBag.SearchCriteria = query;

            var resultDistinct = result.Distinct();
            var pagination = _webService.GetPaginationModel(Request, resultDistinct.Count());
            var viewModel = new EmployeeMachineViewModel
            {
                Employees = _webService.TakePaginationModel(resultDistinct, pagination),
                Pagination = pagination,
                MachineNumber = id
            };

            return View("EnrolledEmployees", viewModel);
        }

        public virtual ActionResult EnrolledEmployees(int id, string ipAddress)
        {
            var employees = _emplyeeMachineService.GetEmployees(id).OrderBy(x => x.Enrolled);
            var pagination = _webService.GetPaginationModel(Request, employees.Count());
            var viewModel = new EmployeeMachineViewModel
            {
                Employees = _webService.TakePaginationModel(employees, pagination),
                Pagination = pagination,
                MachineNumber = id,
            };

            return View(viewModel);
        }

        public virtual ActionResult EditMachine(int id)
        {
            var model = _machineRepository.GetById(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditMachine(Machine model)
        {
            //Validate if machine already exists
            var existingMaching = _machineRepository.Find(x => x.IpAddress == model.IpAddress && x.IsActive && x.MachineId != model.MachineId).FirstOrDefault();
            if (existingMaching != null)
            {
                ModelState.AddModelError("", ErrorMessages.MACHINE_EXISTS);
                return View(model);
            }

            var machine = _machineRepository.GetById(model.MachineId);
            _machineRepository.Update(machine);

            machine.IpAddress = model.IpAddress;
            machine.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("Machine");
        }

        public virtual ActionResult DeleteMachine(int id)
        {
            var machine = _machineRepository.GetById(id);
            _machineRepository.Update(machine);
            machine.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Machine");
        }

        #endregion

        #region Work Schedule
        public virtual ActionResult WorkSchedule()
        {
            var workSchedules = _workScheduleRepository.GetAllActive();
            return View(workSchedules);
        }

        public virtual ActionResult CreateWorkSchedule()
        {
            var viewModel = new WorkScheduleViewModel
            {
                WeekList = GetDayOfWeeks()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateWorkSchedule(WorkScheduleViewModel viewModel)
        {
            var workSchedule = viewModel.MapItem<WorkSchedule>();
            workSchedule.TimeEnd = viewModel.TimeEnd.TimeOfDay;
            workSchedule.TimeStart = viewModel.TimeStart.TimeOfDay;

            _workScheduleRepository.Add(workSchedule);
            _unitOfWork.Commit();

            return RedirectToAction("WorkSchedule");
        }

        public virtual ActionResult EditWorkSchedule(int id)
        {
            var workSchedule = _workScheduleRepository.GetById(id);
            var viewModel = workSchedule.MapItem<WorkScheduleViewModel>();
            viewModel.WeekList = GetDayOfWeeks();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditWorkSchedule(WorkScheduleViewModel viewModel)
        {
            var workSchedule = _workScheduleRepository.GetById(viewModel.WorkScheduleId);
            _workScheduleRepository.Update(workSchedule);

            workSchedule.InjectFrom(viewModel);
            workSchedule.TimeEnd = viewModel.TimeEnd.TimeOfDay;
            workSchedule.TimeStart = viewModel.TimeStart.TimeOfDay;
            workSchedule.IsActive = true;


            _unitOfWork.Commit();
            return RedirectToAction("WorkSchedule");
        }

        public virtual ActionResult DeleteWorkSchedule(int id)
        {
            var workSchedule = _workScheduleRepository.GetById(id);
            _workScheduleRepository.Update(workSchedule);
            workSchedule.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("WorkSchedule");
        }
        #endregion

        private IEnumerable<SelectListItem> GetDayOfWeeks()
        {
            var dayOfWeeks = new List<SelectListItem>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                dayOfWeeks.Add(new SelectListItem
                {
                    Text = dayOfWeek.ToString(),
                    Value = ((int)dayOfWeek).ToString()
                });
            }

            return dayOfWeeks;
        }
    }
}