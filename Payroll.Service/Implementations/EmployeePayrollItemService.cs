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
    public class EmployeePayrollItemService : BaseEntityService<EmployeePayrollItem>, 
        IEmployeePayrollItemService
    {
        private IUnitOfWork _unitOfWork;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private IHolidayService _holidayService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeSalaryService _employeeSalaryService;

        private IEmployeePayrollItemRepository _employeePayrollItemRepository;

        private const String RATE_REST_DAY = "RATE_REST_DAY";
        private const String RATE_OT = "RATE_OT";
        private const String RATE_NIGHTDIF = "RATE_NIGHTDIF";
        private const String RATE_HOLIDAY_SPECIAL = "RATE_HOLIDAY_SPECIAL";
        private const String RATE_HOLIDAY_REGULAR = "RATE_HOLIDAY_REGULAR";
        private const String RATE_OT_HOLIDAY = "RATE_OT_HOLIDAY";
        private const String PAYROLL_REGULAR_HOURS = "PAYROLL_REGULAR_HOURS";
        private const String PAYROLL_IS_SPHOLIDAY_WITH_PAY = "PAYROLL_IS_SPHOLIDAY_WITH_PAY";
        private const String RATE_HOLIDAY_SPECIAL_REST_DAY = "RATE_HOLIDAY_SPECIAL_REST_DAY";

        public EmployeePayrollItemService(IUnitOfWork unitOfWork, IEmployeePayrollItemRepository employeePayrollItemRepository, ITotalEmployeeHoursService totalEmployeeHoursService,
            IEmployeeWorkScheduleService employeeWorkScheduleService, IHolidayService holidayService, ISettingService settingService,
            IEmployeeInfoService employeeInfoService, IEmployeeSalaryService employeeSalaryService) 
            : base(employeePayrollItemRepository)
        {
            _employeePayrollItemRepository = employeePayrollItemRepository;
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _holidayService = holidayService;
            _settingService = settingService;
            _employeeInfoService = employeeInfoService;
            _employeeSalaryService = employeeSalaryService;
        }

        /*Note that this method is applicable to employees with hourly rate*/
        public void GenerateEmployeePayrollItemByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Delete existing by date range
            DeleteByDateRange(payrollDate, payrollDate);

            Double restDayRate = Double.Parse(_settingService.GetByKey(RATE_REST_DAY));
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));
            Double nightDiffRate = Double.Parse(_settingService.GetByKey(RATE_NIGHTDIF));
            Double holidayRegularRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_REGULAR));
            Double holidaySpecialRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL));
            Double OTRateHoliday = Double.Parse(_settingService.GetByKey(RATE_OT_HOLIDAY));
            Double holidaySpecialRestDayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL_REST_DAY));

            IList<TotalEmployeeHours> totalEmployeeHours =
                    _totalEmployeeHoursService.GetByDateRange(payrollStartDate, payrollEndDate);

            //Get all active employee
            var activeEmployeeList = _employeeInfoService.GetAllActive();
            foreach (EmployeeInfo employee in activeEmployeeList)
            {
                //Get all total employee hours
                var employeeTotalHoursList = totalEmployeeHours.Where(h => h.EmployeeId == employee.EmployeeId)
                    .OrderByDescending(h => h.Date);

                var employeeWorkSchedule =
                   _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);
                var workSchedule = employeeWorkSchedule.WorkSchedule;

                var employeePayrollItemList = new List<EmployeePayrollItem>();
                foreach (DateTime day in DatetimeExtension.EachDay(payrollStartDate, payrollEndDate))
                {
                    //Get all total employee hours for the day
                    var employeeTotalHoursListPerDay = employeeTotalHoursList.Where(h => h.Date == day);

                    foreach (TotalEmployeeHours totalHours in employeeTotalHoursListPerDay)
                    {
                        var hourlyRate = _employeeSalaryService.GetEmployeeHourlyRate(employee);
                        Double rateMultiplier = 1;

                        //No work schedule, no computation
                        if (employeeWorkSchedule != null)
                        {
                            DateTime date = totalHours.Date;
                            RateType rateType = totalHours.Type;

                            var isRestDay = date.IsRestDay(workSchedule.WeekStart, workSchedule.WeekEnd);
                            Holiday holiday = _holidayService.GetHoliday(date);

                            Decimal totalPayment = 0;
                            //If NightDif
                            if (rateType == RateType.NightDifferential)
                            {
                                totalPayment += (decimal)(nightDiffRate * totalHours.Hours);
                                hourlyRate = (decimal)nightDiffRate;
                            }
                            else
                            {
                                //Check if holiday 
                                if (holiday != null)
                                {
                                    if (holiday.IsRegularHoliday)
                                    {
                                        rateMultiplier *= holidayRegularRate;
                                        if (isRestDay)
                                        {
                                            rateMultiplier *= restDayRate;
                                            //Regular holiday rest day OT
                                            if (totalHours.Type == RateType.OverTime)
                                            {
                                                rateMultiplier *= OTRateHoliday;
                                                rateType = RateType.RegularHolidayRestDayOT;
                                            }
                                            else
                                            {
                                                //Regular holiday rest day
                                                rateType = RateType.RegularHolidayRestDay;
                                            }

                                        }
                                        else
                                        {
                                            //Regular Holiday OT
                                            if (totalHours.Type == RateType.OverTime)
                                            {
                                                rateMultiplier *= OTRateHoliday;
                                                rateType = RateType.RegularHolidayOT;
                                            }
                                            else
                                                //Regular Holiday
                                                rateType = RateType.RegularHoliday;
                                        }
                                    }
                                    else
                                    {
                                        //Special Holiday 
                                        //Rest day
                                        if (isRestDay)
                                        {
                                            rateMultiplier = holidaySpecialRestDayRate;
                                            //Special holiday rest day OT
                                            if (totalHours.Type == RateType.OverTime)
                                            {
                                                rateMultiplier *= OTRateHoliday;
                                                rateType = RateType.SpecialHolidayRestDayOT;
                                            }
                                            else
                                            {
                                                //Special holiday rest day
                                                rateType = RateType.SpecialHolidayRestDay;
                                            }
                                        }
                                        else
                                        {
                                            rateMultiplier *= holidaySpecialRate;
                                            //Special Holiday OT
                                            if (totalHours.Type == RateType.OverTime)
                                            {
                                                rateMultiplier *= OTRateHoliday;
                                                rateType = RateType.SpecialHolidayOT;
                                            }
                                            else
                                                //Special Holiday
                                                rateType = RateType.SpecialHoliday;
                                        }
                                    }
                                }
                                else
                                {
                                    //Check if rest day and not special holiday
                                    if (isRestDay)
                                    {
                                        rateMultiplier *= restDayRate;
                                        if (rateType == RateType.OverTime)
                                        {
                                            rateMultiplier *= OTRate;
                                            rateType = RateType.RestDayOT;
                                        }
                                        else
                                        {
                                            rateType = RateType.RestDay;
                                        }
                                    }else
                                    {
                                        if (rateType == RateType.OverTime)
                                        {
                                            rateMultiplier *= OTRate;
                                        }
                                    }
                                }

                                totalPayment = (decimal)hourlyRate * (decimal)totalHours.Hours * (decimal)rateMultiplier;
                            }

                            //Get existing 
                            var employeePayrollItem = employeePayrollItemList.Where(pi =>
                                pi.EmployeeId == employee.EmployeeId && pi.RateType == rateType).FirstOrDefault();
                            if (employeePayrollItem == null)
                            {
                                //Create new entry
                                employeePayrollItem = new EmployeePayrollItem()
                                {
                                    Multiplier = rateMultiplier,
                                    EmployeeId = employee.EmployeeId,
                                    TotalHours = totalHours.Hours,
                                    TotalAmount = totalPayment,
                                    RatePerHour = hourlyRate,
                                    PayrollDate = payrollDate,
                                    RateType = rateType
                                };

                                employeePayrollItemList.Add(employeePayrollItem);
                            }
                            else
                            {
                                //Update Entry
                                employeePayrollItem.TotalHours += totalHours.Hours;
                                employeePayrollItem.TotalAmount += totalPayment;
                            }
                        }
                    }
                }
                //Insert all payroll items
                foreach (EmployeePayrollItem payrollItem in employeePayrollItemList)
                {
                    _employeePayrollItemRepository.Add(payrollItem);
                }
            }
            _unitOfWork.Commit();

            //Generate holiday pays
            GenerateEmployeeHolidayPay(payrollDate, payrollStartDate, payrollEndDate);
        }

        public void GenerateEmployeeHolidayPay(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
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
                        WorkSchedule workSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId).WorkSchedule;

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

                        //Check if with worked hours
                        double totalEmployeeHours =
                            _totalEmployeeHoursService.CountTotalHours(employee.EmployeeId, day);

                        int workHours = Convert.ToInt32(_settingService.GetByKey(PAYROLL_REGULAR_HOURS));
                        var hourlyRate = _employeeSalaryService.GetEmployeeHourlyRate(employee);
                        var rateType = holiday.IsRegularHoliday ? RateType.RegularHolidayNotWorked : RateType.SpecialHolidayNotWorked;

                        var employeePayrollItem = Find(employee.EmployeeId, day, rateType);
                        //If null create a holiday pay
                        if (totalEmployeeHours <= 0)
                        {
                            //Update 
                            if (employeePayrollItem != null)
                            {
                                _employeePayrollItemRepository.Update(employeePayrollItem);

                                employeePayrollItem.TotalAmount += hourlyRate * workHours;
                                employeePayrollItem.TotalHours += workHours;
                            }
                            else
                            {
                                //Create new
                                EmployeePayrollItem payrollItem = new EmployeePayrollItem
                                {
                                    EmployeeId = employee.EmployeeId,
                                    PayrollDate = payrollDate,
                                    TotalAmount = hourlyRate * workHours,
                                    TotalHours = workHours,
                                    RateType = rateType,
                                    Multiplier = 1,
                                    RatePerHour = hourlyRate
                                };

                                _employeePayrollItemRepository.Add(payrollItem);
                            }
                        }
                        else
                        {
                            //If existing create new for remaining unpaid hours
                            if (totalEmployeeHours < workHours)
                            {
                                var remainingUnpaidHours =
                                    Convert.ToDouble(workHours - totalEmployeeHours);

                                var amount = hourlyRate * (decimal)remainingUnpaidHours;

                                //Update 
                                if (employeePayrollItem != null)
                                {
                                    _employeePayrollItemRepository.Update(employeePayrollItem);

                                    employeePayrollItem.TotalAmount += amount;
                                    employeePayrollItem.TotalHours += remainingUnpaidHours;
                                }
                                else
                                {
                                    //Create new
                                    EmployeePayrollItem payrollItem = new EmployeePayrollItem
                                    {
                                        EmployeeId = employee.EmployeeId,
                                        PayrollDate = payrollDate,
                                        TotalAmount = amount,
                                        TotalHours = remainingUnpaidHours,
                                        RateType = rateType,
                                        Multiplier = 1,
                                        RatePerHour = hourlyRate
                                    };
                                    _employeePayrollItemRepository.Add(payrollItem);
                                }
                            }
                        }
                        _unitOfWork.Commit();
                    }
                }
            }
        }

        public EmployeePayrollItem Find(int employeeId, DateTime date, RateType rateType)
        {
            return _employeePayrollItemRepository.Find(employeeId, date, rateType);
        }

        private void DeleteByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            //Delete existing payrollItems within date range
            var existingPayrollItems = this.GetByDateRange(dateFrom, dateTo);
            _employeePayrollItemRepository.DeleteAll(existingPayrollItems);

            _unitOfWork.Commit();
        }

        public IList<EmployeePayrollItem> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _employeePayrollItemRepository.GetByDateRange(dateFrom, dateTo);
        }

        public IEnumerable<EmployeePayrollItem> GetByPayrollId(int payrollId)
        {
            return _employeePayrollItemRepository.Find(x => x.PayrollId == payrollId && x.IsActive);
        }
    }
}
