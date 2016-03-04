using System;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Enums;
using Payroll.Common.Extension;
using Payroll.LoginDisplay.Models.Payroll;
using Payroll.Repository.Interface;

namespace Payroll.LoginDisplay.Controllers
{
    public class PayrollController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public PayrollController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public ActionResult DisplayTimeInOut(int id, AttendanceCode attCode, string timeInOut)
        {
            var viewModel = new LogInViewModel
            {
                Datetime = timeInOut.DeserializeDate(),
                AttendanceCode = attCode,
                EmployeeId = id
            };

            return View(viewModel);
        }

        //find a way to create a permanent duration
        //duration value is 1 month
        [HttpGet]
        //[OutputCache(Duration = 2592000, VaryByParam = "id", VaryByCustom = "payroll:employeeinformation")]
        public ActionResult EmployeeInformation(int id)
        {
            var employee = _employeeRepository.GetById(id);
            var viewModel = (LogInViewModel)(new LogInViewModel().InjectFrom(employee));
            viewModel.ImagePath = Url.Content(employee.Picture);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
