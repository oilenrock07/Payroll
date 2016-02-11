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

        public ActionResult DisplayTimeInOut(int id, AttendanceCode attCode, string timeInOut)
        {
            var employee = _employeeRepository.GetById(id);
            var viewModel = (LogInViewModel)(new LogInViewModel().InjectFrom(employee));
            viewModel.Datetime = timeInOut.DeserializeDate();
            viewModel.ImagePath = String.Format("{0}/{1}", "", employee.Picture); //set cache value here
            viewModel.AttendanceCode = attCode;

            return View(viewModel);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
