using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Payroll;
using Payroll.Repository.Interface;
using Payroll.Repository.Models.Payroll;
using Payroll.Service.Interfaces;
using System.Linq;
using Payroll.Entities.Payroll;
using Payroll.Common.Extension;
using Payroll.Helper;
using System.Data;

namespace Payroll.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PayrollPerCompanyController : Controller
    {
        private readonly IWebService _webService;
        private readonly IEmployeePayrollPerCompanyService _employeePayrollService;
        private readonly IEmployeePayrollService _payrollService;
        private readonly ITotalEmployeeHoursPerCompanyService _totalEmployeeHoursService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeePayrollItemPerCompanyService _employeePayrollItemservice;
        private readonly ISettingService _settingsService;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;

        public PayrollPerCompanyController(IWebService webService, 
            IUnitOfWork unitOfWork,
            IEmployeePayrollService payrollService,
            IEmployeePayrollPerCompanyService employeePayrollService, 
            ITotalEmployeeHoursPerCompanyService totalEmployeeHoursService, 
            IEmployeeHoursService employeeHoursService,
            IAttendanceService attendanceService, 
            IEmployeePayrollItemPerCompanyService employeePayrollItemService,
            IEmployeeRepository employeeRepository)
        { 
            _webService = webService;
            _unitOfWork = unitOfWork;
            _employeePayrollService = employeePayrollService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeHoursService = employeeHoursService;
            _attendanceService = attendanceService;
            _employeePayrollItemservice = employeePayrollItemService;
            _employeeRepository = employeeRepository;
            _payrollService = payrollService;
        }

        // GET: Payroll
        public ActionResult Index(string date = "")
        {
            var viewModel = new PayrollViewModel();
            var payrolls = new List<PayrollDao>();
            var payrollDates = _payrollService.GetPayrollDates(6).Select(x => new SelectListItem
            {
                Text = x.FormattedDate,
                Value = x.SerializedDate
            });
            viewModel.PayrollDates = payrollDates;

            if (!String.IsNullOrEmpty(date))
            {
                var dates = date.Split('-');
                var payrollStartDate = dates[0].DeserializeDate();
                var payrollEndDate = dates[1].DeserializeDate();

                ViewBag.StartDate = payrollStartDate;
                ViewBag.EndDate = payrollEndDate;
                GeneratePayroll(payrollStartDate, payrollEndDate);

                //populate the viewmodel here from service data
                //sort it in the service by surname
                var employeePayrollList = _employeePayrollService.GetByDateRange
                    (payrollStartDate, payrollEndDate);

                foreach (EmployeePayrollPerCompany payroll in employeePayrollList)
                {
                    var payrollDto = new PayrollDao
                    {
                        PayrollId = payroll.PayrollPerCompanyId,
                        FirstName = payroll.Employee.FirstName,
                        LastName = payroll.Employee.LastName,
                        MiddleName = payroll.Employee.MiddleName,
                        TotalDeduction = payroll.TotalDeduction,
                        TotalGross = payroll.TotalGross,
                        TotalNet = payroll.TotalNet,
                        Company = payroll.Company
                    };

                    payrolls.Add(payrollDto);
                }

                viewModel.Date = date;
            }

            var pagination = _webService.GetPaginationModel(Request, payrolls.Count);
            viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination);
            viewModel.Pagination = pagination;

            return View(viewModel);
        }

        public ActionResult PayrollPerCompany(string date = "")
        {
            var viewModel = new PayrollViewModel();
            var payrolls = new List<PayrollDao>();
            var payrollDates = _payrollService.GetPayrollDates(6).Select(x => new SelectListItem
            {
                Text = x.FormattedDate,
                Value = x.SerializedDate
            });

            viewModel.PayrollDates = payrollDates;

            if (!String.IsNullOrEmpty(date))
            {
                var dates = date.Split('-');
                var payrollStartDate = dates[0].DeserializeDate();
                var payrollEndDate = dates[1].DeserializeDate();

                ViewBag.StartDate = payrollStartDate;
                ViewBag.EndDate = payrollEndDate;
                GeneratePayroll(payrollStartDate, payrollEndDate);

                //populate the viewmodel here from service data
                //sort it in the service by surname
                var employeePayrollList = _employeePayrollService.GetByDateRange
                    (payrollStartDate, payrollEndDate);

                foreach (EmployeePayrollPerCompany payroll in employeePayrollList)
                {
                    var payrollDto = new PayrollDao
                    {
                        PayrollId = payroll.PayrollPerCompanyId,
                        FirstName = payroll.Employee.FirstName,
                        LastName = payroll.Employee.LastName,
                        MiddleName = payroll.Employee.MiddleName,
                        TotalDeduction = payroll.TotalDeduction,
                        TotalGross = payroll.TotalGross,
                        TotalNet = payroll.TotalNet,
                        Company = payroll.Company
                    };

                    payrolls.Add(payrollDto);
                }

                viewModel.Date = date;
            }

            var pagination = _webService.GetPaginationModel(Request, payrolls.Count);
            viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination);
            viewModel.Pagination = pagination;

            return View(viewModel);
        }

        public ActionResult Search(string date = "", int employeeId = 0)
        {
            //get the last 3 months cutoffs
            var payrollDates = _payrollService.GetPayrollDates(6);
            var viewModel = new PayrollSearchViewModel
            {
                PayrollDates = payrollDates.Select(x => new SelectListItem
                {
                    Text = x.FormattedDate,
                    Value = x.SerializedDate
                }),
                EmployeeId = employeeId
            };
            
            if (date != "")
            {
                var dates = date.Split('-');
                var startDate = dates[0].DeserializeDate();
                var endDate = dates[1].DeserializeDate();

                var employeePayrolls = _employeePayrollService.GetByDateRange(startDate, endDate);

                if (employeeId > 0)
                {
                    employeePayrolls = employeePayrolls.Where(x => x.EmployeeId == employeeId).ToList();
                    viewModel.EmployeeName = employeePayrolls.Any() ? employeePayrolls.First().Employee.FullName : "";
                }
                    
                var payrolls = MapEmployeePayrollToViewModel(employeePayrolls);
                var pagination = _webService.GetPaginationModel(Request, payrolls.Count());
                viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination); 
                viewModel.Date = date;
                viewModel.Pagination = pagination;

                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
            }
 
            return View(viewModel);
        }

        public ActionResult SearchPerCompany(string date = "", int employeeId = 0, int companyId = 0)
        {
            //get the last 3 months cutoffs
            var payrollDates = _payrollService.GetPayrollDates(6);
            var viewModel = new PayrollSearchViewModel
            {
                PayrollDates = payrollDates.Select(x => new SelectListItem
                {
                    Text = x.FormattedDate,
                    Value = x.SerializedDate
                }),
                EmployeeId = employeeId
            };

            if (date != "")
            {
                var dates = date.Split('-');
                var startDate = dates[0].DeserializeDate();
                var endDate = dates[1].DeserializeDate();

                var employeePayrolls = _employeePayrollService.GetByDateRange(startDate, endDate);

                if (employeeId > 0)
                {
                    employeePayrolls = employeePayrolls.Where(x => x.EmployeeId == employeeId).ToList();
                    viewModel.EmployeeName = employeePayrolls.Any() ? employeePayrolls.First().Employee.FullName : "";
                }

                if (companyId > 0)
                {
                    
                }

                var payrolls = MapEmployeePayrollToViewModel(employeePayrolls);
                var pagination = _webService.GetPaginationModel(Request, payrolls.Count());
                viewModel.Payrolls = _webService.TakePaginationModel(payrolls, pagination);
                viewModel.Date = date;
                viewModel.Pagination = pagination;

                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
            }

            return View(viewModel);
        }

        protected IEnumerable<PayrollListViewModel> MapEmployeePayrollToViewModel(IEnumerable<EmployeePayrollPerCompany> payrolls)
        {
            return payrolls.MapCollection<EmployeePayrollPerCompany, PayrollListViewModel>((s, d) =>
            {
                d.FirstName = s.Employee.FirstName;
                d.LastName = s.Employee.LastName;
                d.MiddleName = s.Employee.MiddleName;
                d.Company = s.Company;
            });
        }

        private void GeneratePayroll(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Generate payroll items
            Console.WriteLine("Computing payroll items for date " + payrollStartDate + " to " + payrollEndDate);
            var payrollDate = _payrollService.GetNextPayrollReleaseDate(payrollEndDate);
            _employeePayrollItemservice.GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStartDate, payrollEndDate);

            //Generate Payroll
            Console.WriteLine("Computing payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeePayrollService.GeneratePayroll(payrollStartDate, payrollEndDate);
        }

        public ActionResult Details(int id)
        {
            var viewModel = new PayrollDetailsPerCompanyViewModel();
            viewModel.Payroll = _employeePayrollService.GetById(id);
            viewModel.PayrollItems = _employeePayrollItemservice.GetByPayrollId(id);
          
            return View(viewModel);
        }

        public void ExportToExcel(string startDate, string endDate)
        {
            var data = _employeePayrollItemservice.GetPayrollDetailsForExport(startDate.ToDateTime(), endDate.ToDateTime());
            var fileName = String.Format("Transpose{0}-{1}", startDate.ToDateTime().SerializeShort(), endDate.ToDateTime().SerializeShort());
            Export.ToExcel(Response, data, fileName);
        }

        [HttpPost]
        public JsonResult IsPayrollComputed(string date)
        {
            var dates = date.Split('-');
            var startDate = dates[0].DeserializeDate();
            var endDate = dates[1].DeserializeDate();

            var isPayrollComputed = _employeePayrollService.IsPayrollComputed(startDate, endDate);
            return Json(isPayrollComputed);
        }
    }
}