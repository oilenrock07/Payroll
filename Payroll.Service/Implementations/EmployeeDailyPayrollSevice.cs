using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
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
        private UnitOfWork _unitOfWork;
        private TotalEmployeeHoursService _totalEmployeeHoursService;
        private EmployeeWorkScheduleService _employeeWorkScheduleService;
        private HolidayService _holidayService;
        private SettingService _settingService;
        private EmployeeDailyPayrollRepository _employeeDailySalaryRepository;

        private readonly String RATE_REST_DAY = "RATE_REST_DAY";
        private readonly String RATE_OT = "RATE_OT";
        private readonly String RATE_NIGHTDIF = "RATE_NIGHTDIF";
        private readonly String RATE_HOLIDAY_SPECIAL = "RATE_HOLIDAY_SPECIAL";
        private readonly String RATE_HOLIDAY_REGULAR = "RATE_HOLIDAY_REGULAR";

        public EmployeeDailyPayrollService(UnitOfWork unitOfWork, TotalEmployeeHoursService totalEmployeeHoursService, 
            EmployeeWorkScheduleService employeeWorkScheduleService, HolidayService holidayService, SettingService settingService, 
            EmployeeDailyPayrollRepository employeeDailySalaryRepository)
        {
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _holidayService = holidayService;
            _settingService = settingService;
            _employeeDailySalaryRepository = employeeDailySalaryRepository;
        } 

        public void GenerateEmployeeDailySalaryByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            IList<TotalEmployeeHours> totalEmployeeHours = 
                _totalEmployeeHoursService.GetByDateRange(dateFrom, dateTo);

            Double restDayRate = Double.Parse(_settingService.GetByKey(RATE_REST_DAY));
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));
            Double nightDiffRate = Double.Parse(_settingService.GetByKey(RATE_NIGHTDIF));
            Double holidayRegularRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_REGULAR));
            Double holidateSpecialRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL));

            foreach (TotalEmployeeHours totalHours in totalEmployeeHours)
            {
                WorkSchedule workSchedule = 
                    _employeeWorkScheduleService.GetByEmployeeId(totalHours.EmployeeId).WorkSchedule;

                DateTime date = totalHours.Date;

                Double rateMultiplier = 1;

                //Check if rest day
                if (date.IsRestDay(workSchedule.WeekStart, workSchedule.WeekEnd))
                {
                    rateMultiplier += restDayRate;
                }

                Holiday holiday = _holidayService.GetHoliday(date);
                //Check if holiday 
                if (holiday != null)
                {
                    if (holiday.IsRegularHoliday)
                    {
                        rateMultiplier += holidayRegularRate;
                    }
                    else
                    {
                        rateMultiplier += holidateSpecialRate;
                    }
                   
                }
                //if OT
                if (totalHours.Type == RateType.OverTime)
                {
                    rateMultiplier += OTRate;
                }
                //if NightDif
                if (totalHours.Type == RateType.NightDifferential)
                {
                    rateMultiplier += nightDiffRate;
                }

                var employeeDailySalary = new EmployeeDailyPayroll
                {
                    EmployeeId = totalHours.EmployeeId,
                    Date = totalHours.Date,
                    TotalPay = (decimal)(totalHours.Hours * rateMultiplier),
                    TotalEmployeeHoursId = totalHours.TotalEmployeeHoursId
                };

                //Save
                _employeeDailySalaryRepository.Add(employeeDailySalary);
            }
        }
    }
}
