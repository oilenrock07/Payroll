﻿using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Constants;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollItemPerCompanyService : BaseEntityService<EmployeePayrollItemPerCompany>, 
        IEmployeePayrollItemPerCompanyService
    {
        private IUnitOfWork _unitOfWork;
        private ITotalEmployeeHoursPerCompanyService _totalEmployeeHoursPerCompanyService;
        private IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private IHolidayService _holidayService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeSalaryService _employeeSalaryService;

        private readonly IEmployeePayrollRepository _employeePayrollRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;

        private IEmployeePayrollItemPerCompanyRepository _employeePayrollItemPerCompanyRepository;

        public EmployeePayrollItemPerCompanyService(IUnitOfWork unitOfWork, IEmployeePayrollItemPerCompanyRepository employeePayrollItemPerCompanyRepository,
            ITotalEmployeeHoursPerCompanyService totalEmployeeHoursPerCompanyService,
            IEmployeeWorkScheduleService employeeWorkScheduleService, IHolidayService holidayService, ISettingService settingService,
            IEmployeeInfoService employeeInfoService, IEmployeeSalaryService employeeSalaryService, IEmployeePayrollRepository employeePayrollRepository,
            IEmployeeLeaveRepository employeeLeaveRepository) 
            : base(employeePayrollItemPerCompanyRepository)
        {
            _employeePayrollItemPerCompanyRepository = employeePayrollItemPerCompanyRepository;
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursPerCompanyService = totalEmployeeHoursPerCompanyService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _holidayService = holidayService;
            _settingService = settingService;
            _employeeInfoService = employeeInfoService;
            _employeeSalaryService = employeeSalaryService;
            _employeePayrollRepository = employeePayrollRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
        }

        //Will get last day of work
        private DateTime getLastDayOfWork(DateTime date, WorkSchedule workSchedule)
        {
            var lastDayOfWork = date.AddDays(-1);
            //Check if with schedule and not holiday
            var holiday = _holidayService.GetHoliday(lastDayOfWork);
            if (lastDayOfWork.IsRestDay(workSchedule.WeekStart, 
                workSchedule.WeekEnd) || holiday != null)
            {
                return getLastDayOfWork(lastDayOfWork, workSchedule);
            }
            else
            {
                return lastDayOfWork;
            }
        }

        /*Note that this method is applicable to employees with hourly rate*/
        public void GenerateEmployeePayrollItemByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Delete existing by date range
            DeleteByDateRange(payrollDate, payrollDate);

            Double restDayRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_REST_DAY));
            Double restDayRateOT = Double.Parse(_settingService.GetByKey(SettingValue.RATE_REST_DAY_OT));
            Double OTRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_OT));
            Double nightDiffRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_NIGHTDIF));
            Double holidayRegularRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_HOLIDAY_REGULAR));
            Double holidaySpecialRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_HOLIDAY_SPECIAL));
            Double OTRateHoliday = Double.Parse(_settingService.GetByKey(SettingValue.RATE_OT_HOLIDAY));
            Double holidaySpecialRestDayRate = Double.Parse(_settingService.GetByKey(SettingValue.RATE_HOLIDAY_SPECIAL_REST_DAY));

            IList<TotalEmployeeHoursPerCompany> totalEmployeeHours =
                    _totalEmployeeHoursPerCompanyService.GetByDateRange(payrollStartDate, payrollEndDate);

            //Get all active employee
            var activeEmployeeList = _employeeInfoService.GetAllActive();
            foreach (EmployeeInfo employee in activeEmployeeList)
            {
                var employeeWorkSchedule =
                    _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);
                if (employeeWorkSchedule == null)
                {
                    continue;
                }
                var workSchedule = employeeWorkSchedule.WorkSchedule;

                //Get all total employee hours
                var employeeTotalHoursList = totalEmployeeHours.Where(h => h.TotalEmployeeHours.EmployeeId == employee.EmployeeId)
                    .OrderByDescending(h => h.TotalEmployeeHours.Date);

                var employeePayrollItemList = new List<EmployeePayrollItemPerCompany>();
                foreach (DateTime day in DatetimeExtension.EachDay(payrollStartDate, payrollEndDate))
                {
                    //Get all total employee hours for the day
                    var employeeTotalHoursListPerDay = employeeTotalHoursList.Where(h => h.TotalEmployeeHours.Date == day);

                    foreach (TotalEmployeeHoursPerCompany totalHours in employeeTotalHoursListPerDay)
                    {
                        var hourlyRate = _employeeSalaryService.GetEmployeeHourlyRate(employee);
                        Double rateMultiplier = 1;

                        //No work schedule, no computation
                        if (employeeWorkSchedule != null)
                        {
                            DateTime date = totalHours.TotalEmployeeHours.Date;
                            RateType rateType = totalHours.TotalEmployeeHours.Type;
                            var companyId = totalHours.CompanyId;

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
                                //If employee didn't work the day before holiday will not get holiday pay
                                if (holiday != null)
                                {
                                    var yesterdayHours = _totalEmployeeHoursPerCompanyService.GetByEmployeeDate(employee.EmployeeId, day.AddDays(-1));
                                    var qualifiedForOT = (yesterdayHours != null && yesterdayHours.Count > 0);

                                    if (!qualifiedForOT)
                                    {
                                        var lastDayOfWork = getLastDayOfWork(day, workSchedule);
                                        var employeeHours = _totalEmployeeHoursPerCompanyService.GetByEmployeeDate(employee.EmployeeId, lastDayOfWork);

                                        qualifiedForOT = (employeeHours != null && employeeHours.Count > 0);
                                    }
                                    

                                    //Set holiday to null if the employee didn't work the day before holiday
                                    if (holiday.IsAlwaysPayable == false)
                                    {
                                        if (!qualifiedForOT)
                                            holiday = null;
                                    }
                                }

                                //Check if with holiday pay
                                if (holiday != null)
                                {
                                    if (holiday.IsRegularHoliday)
                                    {
                                        rateMultiplier *= holidayRegularRate;
                                        if (isRestDay)
                                        {
                                            rateMultiplier *= restDayRate;
                                            //Regular holiday rest day OT
                                            if (totalHours.TotalEmployeeHours.Type == RateType.OverTime)
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
                                            if (totalHours.TotalEmployeeHours.Type == RateType.OverTime)
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
                                            if (totalHours.TotalEmployeeHours.Type == RateType.OverTime)
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
                                            if (totalHours.TotalEmployeeHours.Type == RateType.OverTime)
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
                                            rateMultiplier *= restDayRateOT;
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

                            //Get existing by company
                            var employeePayrollItem = employeePayrollItemList.Where(pi =>
                                pi.EmployeeId == employee.EmployeeId && pi.CompanyId == companyId && pi.RateType == rateType).FirstOrDefault();
                            if (employeePayrollItem == null)
                            {
                                //Create new entry
                                employeePayrollItem = new EmployeePayrollItemPerCompany()
                                {
                                    Multiplier = rateMultiplier,
                                    EmployeeId = employee.EmployeeId,
                                    TotalHours = totalHours.Hours,
                                    TotalAmount = totalPayment,
                                    RatePerHour = hourlyRate,
                                    PayrollDate = payrollDate,
                                    RateType = rateType,
                                    CompanyId = companyId
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
                foreach (EmployeePayrollItemPerCompany payrollItem in employeePayrollItemList)
                {
                    _employeePayrollItemPerCompanyRepository.Add(payrollItem);
                }
            }
            _unitOfWork.Commit();
        }

      

        private void DeleteByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            //Delete existing payrollItems within date range
            var existingPayrollItems = this.GetByDateRange(dateFrom, dateTo);
            _employeePayrollItemPerCompanyRepository.DeleteAll(existingPayrollItems);

            _unitOfWork.Commit();
        }

        public IList<EmployeePayrollItemPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _employeePayrollItemPerCompanyRepository.GetByDateRange(dateFrom, dateTo);
        }

        public virtual IEnumerable<EmployeePayrollItemPerCompany> GetByCutoffDates(DateTime dateFrom, DateTime dateTo)
        {
            var payroll = _employeePayrollRepository.Find(x => x.IsActive && x.CutOffStartDate >= dateFrom && x.CutOffEndDate <= dateTo);
            var payrollItems = from payrollItem in _employeePayrollItemPerCompanyRepository.GetAllActive()
                               join pay in payroll on payrollItem.PayrollId equals pay.PayrollId
                               select payrollItem;


            return payrollItems;
        }

        public IEnumerable<EmployeePayrollItemPerCompany> GetByPayrollId(int payrollId)
        {
            return _employeePayrollItemPerCompanyRepository.Find(x => x.PayrollId == payrollId && x.IsActive);
        }

        public virtual DataTable GetPayrollDetailsForExport(DateTime startDate, DateTime endDate)
        {

            var payrollItems = GetByCutoffDates(startDate, endDate).ToList();
            var payrollIds = payrollItems.Select(x => Convert.ToInt32(x.PayrollId)).ToList();

            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Hourly Rate");
            dt.Columns.Add("Total Regular Hours");
            dt.Columns.Add("Regular Hours Pay");

            //check if there are OTs
            var otExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.OverTime))
            {
                dt.Columns.Add("Total OT Hours");
                dt.Columns.Add("OT Rate");
                dt.Columns.Add("% Rate OT");
                dt.Columns.Add("OT Pay");

                otExists = true;
            }

            //check if there are RestDays
            var restDateExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RestDay))
            {
                dt.Columns.Add("Total Rest Day Hours");
                dt.Columns.Add("Rest Day Rate");
                dt.Columns.Add("% Rate Day");
                dt.Columns.Add("Rest Day Pay");

                restDateExists = true;
            }

            //check if there are RestDays OTs
            var restDayOTExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RestDayOT))
            {
                dt.Columns.Add("Total Rest Day OT Hours");
                dt.Columns.Add("Rest Day OT Rate");
                dt.Columns.Add("% Rate OT Day");
                dt.Columns.Add("Rest Day OT Pay");

                restDayOTExists = true;
            }

            //check if there are Night Differentials
            var nightDiffExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RestDayOT))
            {
                dt.Columns.Add("Total Night Differential Hours");
                dt.Columns.Add("Night Differential Rate");
                dt.Columns.Add("% Night Differential");
                dt.Columns.Add("Night Differential Pay");

                nightDiffExists = true;
            }

            //check if there are Regular Holidays
            var regularHolidayExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RegularHoliday))
            {
                dt.Columns.Add("Total Regular Holiday Hours");
                dt.Columns.Add("Regular Holiday Rate");
                dt.Columns.Add("% Regular Holiday");
                dt.Columns.Add("Regular Holiday Pay");

                regularHolidayExists = true;
            }


            //check if there are Regular Holidays OTs
            var regularHolidayOtExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RegularHolidayOT))
            {
                dt.Columns.Add("Total Regular Holiday OT Hours");
                dt.Columns.Add("Regular Holiday OT Rate");
                dt.Columns.Add("% Regular Holiday OT");
                dt.Columns.Add("Regular Holiday OT Pay");

                regularHolidayOtExists = true;
            }

            //check if there are Regular Holiday Rest Days
            var regularHolidayRDExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RegularHolidayRestDay))
            {
                dt.Columns.Add("Total Regular Holiday Rest Day Hours");
                dt.Columns.Add("Regular Holiday Rest Day Rate");
                dt.Columns.Add("% Regular Holiday Rest Day");
                dt.Columns.Add("Regular Holiday Pay Rest Day Pay");

                regularHolidayRDExists = true;
            }

            //check if there are Regular Holiday Rest Day OTs
            var regularHolidayRdOtExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RegularHolidayRestDayOT))
            {
                dt.Columns.Add("Total Regular Holiday Rest Day OT Hours");
                dt.Columns.Add("Regular Holiday Rest Day OT Rate");
                dt.Columns.Add("% Regular Holiday Rest Day OT");
                dt.Columns.Add("Regular Holiday Pay Rest Day OT Pay");

                regularHolidayRdOtExists = true;
            }

            //check if there are Special Holidays
            var specialHolidayExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.SpecialHoliday))
            {
                dt.Columns.Add("Total Special Holiday Hours");
                dt.Columns.Add("Regular Special Holiday Rate");
                dt.Columns.Add("% Special Holiday");
                dt.Columns.Add("Special Holiday Pay");

                specialHolidayExists = true;
            }

            //check if there are Special Holidays OT
            var specialHolidaysOTExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.SpecialHolidayOT))
            {
                dt.Columns.Add("Total Special Holiday OT Hours");
                dt.Columns.Add("Special Holiday OT Rate");
                dt.Columns.Add("% Special Holiday OT");
                dt.Columns.Add("Special Holiday OT Pay");

                specialHolidaysOTExists = true;
            }

            //check if there are Special Holidays Rest Days
            var specialHolidaysRDExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.SpecialHolidayRestDay))
            {
                dt.Columns.Add("Total Special Holiday Rest Day Hours");
                dt.Columns.Add("Special Holiday Rest Day Rate");
                dt.Columns.Add("% Special Holiday Rest Day");
                dt.Columns.Add("Special Holiday Rest Day Pay");

                specialHolidaysRDExists = true;
            }


            //check if there are Special Holidays Rest Days OTs
            var specialHolidaysRDOtExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.SpecialHolidayRestDayOT))
            {
                dt.Columns.Add("Total Special Holiday Rest Day OT Hours");
                dt.Columns.Add("Special Holiday Rest Day OT Rate");
                dt.Columns.Add("% Special Holiday Rest Day OT");
                dt.Columns.Add("Special Holiday Rest Day OT Pay");

                specialHolidaysRDOtExists = true;
            }

            //check if there are Regular Holidays Not worked
            var regularHolidaysNotWorkedExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.RegularHolidayNotWorked))
            {
                dt.Columns.Add("Total Regular Holidays Not Worked Hours");
                dt.Columns.Add("Regular Holidays Not Worked Rate");
                dt.Columns.Add("% Regular Holidays Not Worked");
                dt.Columns.Add("Regular Holidays Not Worked Pay");

                regularHolidaysNotWorkedExists = true;
            }

            //check if there are Special Holidays Not worked
            var specialHolidaysNotWorkedExists = false;
            if (payrollItems.Any(x => x.RateType == RateType.SpecialHolidayNotWorked))
            {
                dt.Columns.Add("Total Special Holidays Not Worked Hours");
                dt.Columns.Add("Special Holidays Not Worked Rate");
                dt.Columns.Add("% Special Holidays Not Worked");
                dt.Columns.Add("Special Holidays Not Worked Pay");

                specialHolidaysNotWorkedExists = true;
            }

            dt.Columns.Add("SSS/PAGIBIG/PHILHEALTH");
            dt.Columns.Add("Total Payroll");

            var employeeIds = payrollItems.Select(x => x.EmployeeId);
            foreach(var id in employeeIds.Distinct())
            {
                var row = dt.NewRow();
                var payroll = payrollItems.First(x => x.EmployeeId == id);
                var employeePayroll = payrollItems.Where(x => x.EmployeeId == id);

                row["Name"] = payroll.Employee.FullName;

                //Regular Hours
                var regularHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.Regular);
                if (regularHours != null)
                {
                    row["Hourly Rate"] = regularHours.RatePerHour;
                    row["Total Regular Hours"] = regularHours.TotalHours;
                    row["Regular Hours Pay"] = regularHours.TotalAmount;
                }

                //OT
                if (otExists)
                {
                    var otHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.OverTime);
                    row["Total OT Hours"] = otHours != null ? otHours.TotalHours : 0;
                    row["OT Rate"] = otHours != null ? otHours.RatePerHour : 0;
                    row["% Rate OT"] = otHours != null ? otHours.Multiplier : 0;
                    row["OT Pay"] = otHours != null ? otHours.TotalAmount : 0;
                }

                //Rest Day
                if (restDateExists)
                {
                    var restDayHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RestDay);
                    row["Total Rest Day Hours"] = restDayHours != null ? restDayHours.TotalHours : 0;
                    row["Rest Day Rate"] = restDayHours != null ? restDayHours.RatePerHour : 0;
                    row["% Rate Day"] = restDayHours != null ? restDayHours.Multiplier : 0;
                    row["Rest Day Pay"] = restDayHours != null ? restDayHours.TotalAmount : 0;
                }

                //Rest Day OT
                if (restDayOTExists)
                {
                    var restDayOTHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RestDayOT);
                    row["Total Rest Day OT Hours"] = restDayOTHours != null ? restDayOTHours.TotalHours : 0;
                    row["Rest Day OT Rate"] = restDayOTHours != null ? restDayOTHours.RatePerHour : 0;
                    row["% Rate OT Day"] = restDayOTHours != null ? restDayOTHours.Multiplier : 0;
                    row["Rest Day OT Pay"] = restDayOTHours != null ? restDayOTHours.TotalAmount : 0;
                }

                //Night Differential
                if (nightDiffExists)
                {
                    var nightDiffHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.NightDifferential);
                    row["Total Night Differential Hours"] = nightDiffHours != null ? nightDiffHours.TotalHours : 0;
                    row["Night Differential Rate"] = nightDiffHours != null ? nightDiffHours.RatePerHour : 0;
                    row["% Night Differential"] = nightDiffHours != null ? nightDiffHours.Multiplier : 0;
                    row["Night Differential Pay"] = nightDiffHours != null ? nightDiffHours.TotalAmount : 0;
                }


                //Regular Holiday
                if (regularHolidayExists)
                {
                    var regularHolidayHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RegularHoliday);
                    row["Total Regular Holiday Hours"] = regularHolidayHours != null ? regularHolidayHours.TotalHours : 0;
                    row["Regular Holiday Rate"] = regularHolidayHours != null ? regularHolidayHours.RatePerHour : 0;
                    row["% Regular Holiday"] = regularHolidayHours != null ? regularHolidayHours.Multiplier : 0;
                    row["Regular Holiday Pay"] = regularHolidayHours != null ? regularHolidayHours.TotalAmount : 0;
                }

                //Regular Holiday
                if (regularHolidayOtExists)
                {
                    var regularHolidayOTHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RegularHolidayOT);
                    row["Total Regular Holiday OT Hours"] = regularHolidayOTHours != null ? regularHolidayOTHours.TotalHours : 0;
                    row["Regular Holiday OT Rate"] = regularHolidayOTHours != null ? regularHolidayOTHours.RatePerHour : 0;
                    row["% Regular Holiday OT"] = regularHolidayOTHours != null ? regularHolidayOTHours.Multiplier : 0;
                    row["Regular Holiday OT Pay"] = regularHolidayOTHours != null ? regularHolidayOTHours.TotalAmount : 0;
                }

                //Regular Holiday Rest Day
                if (regularHolidayRDExists)
                {
                    var regularHolidayRDHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RegularHolidayRestDay);
                    row["Total Regular Holiday Rest Day Hours"] = regularHolidayRDHours != null ? regularHolidayRDHours.TotalHours : 0;
                    row["Regular Holiday Rest Day Rate"] = regularHolidayRDHours != null ? regularHolidayRDHours.RatePerHour : 0;
                    row["% Regular Holiday Rest Day"] = regularHolidayRDHours != null ? regularHolidayRDHours.Multiplier : 0;
                    row["Regular Holiday Rest Day Pay"] = regularHolidayRDHours != null ? regularHolidayRDHours.TotalAmount : 0;
                }

                //Regular Holiday Rest Day OT
                if (regularHolidayRdOtExists)
                {
                    var regularHolidayRdOtHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RegularHolidayRestDayOT);
                    row["Total Regular Holiday Rest Day OT Hours"] = regularHolidayRdOtHours != null ? regularHolidayRdOtHours.TotalHours : 0;
                    row["Regular Holiday Rest Day OT Rate"] = regularHolidayRdOtHours != null ? regularHolidayRdOtHours.RatePerHour : 0;
                    row["% Regular Holiday Rest Day OT"] = regularHolidayRdOtHours != null ? regularHolidayRdOtHours.Multiplier : 0;
                    row["Regular Holiday Rest Day OT Pay"] = regularHolidayRdOtHours != null ? regularHolidayRdOtHours.TotalAmount : 0;
                }

                //Special Holiday
                if (specialHolidayExists)
                {
                    var specialHolidayHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.SpecialHoliday);
                    row["Total Special Holiday Hours"] = specialHolidayHours != null ? specialHolidayHours.TotalHours : 0;
                    row["Regular Special Holiday Rate"] = specialHolidayHours != null ? specialHolidayHours.RatePerHour : 0;
                    row["% Special Holiday"] = specialHolidayHours != null ? specialHolidayHours.Multiplier : 0;
                    row["Special Holiday Pay"] = specialHolidayHours != null ? specialHolidayHours.TotalAmount : 0;
                }

                //Special Holiday OT
                if (specialHolidaysOTExists)
                {
                    var specialHolidayOtHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.SpecialHolidayOT);
                    row["Total Special Holiday OT Hours"] = specialHolidayOtHours != null ? specialHolidayOtHours.TotalHours : 0;
                    row["Special Holiday OT Rate"] = specialHolidayOtHours != null ? specialHolidayOtHours.RatePerHour : 0;
                    row["% Special Holiday OT"] = specialHolidayOtHours != null ? specialHolidayOtHours.Multiplier : 0;
                    row["Special Holiday OT Pay"] = specialHolidayOtHours != null ? specialHolidayOtHours.TotalAmount : 0;
                }

                //Special Holiday Rest Day
                if (specialHolidaysRDExists)
                {
                    var specialHolidayRdHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.SpecialHolidayRestDay);
                    row["Total Special Holiday Rest Day Hours"] = specialHolidayRdHours != null ? specialHolidayRdHours.TotalHours : 0;
                    row["Special Holiday Rest Day Rate"] = specialHolidayRdHours != null ? specialHolidayRdHours.RatePerHour : 0;
                    row["% Special Holiday Rest Day"] = specialHolidayRdHours != null ? specialHolidayRdHours.Multiplier : 0;
                    row["Special Holiday Rest Day Pay"] = specialHolidayRdHours != null ? specialHolidayRdHours.TotalAmount : 0;
                }

                if (specialHolidaysRDOtExists)
                {
                    var specialHolidayRdOtHours = employeePayroll.FirstOrDefault(x => x.RateType == RateType.SpecialHolidayRestDayOT);
                    row["Total Special Holiday Rest Day OT Hours"] = specialHolidayRdOtHours != null ? specialHolidayRdOtHours.TotalHours : 0;
                    row["Special Holiday Rest Day OT Rate"] = specialHolidayRdOtHours != null ? specialHolidayRdOtHours.RatePerHour : 0;
                    row["% Special Holiday Rest Day OT"] = specialHolidayRdOtHours != null ? specialHolidayRdOtHours.Multiplier : 0;
                    row["Special Holiday Rest Day OT Pay"] = specialHolidayRdOtHours != null ? specialHolidayRdOtHours.TotalAmount : 0;
                }

                if (regularHolidaysNotWorkedExists)
                {
                    var regularHolidaysNotWorked = employeePayroll.FirstOrDefault(x => x.RateType == RateType.RegularHolidayNotWorked);
                    row["Total Regular Holidays Not Worked Hours"] = regularHolidaysNotWorked != null ? regularHolidaysNotWorked.TotalHours : 0;
                    row["Regular Holidays Not Worked Rate"] = regularHolidaysNotWorked != null ? regularHolidaysNotWorked.RatePerHour : 0;
                    row["% Regular Holidays Not Worked"] = regularHolidaysNotWorked != null ? regularHolidaysNotWorked.Multiplier : 0;
                    row["Regular Holidays Not Worked Pay"] = regularHolidaysNotWorked != null ? regularHolidaysNotWorked.TotalAmount : 0;
                }

                if (specialHolidaysNotWorkedExists)
                {
                    var specialHolidaysNotWorked = employeePayroll.FirstOrDefault(x => x.RateType == RateType.SpecialHolidayNotWorked);
                    row["Total Special Holidays Not Worked Hours"] = specialHolidaysNotWorked != null ? specialHolidaysNotWorked.TotalHours : 0;
                    row["Special Holidays Not Worked Rate"] = specialHolidaysNotWorked != null ? specialHolidaysNotWorked.RatePerHour : 0;
                    row["% Special Holidays Not Worked"] = specialHolidaysNotWorked != null ? specialHolidaysNotWorked.Multiplier : 0;
                    row["Special Holidays Not Worked Pay"] = specialHolidaysNotWorked != null ? specialHolidaysNotWorked.TotalAmount : 0;
                }
            }

            return dt;
        }

    }
}
