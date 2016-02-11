using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Employee;
using Payroll.Repository.Interface;
using Payroll.Common.Extension;

namespace Payroll.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISettingRepository _settingRepository;

        //todo: implement caching for settings repository

        public EmployeeController(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, ISettingRepository settingRepository)
        {
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _settingRepository = settingRepository;
        }

        public virtual ActionResult Index()
        {
            var employees = _employeeRepository.Find(x => x.IsActive).ToList();

            return View(employees);
        }

        public virtual ActionResult Edit(int id)
        {
            var employee = _employeeRepository.GetById(id);
            return View(employee);
        }

        [HttpPost]
        public virtual ActionResult Edit(Employee employee)
        {
            employee.IsActive = true;

            _employeeRepository.Update(employee);
            _unitOfWork.Commit();

            UploadImage(employee.EmployeeId);

            return RedirectToAction("Index");
        }

        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Create(EmployeeViewModel viewModel)
        {
            var employee = (Employee) new Employee().InjectFrom(viewModel);
            employee.IsActive = true;

            var newEmployee = _employeeRepository.Add(employee);
            _unitOfWork.Commit();

            UploadImage(newEmployee.EmployeeId);

            return RedirectToAction("Index");
        }

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
                            var fileName = Path.GetFileName(file.FileName);
                            var imagePath = Path.Combine(Server.MapPath(imageUploadPath), String.Format("{0}.jpg", id));
                            file.SaveAs(imagePath);

                            return String.Format("{0}/{1}", imageUploadPath, fileName);
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