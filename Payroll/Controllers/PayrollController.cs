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
    public class PayrollController : Controller
    {
        private readonly IWebService _webService;
        private readonly IEmployeePayrollService _employeePayrollService;
        private readonly ITotalEmployeeHoursService _totalEmployeeHoursService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeePayrollItemService _employeePayrollItemservice;
        private readonly ISettingService _settingsService;
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IEmployeeAdjustmentRepository _employeeAdjustmentRepository;
        private readonly IEmployeeAdjustmentService _employeeAdjustmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeePayrollDeductionService _employeePayrollDeductionService;

        public PayrollController(IWebService webService, IUnitOfWork unitOfWork, 
            IEmployeePayrollService employeePayrollService, 
            ITotalEmployeeHoursService totalEmployeeHoursService, IEmployeeHoursService employeeHoursService,
            IAttendanceService attendanceService, IEmployeePayrollItemService employeePayrollItemService,
            IAdjustmentRepository adjustmentRepository, IEmployeeAdjustmentRepository employeeAdjustmentRepository,
            IEmployeeAdjustmentService employeeAdjustmentService, IEmployeeRepository employeeRepository, IEmployeePayrollDeductionService employeePayrollDeductionService)
        { 
            _webService = webService;
            _unitOfWork = unitOfWork;
            _employeePayrollService = employeePayrollService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeHoursService = employeeHoursService;
            _attendanceService = attendanceService;
            _employeePayrollItemservice = employeePayrollItemService;
            _adjustmentRepository = adjustmentRepository;
            _employeeAdjustmentRepository = employeeAdjustmentRepository;
            _employeeAdjustmentService = employeeAdjustmentService;
            _employeeRepository = employeeRepository;
            _employeePayrollDeductionService = employeePayrollDeductionService;
        }

        // GET: Payroll
        public ActionResult Index(string date = "")
        {
            var viewModel = new PayrollViewModel();
            var payrolls = new List<PayrollDao>();
            var payrollDates = _employeePayrollService.GetPayrollDates(3).Select(x => new SelectListItem
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
                var employeePayrollList = _employeePayrollService.GetByPayrollDateRange
                    (payrollStartDate, payrollEndDate);

                foreach (EmployeePayroll payroll in employeePayrollList)
                {
                    var payrollDto = new PayrollDao
                    {
                        PayrollId = payroll.PayrollId,
                        FirstName = payroll.Employee.FirstName,
                        LastName = payroll.Employee.LastName,
                        MiddleName = payroll.Employee.MiddleName,
                        TotalDeduction = payroll.TotalDeduction,
                        TotalGross = payroll.TotalGross,
                        TotalNet = payroll.TotalNet
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
            var payrollDates = _employeePayrollService.GetPayrollDates(3);
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

                var employeePayrolls = _employeePayrollService.GetByPayrollDateRange(startDate, endDate);

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

        protected IEnumerable<PayrollListViewModel> MapEmployeePayrollToViewModel(IEnumerable<EmployeePayroll> payrolls)
        {
            return payrolls.MapCollection<EmployeePayroll, PayrollListViewModel>((s, d) =>
            {
                d.FirstName = s.Employee.FirstName;
                d.LastName = s.Employee.LastName;
                d.MiddleName = s.Employee.MiddleName;
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
            /*Console.WriteLine("Computing daily payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(payrollStartDate, payrollEndDate);*/
            
            //Generate payroll items
            Console.WriteLine("Computing payroll items for date " + payrollStartDate + " to " + payrollEndDate);
            var payrollDate = _employeePayrollService.GetNextPayrollReleaseDate(payrollEndDate);
            _employeePayrollItemservice.GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStartDate, payrollEndDate);

            //Generate Payroll
            Console.WriteLine("Computing payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeePayrollService.GeneratePayroll(payrollStartDate, payrollEndDate);
        }

        public ActionResult Adjustment()
        {
            var payrollDates = _employeePayrollService.GetPayrollDates(3).ToList();
            var viewModel = new EmployeeAdjustmentViewModel
            {
                Adjustments = payrollDates
                    .Select(x => new SelectListItem
                    {
                        Text = x.FormattedDate,
                        Value = x.SerializedDate
                    }),
                EmployeeAdjustments = GetEmployeeAdjustments(payrollDates.First().SerializedDate)
        };

            return View(viewModel);
        }

        [HttpPost]
        public PartialViewResult GetAdjustments(string date)
        {

            var viewModel = new EmployeeAdjustmentViewModel {EmployeeAdjustments = GetEmployeeAdjustments(date)};
            return PartialView("_Adjustments", viewModel);
        }


        protected IEnumerable<EmployeeAdjustmentDao> GetEmployeeAdjustments(string date)
        {
            var dates = date.Split('-');
            var payrollStartDate = dates[0].DeserializeDate();
            var payrollEndDate = dates[1].DeserializeDate();

            var adjustments = _employeeAdjustmentService.GetEmployeeAdjustmentByDate(payrollStartDate, payrollEndDate);
            return adjustments;
        }

        [HttpPost]
        public PartialViewResult ViewEmployeeAdjustmentDetails(int id, string date)
        {
            var dates = date.Split('-');
            var payrollStartDate = dates[0].DeserializeDate();
            var payrollEndDate = dates[1].DeserializeDate();

            var adjustments = _employeeAdjustmentService.GetEmployeeAdjustments(id, payrollStartDate, payrollEndDate);
            return PartialView("_ViewAdjustmentModalContent", adjustments);
        }

        public ActionResult CreateAdjustment(int id = 0)
        {
            var viewModel = new EmployeeAdjustmentCreateViewModel
            {
                Adjustments = Adjustments(),
                EmployeeId = id,
                Employee = id > 0 ? _employeeRepository.GetById(id) : null
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateAdjustment(EmployeeAdjustmentCreateViewModel viewModel)
        {
            var employeeAdjustment = viewModel.MapItem<EmployeeAdjustment>();
            _employeeAdjustmentRepository.Add(employeeAdjustment);
            _unitOfWork.Commit();

            return RedirectToAction("Adjustment");
        }


        public ActionResult EditAdjustment(int id)
        {
            var adjustments = _employeeAdjustmentRepository.GetById(id);

            var viewModel = adjustments.MapItem<EmployeeAdjustmentCreateViewModel>();
            viewModel.Adjustments = Adjustments();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditAdjustment(EmployeeAdjustmentCreateViewModel viewModel)
        {
            var adjustment = _employeeAdjustmentRepository.GetById(viewModel.EmployeeAdjustmentId);
            _employeeAdjustmentRepository.Update(adjustment);

            adjustment.InjectFrom(viewModel);
            _unitOfWork.Commit();

            return RedirectToAction("Adjustment");
        }

        public ActionResult DeleteAdjustment(int id)
        {
            var adjustment = _employeeAdjustmentRepository.GetById(id);
            _employeeAdjustmentRepository.Update(adjustment);
            adjustment.IsActive = false;

            _unitOfWork.Commit();

            return RedirectToAction("Adjustment");
        }

        protected IEnumerable<SelectListItem> Adjustments()
        {
            var adjustments = _adjustmentRepository.GetAllActive().ToList();
            return adjustments.Any()
                ? adjustments.Select(x => new SelectListItem
                {
                    Text = x.AdjustmentName,
                    Value = x.AdjustmentId.ToString()
                })
                : new List<SelectListItem>();
        }

        public ActionResult Details(int id)
        {
            var viewModel = new PayrollDetailsViewModel();
            viewModel.Payroll = _employeePayrollService.GetById(id);
            viewModel.Deductions = _employeePayrollDeductionService.GetByPayroll(id);
            viewModel.PayrollItems = _employeePayrollItemservice.GetByPayrollId(id);
            viewModel.Adjustments = _employeeAdjustmentService.GetByPayrollId(id);

            return View(viewModel);
        }

        public void ExportToExcel(string startDate, string endDate)
        {
            var data = _employeePayrollItemservice.GetPayrollDetailsForExport(startDate.ToDateTime(), endDate.ToDateTime());
            var fileName = String.Format("Transpose{0}-{1}", Convert.ToDateTime(startDate).Serialize(), Convert.ToDateTime(endDate).Serialize());
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