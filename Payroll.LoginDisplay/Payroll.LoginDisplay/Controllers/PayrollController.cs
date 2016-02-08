using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Omu.ValueInjecter;
using Payroll.Common.Enums;
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
            viewModel.Datetime = Convert.ToDateTime(timeInOut);
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
