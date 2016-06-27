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
        private readonly ITotalEmployeeHoursService _totalEmployeeHoursService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeDailyPayrollService _employeeDailyPayrollService;
   
        public PayrollController(IWebService webService, IEmployeePayrollService employeePayrollService, 
            ITotalEmployeeHoursService totalEmployeeHoursService, IEmployeeHoursService employeeHoursService,
            IAttendanceService attendanceService, IEmployeeDailyPayrollService employeeDailyPayrollService)
        {
            _webService = webService;
            _employeePayrollService = employeePayrollService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeHoursService = employeeHoursService;
            _attendanceService = attendanceService;
            _employeeDailyPayrollService = employeeDailyPayrollService;
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

                GeneratePayroll(payrollStartDate, payrollEndDate);

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

        private void GeneratePayroll(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Generate Attendance
            Console.WriteLine("Generating Attendance...");
            _attendanceService.CreateWorkSchedules();

            //Compute employee hours
            Console.WriteLine("Computing daily employee hours for date " + payrollStartDate + " to " +
                              payrollEndDate);
            _employeeHoursService.GenerateEmployeeHours(payrollStartDate, payrollEndDate);

            //Compute total employee hours
            Console.WriteLine("Computing total employee hours for date " + payrollStartDate + " to " +
                              payrollEndDate);
            _totalEmployeeHoursService.GenerateTotalByDateRange(payrollStartDate, payrollEndDate);

            //Compute daily payroll
            Console.WriteLine("Computing daily payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(payrollStartDate, payrollEndDate);

            //Generate Payroll
            Console.WriteLine("Computing payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeePayrollService.GeneratePayroll(payrollStartDate, payrollEndDate);

        }
    }
}