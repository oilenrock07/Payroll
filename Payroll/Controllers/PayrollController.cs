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
        private readonly ISettingService _settingsService;
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IEmployeeAdjustmentRepository _employeeAdjustmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayrollController(IWebService webService, IUnitOfWork unitOfWork, 
            IEmployeePayrollService employeePayrollService, 
            ITotalEmployeeHoursService totalEmployeeHoursService, IEmployeeHoursService employeeHoursService,
            IAttendanceService attendanceService, IEmployeeDailyPayrollService employeeDailyPayrollService,
            IAdjustmentRepository adjustmentRepository, IEmployeeAdjustmentRepository employeeAdjustmentRepository)
        { 
            _webService = webService;
            _unitOfWork = unitOfWork;
            _employeePayrollService = employeePayrollService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeHoursService = employeeHoursService;
            _attendanceService = attendanceService;
            _employeeDailyPayrollService = employeeDailyPayrollService;
            _adjustmentRepository = adjustmentRepository;
            _employeeAdjustmentRepository = employeeAdjustmentRepository;
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

        public ActionResult Adjustment()
        {
            var adjustments = _employeeAdjustmentRepository.GetAllActive().ToList();
            return View(adjustments);
        }

        public ActionResult CreateAdjustment()
        {
            var viewModel = new EmployeeAdjustmentViewModel
            {
                Adjustments = Adjustments()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateAdjustment(EmployeeAdjustmentViewModel viewModel)
        {
            var employeeAdjustment = viewModel.MapItem<EmployeeAdjustment>();
            _employeeAdjustmentRepository.Add(employeeAdjustment);
            _unitOfWork.Commit();

            return RedirectToAction("Adjustment");
        }


        public ActionResult EditAdjustment(int id)
        {
            var adjustments = _employeeAdjustmentRepository.GetById(id);

            var viewModel = adjustments.MapItem<EmployeeAdjustmentViewModel>();
            viewModel.Adjustments = Adjustments();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditAdjustment(EmployeeAdjustmentViewModel viewModel)
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
    }
}