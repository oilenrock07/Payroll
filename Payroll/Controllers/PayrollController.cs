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
        public ActionResult Index(string StartDate = "", string EndDate = "")
        {
            var viewModel = new PayrollViewModel();
            var payrolls = new List<PayrollDao>();

            var pagination = _webService.GetPaginationModel(Request, payrolls.Count);
            var payrollStartDate = _employeePayrollService.GetNextPayrollStartDate();
            var payrollEndDate = _employeePayrollService.GetNextPayrollEndDate(payrollStartDate);
            
            if (!String.IsNullOrEmpty(StartDate) && !String.IsNullOrEmpty(EndDate))
            {
                payrollStartDate = DateTime.Parse(StartDate);
                payrollEndDate = DateTime.Parse(EndDate);

                //Generate Payroll
                _employeePayrollService.GeneratePayroll(payrollStartDate, payrollEndDate);

                //populate the viewmodel here from service data
                //sort it in the service by surname
                var employeePayrollList = _employeePayrollService
                    .GetByDateRange(payrollStartDate, payrollEndDate).OrderBy(p => p.Employee.LastName);

                foreach (EmployeePayroll payroll in employeePayrollList)
                {
                    var payrollDto = new PayrollDao
                    {
                        FirstName = payroll.Employee.FirstName,
                        LastName = payroll.Employee.LastName,
                        MiddleName = payroll.Employee.MiddleName,
                        TotalDeduction = payroll.TotalDeduction,
                        TotalGross = payroll.TotalGross,
                        TotalNet = payroll.TotalNet
                    };

                    payrolls.Add(payrollDto);
                }
            }

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