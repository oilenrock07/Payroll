using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Models.Payroll;
using Payroll.Repository.Models.Payroll;
using Payroll.Service.Interfaces;
using System.Linq;
using Payroll.Entities.Payroll;
using Payroll.Common.Extension;

namespace Payroll.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PayrollController : Controller
    {
        private readonly IWebService _webService;
        private readonly IEmployeePayrollService _employeePayrollService;
        private readonly ISettingService _settingsService;
   
        public PayrollController(IWebService webService, 
            IEmployeePayrollService employeePayrollService)
        {
            _webService = webService;
            _employeePayrollService = employeePayrollService;
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

        public ActionResult Search(string payrollDate = "", int employeeId = 0)
        {
            //get the last 6 months cutoffs
            var payrollDates = _employeePayrollService.GetPayrollDates(6);
            var viewModel = new PayrollSearchViewModel
            {
                PayrollDates = payrollDates.Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                }),
                EmployeeId = employeeId
            };

            if (payrollDate != "")
            {
                var dates = payrollDate.Split(new string[] { " to " }, StringSplitOptions.None);
                var employeePayrolls = _employeePayrollService.GetByDateRange(Convert.ToDateTime(dates[0]), Convert.ToDateTime(dates[1]));

                if (employeeId > 0)
                    employeePayrolls = employeePayrolls.Where(x => x.EmployeeId == employeeId).ToList();

                var payrolls = MapEmployeePayrollToViewModel(employeePayrolls);
                var pagination = _webService.GetPaginationModel(Request, payrolls.Count());
                viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination); 
                viewModel.PayrollDate = payrollDate;
                viewModel.Pagination = pagination;
            }

            return View(viewModel);
        }

        protected IEnumerable<PayrollListViewModel> MapEmployeePayrollToViewModel(IEnumerable<EmployeePayroll> payrolls)
        {
            return payrolls.MapCollection<EmployeePayroll, PayrollListViewModel>((s, d) =>
            {
                
            });
        }
    }
}