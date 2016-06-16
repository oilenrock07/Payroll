using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Models.Payroll;
using Payroll.Repository.Models.Payroll;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PayrollController : Controller
    {
        private readonly IWebService _webService;
        private readonly IEmployeePayrollService _employeePayrollService;
        private readonly ISettingService _settingsService;

        public PayrollController(IWebService webService, 
            IEmployeePayrollService employeePayrollService, ISettingService settingService)
        {
            _webService = webService;
            _employeePayrollService = employeePayrollService;
            _settingsService = settingService;
        }

        // GET: Payroll
        public ActionResult Index(string date = "")
        {
            var viewModel = new PayrollViewModel();
            var payrolls = new List<PayrollDao>();
            if (!String.IsNullOrEmpty(date))
            {
                //populate the viewmodel here from service data
                //sort it in the service by surname?
                payrolls = new List<PayrollDao>()
                {
                    new PayrollDao
                    {
                        FirstName = "Cornelio",
                        LastName = "Cawicaan",
                        MiddleName = "Bue",
                        TotalDeduction = 2000,
                        TotalGross = 20000,
                        TotalNet = 18000
                    },
                    new PayrollDao
                    {
                        FirstName = "Jona",
                        LastName = "Pereira",
                        MiddleName = "Aprecio",
                        TotalDeduction = 1800,
                        TotalGross = 20000,
                        TotalNet = 18200
                    },
                    new PayrollDao
                    {
                        FirstName = "Mar",
                        LastName = "Roxas",
                        MiddleName = "XYZ",
                        TotalDeduction = 1000,
                        TotalGross = 20000,
                        TotalNet = 19000
                    },
                    new PayrollDao
                    {
                        FirstName = "Grace",
                        LastName = "Poe",
                        MiddleName = "ABC",
                        TotalDeduction = 2500,
                        TotalGross = 20000,
                        TotalNet = 17500
                    },
                    new PayrollDao
                    {
                        FirstName = "Rodrigo",
                        LastName = "Duterte",
                        MiddleName = "XXX",
                        TotalDeduction = 0,
                        TotalGross = 20000,
                        TotalNet = 20000
                    },
                };
            }

            var pagination = _webService.GetPaginationModel(Request, payrolls.Count);
            var payrollStartDate = _employeePayrollService.GetNextPayrollStartDate();
            var payrollEndDate = _employeePayrollService.GetNextPayrollEndDate(payrollStartDate);

            viewModel.StartDate = payrollStartDate.ToShortDateString();
            viewModel.EndDate = payrollEndDate.ToShortDateString();
            viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination);
            viewModel.Pagination = pagination;

            return View(viewModel);
        }
    }
}