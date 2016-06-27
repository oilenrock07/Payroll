using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeeDailyPayrollService : IEmployeeDailyPayrollService
    {
        private IUnitOfWork _unitOfWork;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private IHolidayService _holidayService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeSalaryService _employeeSalaryService;

        private IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;

        private const String RATE_REST_DAY = "RATE_REST_DAY";
        private const String RATE_OT = "RATE_OT";
        private const String RATE_NIGHTDIF = "RATE_NIGHTDIF";
        private const String RATE_HOLIDAY_SPECIAL = "RATE_HOLIDAY_SPECIAL";
        private const String RATE_HOLIDAY_REGULAR = "RATE_HOLIDAY_REGULAR";
        private const String RATE_OT_HOLIDAY = "RATE_OT_HOLIDAY";
        private const String PAYROLL_REGULAR_HOURS = "PAYROLL_REGULAR_HOURS";
        private const String PAYROLL_IS_SPHOLIDAY_WITH_PAY = "PAYROLL_IS_SPHOLIDAY_WITH_PAY";
        private const String RATE_HOLIDAY_SPECIAL_REST_DAY = "RATE_HOLIDAY_SPECIAL_REST_DAY";

        public EmployeeDailyPayrollService(IUnitOfWork unitOfWork, ITotalEmployeeHoursService totalEmployeeHoursService, 
            IEmployeeWorkScheduleService employeeWorkScheduleService, IHolidayService holidayService, ISettingService settingService, 
            IEmployeeDailyPayrollRepository employeeDailyPayrollRepository, IEmployeeInfoService employeeInfoService, IEmployeeSalaryService employeeSalaryService)
        {
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _holidayService = holidayService;
            _settingService = settingService;
            _employeeInfoService = employeeInfoService;
            _employeeDailyPayrollRepository = employeeDailyPayrollRepository;
            _employeeSalaryService = employeeSalaryService;
        } 

        /*Note that this method is applicable to employees with hourly rate*/
        public void GenerateEmployeeDailySalaryByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            //Delete existing by date range
            DeleteByDateRange(dateFrom, dateTo);

            IList<TotalEmployeeHours> totalEmployeeHours = 
                _totalEmployeeHoursService.GetByDateRange(dateFrom, dateTo);

            Double restDayRate = Double.Parse(_settingService.GetByKey(RATE_REST_DAY));
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));
            Double nightDiffRate = Double.Parse(_settingService.GetByKey(RATE_NIGHTDIF));
            Double holidayRegularRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_REGULAR));
            Double holidaySpecialRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL));
            Double OTRateHoliday = Double.Parse(_settingService.GetByKey(RATE_OT_HOLIDAY));
            Double holidaySpecialRestDayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL_REST_DAY));

            foreach (TotalEmployeeHours totalHours in totalEmployeeHours)
            {
                EmployeeInfo employeeInfo = _employeeInfoService.GetByEmployeeId(totalHours.EmployeeId);

                var hourlyRate = _employeeSalaryService.GetEmployeeHourlyRate(employeeInfo);

                var employeeWorkSchedule =
                   _employeeWorkScheduleService.GetByEmployeeId(totalHours.EmployeeId);
                //No work schedule, no computation
                if (employeeWorkSchedule != null)
                {
                    DateTime date = totalHours.Date;
                    Double rateMultiplier = 1;

                    var workSchedule = employeeWorkSchedule.WorkSchedule;
                    var isRestDay = date.IsRestDay(workSchedule.WeekStart, workSchedule.WeekEnd);
                    Holiday holiday = _holidayService.GetHoliday(date);

                    //Check if rest day and not special holiday
                    if (isRestDay &&
                        !(holiday != null && !holiday.IsRegularHoliday))
                    {
                        rateMultiplier *= restDayRate;
                    }

                   
                    //Check if holiday 
                    if (holiday != null)
                    {
                        if (holiday.IsRegularHoliday)
                        {
                            rateMultiplier *= holidayRegularRate;
                        }
                        else
                        {
                            if (isRestDay)
                            {
                                rateMultiplier *= holidaySpecialRestDayRate;
                            }
                            else
                            {
                                rateMultiplier *= holidaySpecialRate;
                            }
                        }
                    }
                    //if OT
                    if (totalHours.Type == RateType.OverTime)
                    {
                        //If holiday use holiday ot rate
                        if (holiday != null)
                        {
                            rateMultiplier *= OTRateHoliday;
                        }
                        else
                        {
                            rateMultiplier *= OTRate;
                        }
                    }

                    decimal totalPayment = 0;

                    //if NightDif
                    if (totalHours.Type == RateType.NightDifferential)
                    {
                        //rateMultiplier *= nightDiffRate;
                        /*if (rateMultiplier > 1)
                        {
                            totalPayment = ((decimal)(totalHours.Hours * (rateMultiplier - 1)) * hourlyRate);
                        }*/
                        totalPayment += (decimal)(nightDiffRate * totalHours.Hours * rateMultiplier );
                    }
                    else
                    {
                        totalPayment = ((decimal)(totalHours.Hours * rateMultiplier)) * hourlyRate;
                    }

                    var employeeDailySalary = new EmployeeDailyPayroll
                    {
                        EmployeeId = totalHours.EmployeeId,
                        Date = totalHours.Date,
                        TotalPay = totalPayment,
                        TotalEmployeeHoursId = totalHours.TotalEmployeeHoursId,
                        RateType = totalHours.Type
                    };

                    //Save
                    _employeeDailyPayrollRepository.Add(employeeDailySalary);
                }
            }
            _unitOfWork.Commit();

            //Generate holiday pays
            GenerateEmployeeHolidayPay(dateFrom, dateTo);
            _unitOfWork.Commit();
        }

        public IList<EmployeeDailyPayroll> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _employeeDailyPayrollRepository.GetByDateRange(dateFrom, dateTo);
        }

        public void GenerateEmployeeHolidayPay(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Get all active employees
            IList<EmployeeInfo> employees = _employeeInfoService.GetAllActive();

            foreach (DateTime day in DatetimeExtension.EachDay(payrollStartDate, payrollEndDate))
            {
                //Check if holiday
                var holiday = _holidayService.GetHoliday(day);
                bool isSpecialHolidayPaid = Convert.ToInt32(_settingService.GetByKey(PAYROLL_IS_SPHOLIDAY_WITH_PAY)) > 0;

                if (holiday != null && (holiday.IsRegularHoliday || isSpecialHolidayPaid))
                {
                    foreach (EmployeeInfo employee in employees)
                    {
                        WorkSchedule workSchedule =_employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId).WorkSchedule;

                        if (workSchedule != null)
                        {
                            //Check if within schedule
                            if (day.IsRestDay(workSchedule.WeekStart, workSchedule.WeekEnd))
                            {
                                //Don't proceed
                                return;
                            }
                        }
                        else
                        {
                            //No work schedule, no holiday pay
                            return;
                        }

                        //If with schedule on this date, generate holiday pay

                        //Check if already have daily entry
                        EmployeeDailyPayroll dailyPayroll = _employeeDailyPayrollRepository.GetByDate(employee.EmployeeId, day);

                        int workHours = Convert.ToInt32(_settingService.GetByKey(PAYROLL_REGULAR_HOURS));
                        var hourlyRate = _employeeSalaryService.GetEmployeeHourlyRate(employee);

                        //If null create a holiday pay
                        if (dailyPayroll == null)
                        {
                            EmployeeDailyPayroll newDailyPayroll = new EmployeeDailyPayroll
                            {
                                EmployeeId = employee.EmployeeId,
                                Date = day,
                                TotalPay = hourlyRate * workHours,
                                RateType = RateType.Regular
                            };

                            _employeeDailyPayrollRepository.Add(newDailyPayroll);
                        }
                        else
                        {
                            //If existing create new for remaining unpaid hours
                            //if total hours worked is less than regular working hours
                            //Get total hours worked
                            IList<TotalEmployeeHours> employeeHours =
                                _totalEmployeeHoursService.GetByTypeAndDateRange(employee.EmployeeId, null, payrollStartDate, payrollEndDate);
                            var totalEmployeeHours = employeeHours.Sum(h => h.Hours);
                            if (totalEmployeeHours < workHours)
                            {
                                var remainingUnpaidHours = 
                                    Convert.ToDecimal(workHours - totalEmployeeHours);

                                EmployeeDailyPayroll newDailyPayroll = new EmployeeDailyPayroll
                                {
                                    EmployeeId = employee.EmployeeId,
                                    Date = day,
                                    TotalPay = hourlyRate * remainingUnpaidHours,
                                    RateType = RateType.Regular
                                };
                                _employeeDailyPayrollRepository.Add(newDailyPayroll);
                            }
                        }
                    }
                }             
            }
        }

        public IList<EmployeeDailyPayroll> GetByTypeAndDateRange(RateType rateType, DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _employeeDailyPayrollRepository.GetByTypeAndDateRange(rateType, dateFrom, dateTo);
        }

        private void DeleteByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            //Delete existing daily employee payroll within date range
            var existingDailyPayroll = this.GetByDateRange(dateFrom, dateTo);
            _employeeDailyPayrollRepository.DeleteAll(existingDailyPayroll);

            _unitOfWork.Commit();
        }
    }
}
