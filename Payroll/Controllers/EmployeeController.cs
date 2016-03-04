using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
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

        public EmployeeController(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, IEmployeeInfoRepository employeeInfoRepository,
            ISettingRepository settingRepository, IPositionRepository positionRepository,
            IWebService webService, IPaymentFrequencyRepository paymentFrequencyRepository)
        {
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _settingRepository = settingRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _positionRepository = positionRepository;
            _webService = webService;
            _paymentFrequencyRepository = paymentFrequencyRepository;
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
            var viewModel = _employeeRepository.GetById(id).MapItem<EmployeeViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(EmployeeViewModel viewModel)
        {

            //validate birthdate
            if (!viewModel.BirthDate.IsValidBirthDate())
            {
                ModelState.AddModelError("", ErrorMessages.INVALID_DATE);
                return View(viewModel);
            }

            var employee = _employeeRepository.GetById(viewModel.EmployeeId);
            _employeeRepository.Update(employee);
            employee.InjectFrom(viewModel);
            employee.IsActive = true;

            var imagePath = UploadImage(employee.EmployeeId);
            if (!String.IsNullOrEmpty(imagePath))
                employee.Picture = imagePath;

            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }

        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(EmployeeViewModel viewModel)
        {
            //validate birthdate
            if (!viewModel.BirthDate.IsValidBirthDate())
            {
                ModelState.AddModelError("", ErrorMessages.INVALID_DATE);
                return View(viewModel);
            }

            var employee = viewModel.MapItem<Employee>();
            employee.IsActive = true;

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
            };
            var newEmployee = _employeeInfoRepository.Add(employeeInfo).Employee;
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

        [HttpGet]
        public ActionResult Details(int id)
        {
            var employeeInfo = _employeeInfoRepository.GetByEmployeeId(id);
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


            positions.Insert(0, new SelectListItem {Text = "Select Position", Value = "0"});
            paymentFrequencies.Insert(0, new SelectListItem { Text = "Select Payment Frequency", Value = "0" });

            var viewModel = new EmployeeInfoViewModel();

            if (employeeInfo != null)
            {
                viewModel.EmployeeInfo = employeeInfo;
                viewModel.ImagePath = employeeInfo.Employee.Picture != null ? Url.Content(employeeInfo.Employee.Picture) : "";
                viewModel.Name = employeeInfo.Employee.FullName;
                viewModel.Positions = positions;
                viewModel.PositionId = Convert.ToInt32(employeeInfo.PositionId);
                viewModel.PaymentFrequency = Convert.ToInt32(employeeInfo.PaymentFrequencyId);
                viewModel.PaymentFrequencies = paymentFrequencies;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmploymentInfo(EmployeeInfoViewModel viewModel)
        {
            var employeeInfo = new EmployeeInfo {EmploymentInfoId = viewModel.EmployeeInfo.EmploymentInfoId};
            _employeeInfoRepository.Update(employeeInfo);
            employeeInfo.InjectFrom(viewModel.EmployeeInfo);
            employeeInfo.PositionId = viewModel.PositionId;
            employeeInfo.PaymentFrequencyId = viewModel.PaymentFrequency;

            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }

    }
}