using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Payroll.Attributes;
using Payroll.Common.Enums;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models;
using Payroll.Models.Employee;
using Payroll.Repository.Interface;
using Payroll.Common.Extension;
using Payroll.Repository.Repositories;
using Payroll.Resources;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [DefaultAuthorize(Roles = "Admin,Manager")]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IWebService _webService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeLoanRepository _employeeLoanRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IEmployeeInfoHistoryRepository _employeeInfoHistoryRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IDeductionRepository _deductionRepository;
        private readonly IEmployeeDeductionService _employeeDeductionService;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public EmployeeController(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, IEmployeeInfoRepository employeeInfoRepository,
            ISettingRepository settingRepository, IPositionRepository positionRepository, IEmployeeLoanRepository employeeLoanRepository,
            IWebService webService, IDepartmentRepository departmentRepository, ILoanRepository loanRepository, IEmployeeInfoHistoryRepository employeeInfoHistoryRepository, IEmployeeLeaveRepository employeeLeaveRepository,
            ILeaveRepository leaveRepository, IDeductionRepository deductionRepository, IEmployeeDeductionService employeeDeductionService,
            IEmployeeWorkScheduleService employeeWorkScheduleService, IWorkScheduleRepository workScheduleRepository) 
        {
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _settingRepository = settingRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _positionRepository = positionRepository;
            _webService = webService;
            _employeeLoanRepository = employeeLoanRepository;
            _departmentRepository = departmentRepository;
            _loanRepository = loanRepository;
            _employeeInfoHistoryRepository = employeeInfoHistoryRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _leaveRepository = leaveRepository;
            _deductionRepository = deductionRepository;
            _employeeDeductionService = employeeDeductionService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _workScheduleRepository = workScheduleRepository;
        }

        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Index()
        {
            var employees = _employeeInfoRepository.Find(x => x.Employee.IsActive).ToList();
            var pagination = _webService.GetPaginationModel(Request, employees.Count);
            var viewModel = new EmployeeListViewModel
            {
                Employees = _webService.TakePaginationModel(employees, pagination),
                Pagination = pagination
            };

            return View(viewModel);
        }

        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult SearchEmployee(string query)
        {
            var result = new List<EmployeeInfo>();
            var firstNames = _employeeInfoRepository.Find(x => x.Employee.FirstName.Contains(query) && x.Employee.IsActive).ToList();
            var lastNames = _employeeInfoRepository.Find(x => x.Employee.LastName.Contains(query) && x.Employee.IsActive).ToList();
            var employeeCodes = _employeeInfoRepository.Find(x => x.Employee.EmployeeCode == query && x.Employee.IsActive).ToList();

            int id;
            if (int.TryParse(query, out id))
            {
                var employeeId = _employeeInfoRepository.Find(x => x.EmployeeId == id && x.Employee.IsActive).ToList();
                result.AddRange(employeeId);
            }
                
            result.AddRange(firstNames);
            result.AddRange(lastNames);
            result.AddRange(employeeCodes);

            ViewBag.SearchCriteria = query;

            var resultDistinct = result.Distinct();
            var pagination = _webService.GetPaginationModel(Request, resultDistinct.Count());
            var viewModel = new EmployeeListViewModel
            {
                Employees = _webService.TakePaginationModel(resultDistinct, pagination),
                Pagination = pagination
            };

            return View("Index", viewModel);
        }

        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Edit(int id)
        {
            var viewModel = GetEmployeeViewModel(_employeeInfoRepository.GetByEmployeeId(id));
            viewModel.IsPrivate = _settingRepository.GetSettingValue("IS_PRIVATE_COMPANY", "true") == "true";

            ViewBag.Title = "Edit Employee";
            ViewBag.FormAction = "/Employee/Edit";
            
            return View("Details", viewModel);
        }

        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Create()
        {
            var viewModel = GetEmployeeViewModel(new EmployeeInfo{ Employee = new Employee()});
            viewModel.IsPrivate = _settingRepository.GetSettingValue("IS_PRIVATE_COMPANY", "true") == "true";

            ViewBag.Title = "Create Employee";
            ViewBag.FormAction = "/Employee/Create";

            return View("Details", viewModel);
        }

        protected EmployeeInfoViewModel GetEmployeeViewModel(EmployeeInfo employeeInfo)
        {
            var viewModel = new EmployeeInfoViewModel();
            GetDropDowns(viewModel, employeeInfo.EmployeeId);

            if (employeeInfo != null)
            {
                viewModel.EmployeeInfo = employeeInfo;
                viewModel.ImagePath = employeeInfo.Employee.Picture != null ? Url.Content(employeeInfo.Employee.Picture) : "/Images/noimage.jpg";
                viewModel.PositionId = Convert.ToInt32(employeeInfo.PositionId);
                viewModel.Gender = employeeInfo.Employee.Gender;
                viewModel.EmploymentStatus = employeeInfo.EmploymentStatus;
            }

            return viewModel;
        }

        protected void GetDropDowns(EmployeeInfoViewModel viewModel, int employeeId)
        {
            var positions = _positionRepository.Find(x => x.IsActive).Select(x => new SelectListItem
            {
                Text = x.PositionName,
                Value = x.PositionId.ToString()
            }).ToList();

            var paymentFrequencies = new List<SelectListItem>();
            foreach (FrequencyType frquency in Enum.GetValues(typeof(FrequencyType)))
            {
                paymentFrequencies.Add(new SelectListItem
                {
                    Text = frquency.ToString(),
                    Value = ((int)frquency).ToString()
                });
            }

            var genders = new List<SelectListItem>();
            foreach (Gender gender in Enum.GetValues(typeof(Gender)))
            {
                genders.Add(new SelectListItem
                {
                    Text = gender.ToString(),
                    Value = ((int)gender).ToString()
                });
            }

            var employmentStatus = new List<SelectListItem>();
            foreach (EmploymentStatus status in Enum.GetValues(typeof(EmploymentStatus)))
            {
                employmentStatus.Add(new SelectListItem
                {
                    Text = status.ToString(),
                    Value = ((int)status).ToString()
                });
            }

            positions.Insert(0, new SelectListItem { Text = "Select Position", Value = "0" });
            paymentFrequencies.Insert(0, new SelectListItem { Text = "Select Payment Frequency", Value = "0" });


            //For employee department association
            var departments = _departmentRepository.Find(x => x.IsActive).Select(x => new EmployeeDepartmentViewModel
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName
            }).ToList();

            if (employeeId > 0)
            {
                var employeeDepartments = _employeeRepository.GetDepartments(employeeId);
                foreach (var employeeDepartment in employeeDepartments)
                {
                    var department = departments.FirstOrDefault(x => x.DepartmentId == employeeDepartment.DepartmentId);
                    if (department != null)
                    {
                        department.Checked = true;
                    }
                }
            }

            //get employee work schedule
            if (employeeId > 0)
            {
                var employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employeeId);
                if (employeeWorkSchedule != null)
                {
                    viewModel.WorkSchedule = employeeWorkSchedule.WorkSchedule;
                    viewModel.WorkScheduleId = employeeWorkSchedule.WorkSchedule.WorkScheduleId;
                }
            }

            //Get Employee Deductions
            var employeeDeduction = _employeeDeductionService.GetEmployeeDeduction(employeeId);
            var deductions = _deductionRepository.GetAllActive().ToList();
            var deductionsViewModel = new List<EmployeeDeductionViewModel>();
            if (deductions.Any())
            {
                deductionsViewModel = deductions.MapCollection<Deduction, EmployeeDeductionViewModel>((s, d) =>
                {
                    var existingEmployeeDeduction = employeeDeduction.FirstOrDefault(x => x.IsActive && x.DeductionId == s.DeductionId && x.EmployeeId == employeeId);
                    if (existingEmployeeDeduction != null)
                    {
                        d.IsChecked = true;
                        d.Amount = existingEmployeeDeduction.Amount;
                    }
                }).ToList();
            }


            viewModel.EmployeeDeductions = deductionsViewModel;
            viewModel.Positions = positions;
            viewModel.Departments = departments;
            viewModel.PaymentFrequencies = paymentFrequencies;
            viewModel.Genders = genders;
            viewModel.EmploymentStatuses = employmentStatus;
        }

        public virtual ActionResult WorkSchedules(int workScheduleId)
        {
            var workSchedules = _workScheduleRepository.GetAllActive();
            ViewBag.WorkScheduleId = workScheduleId;
            return View(workSchedules);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Create(EmployeeInfoViewModel viewModel)
        {
            //validate birthdate
            if (!viewModel.EmployeeInfo.Employee.BirthDate.IsValidBirthDate())
            {
                GetDropDowns(viewModel, viewModel.EmployeeInfo.EmployeeId);
                ModelState.AddModelError("", ErrorMessages.INVALID_DATE);
                return View("Details", viewModel);
            }

            //validate employee code
            var existingEmployeeCode = _employeeRepository.Find(x => x.EmployeeCode == viewModel.EmployeeInfo.Employee.EmployeeCode && x.IsActive);
            if (existingEmployeeCode != null)
            {
                GetDropDowns(viewModel, viewModel.EmployeeInfo.EmployeeId);
                ModelState.AddModelError("", ErrorMessages.USED_EMPLOYEECODE);
                return View("Details", viewModel);
            }

            var employee = viewModel.EmployeeInfo.Employee.MapItem<Employee>();
            employee.Gender = viewModel.Gender;
            var employeeInfo = viewModel.EmployeeInfo;
            employeeInfo.Employee = employee;
            employeeInfo.PositionId = viewModel.PositionId != 0 ? viewModel.PositionId : (int?) null;
            employeeInfo.SalaryFrequency = (FrequencyType)viewModel.PaymentFrequency;
            employeeInfo.EmploymentStatus = viewModel.EmploymentStatus;

            var newEmployee = _employeeInfoRepository.Add(employeeInfo).Employee;
            _employeeInfoHistoryRepository.Add(employeeInfo.MapItem<EmployeeInfoHistory>());

            _unitOfWork.Commit();

            //departments
            var departments = viewModel.CheckedDepartments != null
                            ? viewModel.CheckedDepartments.Split(',').Select(Int32.Parse)
                            : new List<int>();

            _employeeRepository.UpdateDepartment(departments, employee.EmployeeId);

            //employee deductions
            if (!String.IsNullOrEmpty(viewModel.CheckedEmployeeDeductions))
            {
                var employeeDeductions = viewModel.CheckedEmployeeDeductions != null
                        ? JsonConvert.DeserializeObject<List<EmployeeDeduction>>(viewModel.CheckedEmployeeDeductions)
                        : new List<EmployeeDeduction>();
                _employeeDeductionService.UpdateEmployeeDeduction(employeeDeductions, viewModel.EmployeeInfo.EmployeeId);
            }

            //work schedule
            if (viewModel.WorkScheduleId > 0)
            {
                _employeeWorkScheduleService.Add(viewModel.WorkScheduleId, employee.EmployeeId);
            }
            
            _unitOfWork.Commit();

            //upload the picture and update the record
            var imagePath = UploadImage(newEmployee.EmployeeId);
            if (!String.IsNullOrEmpty(imagePath))
            {
                _employeeRepository.Update(newEmployee);
                newEmployee.Picture = imagePath;
                _unitOfWork.Commit();
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Edit(EmployeeInfoViewModel viewModel)
        {
            //validate birthdate
            if (!viewModel.EmployeeInfo.Employee.BirthDate.IsValidBirthDate())
            {
                GetDropDowns(viewModel, viewModel.EmployeeInfo.EmployeeId);
                ModelState.AddModelError("", ErrorMessages.INVALID_DATE);
                return View("Details", viewModel);
            }

            var employeeInfo = _employeeInfoRepository.GetById(viewModel.EmployeeInfo.EmploymentInfoId);
            _employeeInfoRepository.Update(employeeInfo);

            employeeInfo.Allowance = viewModel.EmployeeInfo.Allowance;
            employeeInfo.CustomDate1 = viewModel.EmployeeInfo.CustomDate1;
            employeeInfo.DateHired = viewModel.EmployeeInfo.DateHired;
            employeeInfo.Dependents = viewModel.EmployeeInfo.Dependents;
            employeeInfo.EmploymentStatus = viewModel.EmployeeInfo.EmploymentStatus;
            employeeInfo.GSIS = viewModel.EmployeeInfo.GSIS;
            employeeInfo.Married = viewModel.EmployeeInfo.Married;
            employeeInfo.PAGIBIG = viewModel.EmployeeInfo.PAGIBIG;
            employeeInfo.EmploymentStatus = viewModel.EmployeeInfo.EmploymentStatus;
            employeeInfo.PhilHealth = viewModel.EmployeeInfo.PhilHealth;
            employeeInfo.SSS = viewModel.EmployeeInfo.SSS;
            employeeInfo.Salary = viewModel.EmployeeInfo.Salary;
            employeeInfo.TIN = viewModel.EmployeeInfo.TIN;
            employeeInfo.EmploymentStatus = viewModel.EmploymentStatus;

            employeeInfo.PositionId = viewModel.PositionId;
            employeeInfo.SalaryFrequency = (FrequencyType)viewModel.PaymentFrequency;
            var picture = employeeInfo.Employee.Picture;
            employeeInfo.Employee.InjectFrom( viewModel.EmployeeInfo.Employee);
            employeeInfo.Employee.Gender = viewModel.Gender;
            employeeInfo.Employee.Picture = picture;

            var employeeDeductions = viewModel.CheckedEmployeeDeductions != null
                                    ? JsonConvert.DeserializeObject<List<EmployeeDeduction>>(viewModel.CheckedEmployeeDeductions)
                                    : new List<EmployeeDeduction>();
            _employeeDeductionService.UpdateEmployeeDeduction(employeeDeductions, viewModel.EmployeeInfo.EmployeeId);

            var departments = viewModel.CheckedDepartments != null
                            ? viewModel.CheckedDepartments.Split(',').Select(Int32.Parse)
                            : new List<int>();

            //work schedules
            _employeeWorkScheduleService.Update(viewModel.WorkScheduleId, viewModel.EmployeeInfo.EmployeeId);

            _employeeRepository.UpdateDepartment(departments, viewModel.EmployeeInfo.EmployeeId);
            _employeeInfoHistoryRepository.Add(employeeInfo.MapItem<EmployeeInfoHistory>());
            _unitOfWork.Commit();

            //upload the picture and update the record
            var imagePath = UploadImage(viewModel.EmployeeInfo.EmployeeId);
            if (!String.IsNullOrEmpty(imagePath))
            {
                var employee = employeeInfo.Employee;
                _employeeRepository.Update(employee);
                employee.Picture = imagePath;
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [OverrideAuthorizeAttribute(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult Delete(int id)
        {
            var employee = _employeeRepository.GetById(id);
            _employeeRepository.Update(employee);
            employee.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        protected virtual string UploadImage(int id)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var imageUploadPath = _settingRepository.GetSettingValue("EMPLOYEE_IMAGE_PATH");
                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            var imagePath = Path.Combine(Server.MapPath(imageUploadPath), String.Format("{0}.jpg", id));
                            file.SaveAs(imagePath);

                            return String.Format("{0}/{1}", imageUploadPath, String.Format("{0}.jpg", id));
                        }
                    }
                }
                catch (Exception ex)
                {
                    //handle exception here
                }
            }

            return "";
        }

        public virtual ActionResult EmployeeLoans()
        {
            var result = _employeeLoanRepository.GetActiveEmployeeLoans();
            return View(result);
        }

        public virtual ActionResult CreateEmployeeLoan()
        {
            var viewModel = new EmployeeLoanViewModel();
            viewModel.Loans = _loanRepository.Find(x => x.IsActive).Select(x => new SelectListItem
            {
                Value = x.LoanId.ToString(),
                Text = x.LoanName
            }).ToList();

            var dayOfWeeks = new List<SelectListItem>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                dayOfWeeks.Add(new SelectListItem
                {
                    Text = dayOfWeek.ToString(),
                    Value = ((int)dayOfWeek).ToString()
                });
            }
            viewModel.WeeklyPaymentDayOfWeekList = dayOfWeeks;


            var loanPaymentFrequencies = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = FrequencyType.Weekly.ToString(),
                    Value =((int)FrequencyType.Weekly).ToString() 
                },
                new SelectListItem
                {
                    Text = FrequencyType.SemiMonthly.ToString(),
                    Value =((int)FrequencyType.SemiMonthly).ToString() 
                },
                new SelectListItem
                {
                    Text = FrequencyType.Monthly.ToString(),
                    Value =((int)FrequencyType.Monthly).ToString() 
                }
            };

            viewModel.PaymentFrequencies = loanPaymentFrequencies;

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult CreateEmployeeLoan(EmployeeLoanViewModel viewModel)
        {
            var employeeLoan = viewModel.MapItem<EmployeeLoanViewModel, EmployeeLoan>((s, d) =>
            {
                d.IsActive = true;
            });
            _employeeLoanRepository.Add(employeeLoan);

            _unitOfWork.Commit();
            return RedirectToAction("EmployeeLoans");
        }

        public virtual ActionResult EditEmployeeLoan(int id)
        {
            var employeeLoan = _employeeLoanRepository.GetById(id);

            var viewModel = employeeLoan.MapItem<EmployeeLoanViewModel>();
            viewModel.EmployeeName = employeeLoan.Employee.FullName;

            viewModel.Loans = _loanRepository.Find(x => x.IsActive).Select(x => new SelectListItem
            {
                Value = x.LoanId.ToString(),
                Text = x.LoanName
            }).ToList();

            var dayOfWeeks = new List<SelectListItem>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                dayOfWeeks.Add(new SelectListItem
                {
                    Text = dayOfWeek.ToString(),
                    Value = ((int)dayOfWeek).ToString()
                });
            }
            viewModel.WeeklyPaymentDayOfWeekList = dayOfWeeks;


            var loanPaymentFrequencies = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = FrequencyType.Weekly.ToString(),
                    Value =((int)FrequencyType.Weekly).ToString() 
                },
                new SelectListItem
                {
                    Text = FrequencyType.SemiMonthly.ToString(),
                    Value =((int)FrequencyType.SemiMonthly).ToString() 
                },
                new SelectListItem
                {
                    Text = FrequencyType.Monthly.ToString(),
                    Value =((int)FrequencyType.Monthly).ToString() 
                }
            };

            viewModel.PaymentFrequencies = loanPaymentFrequencies;
            
            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult EditEmployeeLoan(EmployeeLoanViewModel viewModel)
        {
            var model = _employeeLoanRepository.GetById(viewModel.EmployeeLoanId);
            _employeeLoanRepository.Update(model);
            model.InjectFrom(viewModel);
            model.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("EmployeeLoans");
        }

        //employee loan should only be deleted if there are NO outstanding payment
        public virtual ActionResult DeleteEmployeeLoan(int id)
        {
            var employeeLoan = _employeeLoanRepository.GetById(id);
            _employeeLoanRepository.Update(employeeLoan);
            employeeLoan.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("EmployeeLoans");
        }


        #region EmployeeLeave
        [Authorize(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult EmployeeLeaves(int month, int year)
        {
            var employeeLeaves = _employeeLeaveRepository.GetEmployeeLeavesByDate(month, year).ToList();
            var employees = new List<EmployeeLeaveViewModel>();
            
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            while (startDate <= endDate)
            {
                employees.Add(new EmployeeLeaveViewModel
                {
                    Date = startDate,
                    Employees = employeeLeaves.Where(x => x.Date == startDate).ToList()
                });
                startDate = startDate.AddDays(1);
            }

            var years = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = DateTime.Now.AddYears(-1).Year.ToString(),
                    Value = DateTime.Now.AddYears(-1).Year.ToString(),
                },
                new SelectListItem
                {
                    Text = DateTime.Now.Year.ToString(),
                    Value = DateTime.Now.Year.ToString(),
                },
                new SelectListItem
                {
                    Text = DateTime.Now.AddYears(1).Year.ToString(),
                    Value = DateTime.Now.AddYears(1).Year.ToString(),
                }
            };

            var viewModel = new EmployeeLeaveListViewModel
            {
                Employees = employees,
                Month = month,
                Year = year,
                Years = years
            };

            //should display the employee leaves per month
            //should have a calendar wihch marks all the employee leaves for the month
            //upon double click or click, display a modal which displays the employee name
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Manager,Encoder")]
        public virtual ActionResult CreateEmployeeLeave()
        {

            var leaves = _leaveRepository.Find(x => x.IsActive)
                                  .Select(x => new SelectListItem
                                    {
                                        Value = x.LeaveId.ToString(),
                                        Text = x.LeaveName
                                    });
            var hours = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "8",
                    Text = "Whole Day"
                },
                new SelectListItem
                {
                    Value = "4",
                    Text = "Half Day"
                },
                new SelectListItem
                {
                    Value = "-1",
                    Text = "Specify Hours"
                }
            };

            var viewModel = new EmployeeLeaveCreateViewModel
            {
                Date = DateTime.Now.AddDays(1),
                Leaves = leaves,
                LeaveHours = hours
            };

            return View(viewModel);
        }

        public virtual ActionResult EditEmployeeLeave(int id)
        {
            var leaves = _leaveRepository.Find(x => x.IsActive)
                                  .Select(x => new SelectListItem
                                  {
                                      Value = x.LeaveId.ToString(),
                                      Text = x.LeaveName
                                  });
            var hours = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "8",
                    Text = "Whole Day"
                },
                new SelectListItem
                {
                    Value = "4",
                    Text = "Half Day"
                },
                new SelectListItem
                {
                    Value = "-1",
                    Text = "Specify Hours"
                }
            };

            var employeeLeave = _employeeLeaveRepository.GetById(id);

            var viewModel = new EmployeeLeaveCreateViewModel
            {
                Date = employeeLeave.Date,
                Leaves = leaves,
                MarkAsApproved = employeeLeave.LeaveStatus == LeaveStatus.Approved,
                LeaveHours = hours,
                Hours = employeeLeave.Hours,
                SpecifiedHours = employeeLeave.Hours,
                EmployeeId = employeeLeave.EmployeeId,
                EmployeeName = employeeLeave.Employee.FullName,
                LeaveId = employeeLeave.LeaveId,
                Reason = employeeLeave.Reason,
                EmployeeLeaveId = id
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult CreateEmployeeLeave(EmployeeLeaveCreateViewModel viewModel)
        {
            var employeeLeave = viewModel.MapItem<EmployeeLeave>();
            employeeLeave.Hours = viewModel.Hours > 0 ? viewModel.Hours : viewModel.SpecifiedHours;
            employeeLeave.IsActive = true;
            employeeLeave.LeaveStatus = LeaveStatus.Pending;
            employeeLeave.ApprovedBy = null;

            if (viewModel.MarkAsApproved)
            {
                employeeLeave.ApprovedBy = User.Identity.GetUserId();
                employeeLeave.LeaveStatus = LeaveStatus.Approved;
            }
                

            _employeeLeaveRepository.Add(employeeLeave);
            _unitOfWork.Commit();
            return RedirectToAction("EmployeeLeaves", new { month = DateTime.Now.Month, year = DateTime.Now.Year});
        }

        [HttpPost]
        public virtual ActionResult EditEmployeeLeave(EmployeeLeaveCreateViewModel viewModel)
        {
            var employeeLeave = _employeeLeaveRepository.GetById(viewModel.EmployeeLeaveId);
            _employeeLeaveRepository.Update(employeeLeave);

            employeeLeave.Hours = viewModel.Hours > 0 ? viewModel.Hours : viewModel.SpecifiedHours;
            employeeLeave.Date = viewModel.Date;
            employeeLeave.Reason = viewModel.Reason;
            employeeLeave.LeaveId = viewModel.LeaveId;

            if (viewModel.MarkAsApproved)
            {
                employeeLeave.ApprovedBy = User.Identity.GetUserId();
                employeeLeave.LeaveStatus = LeaveStatus.Approved;
            }


            _unitOfWork.Commit();
            return RedirectToAction("EmployeeLeaves", new { month = DateTime.Now.Month, year = DateTime.Now.Year });
        }

        public virtual ActionResult ApproveRejectLeave(int id, LeaveStatus status)
        {
            var employeeLeave = _employeeLeaveRepository.GetById(id);
            _employeeLeaveRepository.Update(employeeLeave);
            employeeLeave.LeaveStatus = status;
            _unitOfWork.Commit();

            return RedirectToAction("EmployeeLeaves", new { month = DateTime.Now.Month, year = DateTime.Now.Year });
        }

        public virtual ActionResult DeleteEmployeeLeave(int id)
        {
            var employeeLeave = _employeeLeaveRepository.GetById(id);
            _employeeLeaveRepository.Update(employeeLeave);
            employeeLeave.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("EmployeeLeaves", new { month = DateTime.Now.Month, year = DateTime.Now.Year });
        }
        #endregion

        #region Employee Deductions

        //public virtual ActionResult CreateEmployeeDeductions(int employeeId)
        //{
        //    var deductions = _deductionRepository.GetAllActive().Select(x => new SelectListItem
        //    {
        //        Text = x.DeductionName,
        //        Value = x.DeductionId.ToString()
        //    });

        //    var viewModel = new EmployeeDeductionViewModel
        //    {
        //        Deductions = deductions,
        //        EmployeeId = employeeId
        //    };

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public virtual PartialViewResult CreateEmployeeDeductions(EmployeeDeductionViewModel viewModel)
        //{
        //    var employeeDeduction = viewModel.MapItem<EmployeeDeduction>();
        //    _employeeDeductionService.Add(employeeDeduction);
        //    _unitOfWork.Commit();

        //    var employeeDeductions = _employeeDeductionService.GetEmployeeDeduction(viewModel.EmployeeId);
        //    return PartialView("_EmployeeDeductions", employeeDeductions);
        //}

        #endregion
    }
}