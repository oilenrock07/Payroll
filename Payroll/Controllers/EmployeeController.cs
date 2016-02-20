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
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IWebService _webService;

        public EmployeeController(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, ISettingRepository settingRepository,
            IWebService webService)
        {
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _settingRepository = settingRepository;
            _webService = webService;
        }

        public virtual ActionResult Index()
        {
            var employees = _employeeRepository.Find(x => x.IsActive).ToList();
            var pagination = _webService.GetPaginationModel(Request, employees);

            return View(pagination);
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
            var employee = new Employee {EmployeeId = viewModel.EmployeeId};
            _employeeRepository.Update(employee);
            employee = viewModel.MapItem<Employee>();
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
            var employee = viewModel.MapItem<Employee>();
            employee.IsActive = true;

            var newEmployee = _employeeRepository.Add(employee);
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
            var employee = new Employee {EmployeeId = id};
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
                    var imageUploadPath = _settingRepository.GetSettingValue("APP_EMPLOYEE_IMAGE_PATH");
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