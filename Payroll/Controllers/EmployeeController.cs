using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Enums;
using Payroll.Common.Helpers;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models;
using Payroll.Models.Employee;
using Payroll.Repository.Interface;
using Payroll.Common.Extension;
using Payroll.Resources;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IWebService _webService;
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeController(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, IEmployeeInfoRepository employeeInfoRepository,
            ISettingRepository settingRepository, IPositionRepository positionRepository,
            IWebService webService, IPaymentFrequencyRepository paymentFrequencyRepository, IDepartmentRepository departmentRepository)
        {
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _settingRepository = settingRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _positionRepository = positionRepository;
            _webService = webService;
            _paymentFrequencyRepository = paymentFrequencyRepository;
            _departmentRepository = departmentRepository;
        }

        public virtual ActionResult Index()
        {
            var employees = _employeeRepository.Find(x => x.IsActive).ToList();
            var pagination = _webService.GetPaginationModel(Request, employees);

            return View(pagination);
        }

        public virtual ActionResult SearchEmployee(string query)
        {
            var firstNames = _employeeRepository.Find(x => x.FirstName.Contains(query) && x.IsActive).ToList();
            var lastNames = _employeeRepository.Find(x => x.LastName.Contains(query) && x.IsActive).ToList();
            var employeeCodes = _employeeRepository.Find(x => x.EmployeeCode.Contains(query) && x.IsActive).ToList();

            var result = new List<Employee>();
            result.AddRange(firstNames);
            result.AddRange(lastNames);
            result.AddRange(employeeCodes);

            ViewBag.SearchCriteria = query;
            var pagination = _webService.GetPaginationModel(Request, result.Distinct());
            return View("Index", pagination);
        }

        public virtual ActionResult Edit(int id)
        {
            var viewModel = GetEmployeeViewModel(_employeeInfoRepository.GetByEmployeeId(id));
            ViewBag.Title = "Edit Employee";
            ViewBag.FormAction = "/Employee/Edit";
            
            return View("Details", viewModel);
        }

        public virtual ActionResult Create()
        {
            var viewModel = GetEmployeeViewModel(new EmployeeInfo{ Employee = new Employee()});
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
                viewModel.PaymentFrequency = Convert.ToInt32(employeeInfo.PaymentFrequencyId);
                viewModel.Gender = employeeInfo.Employee.Gender;
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

            var paymentFrequencies = _paymentFrequencyRepository.Find(x => x.IsActive).Select(x => new SelectListItem
            {
                Text = x.Frequency.FrequencyName,
                Value = x.FrequencyId.ToString()
            }).ToList();

            var genders = new List<SelectListItem>();
            foreach (Gender gender in Enum.GetValues(typeof(Gender)))
            {
                genders.Add(new SelectListItem
                {
                    Text = gender.ToString(),
                    Value = ((int)gender).ToString()
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

            viewModel.Positions = positions;
            viewModel.Departments = departments;
            viewModel.PaymentFrequencies = paymentFrequencies;
            viewModel.Genders = genders;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(EmployeeInfoViewModel viewModel)
        {
            //validate birthdate
            if (!viewModel.EmployeeInfo.Employee.BirthDate.IsValidBirthDate())
            {
                GetDropDowns(viewModel, viewModel.EmployeeInfo.EmployeeId);
                ModelState.AddModelError("", ErrorMessages.INVALID_DATE);
                return View("Details", viewModel);
            }

            var employee = viewModel.EmployeeInfo.Employee.MapItem<Employee>();
            employee.Gender = viewModel.Gender;
            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                PaymentFrequencyId = viewModel.PaymentFrequency != 0 ? viewModel.PaymentFrequency : (int?) null,
                PositionId = viewModel.PositionId != 0 ? viewModel.PositionId : (int?)null,
            };

            var newEmployee = _employeeInfoRepository.Add(employeeInfo).Employee;
            _unitOfWork.Commit();

            var departments = viewModel.CheckedDepartments != null
                            ? viewModel.CheckedDepartments.Split(',').Select(Int32.Parse)
                            : new List<int>();

            _employeeRepository.UpdateDepartment(departments, employee.EmployeeId);
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

            employeeInfo.PositionId = viewModel.PositionId;
            employeeInfo.PaymentFrequencyId = viewModel.PaymentFrequency;
            employeeInfo.Employee.InjectFrom( viewModel.EmployeeInfo.Employee);

            var departments = viewModel.CheckedDepartments != null
                            ? viewModel.CheckedDepartments.Split(',').Select(Int32.Parse)
                            : new List<int>();
            _employeeRepository.UpdateDepartment(departments, viewModel.EmployeeInfo.EmployeeId);
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

    }
}