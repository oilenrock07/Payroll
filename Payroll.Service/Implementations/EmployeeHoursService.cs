using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeeHoursService : IEmployeeHoursService
    {
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        private readonly ISettingService _settingService;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;

        public EmployeeHoursService(IEmployeeHoursRepository employeeHoursRepository,
          IUnitOfWork unitOfWork, IAttendanceService attendanceService, ISettingService settingService,
          IEmployeeWorkScheduleService employeeWorkScheduleService)
        {
            _employeeHoursRepository = employeeHoursRepository;
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
            _settingService = settingService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
        }

        public int GenerateEmployeeHours(int PaymentFrequencyId, DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
            IList<Employee> employees = _employeeService.GetActiveByPaymentFrequency(PaymentFrequencyId);

            foreach (var employee in employees)
            {
                var employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);

                foreach (DateTime day in DatetimeExtension.EachDay(fromDate, toDate))
                {
                    //Get all employee attendance within date range
                        // Will not include attendance without clockout
                    IList<Attendance> attendanceList = _attendanceService
                        .GetAttendanceByDate(employee.EmployeeId, day);

                    //Compute hours
                    foreach (var attendance in attendanceList)
                    {
                      
                        // Early OT or OT of from yesterday
                        //  This may be special for client
                        DateTime scheduledTimeIn = day;
                        scheduledTimeIn.ChangeTime(employeeWorkSchedule.WorkSchedule.TimeStart.Hours,
                             employeeWorkSchedule.WorkSchedule.TimeStart.Minutes, 0, 0);

                        DateTime scheduledTimeOut = day;
                        // If scheduled clock out time is less than clock in time
                            // scheduled out should be tomorrow
                        if (employeeWorkSchedule.WorkSchedule.TimeEnd < employeeWorkSchedule.WorkSchedule.TimeStart)
                            scheduledTimeOut = day.AddDays(1);

                        scheduledTimeOut.ChangeTime(employeeWorkSchedule.WorkSchedule.TimeEnd.Hours,
                             employeeWorkSchedule.WorkSchedule.TimeEnd.Minutes, 0, 0);

                        DateTime clockIn = attendance.ClockIn;
                        DateTime? clockOut = attendance.ClockOut;

                        bool arrivedEarlierThanSchedule = clockIn < scheduledTimeIn;
                        bool isWithinGracePeriod = this.isWithinGracePeriod(clockIn, scheduledTimeIn);
                        bool isWithinAdvanceOtPeriod = this.isForAdvanceOT(clockIn, scheduledTimeIn);

                        // Count hour before 12 midnight
                        // If different date set clock in to 12AM
                        if (attendance.ClockIn.Day < day.Day)
                        {
                            clockIn = day;
                        }

                        // ********************
                        // *** Advance OT *****
                        // ********************
                        if (arrivedEarlierThanSchedule &&
                            isWithinAdvanceOtPeriod)
                        {
                            TimeSpan? advancedOTHoursCount = scheduledTimeIn - clockIn;

                            EmployeeHours advancedOTHours =
                                new EmployeeHours
                                {
                                    OriginAttendanceId = attendance.AttendanceId,
                                    Date = new DateTime(),
                                    EmployeeId = attendance.EmployeeId,
                                    Hours = advancedOTHoursCount.Value.Hours,
                                    Type = Entities.Enums.RateType.OverTime
                                };

                            _employeeHoursRepository.Add(advancedOTHours);
                        }


                        // Check if within graceperiod or if advance OT
                        if (isWithinGracePeriod || arrivedEarlierThanSchedule)
                        {
                            //Set clockIn to scheduled time in 
                                // For counting of regular hours
                            clockIn = scheduledTimeIn;
                        }

                        // ********************
                        // *** Regular Hours **
                        // ********************
                        bool clockoutLaterThanScheduled = clockOut > scheduledTimeOut;

                        var tempClockOut = clockOut;
                        // If clock out is greater than scheduled time ouot
                        if (clockoutLaterThanScheduled)
                        {
                            //Set clock out to scheduled time out
                            // Ot will be counted later
                            tempClockOut = scheduledTimeOut;
                        }

                        TimeSpan? regularHoursCount = tempClockOut - clockIn;

                        EmployeeHours regularHours =
                                new EmployeeHours
                                {
                                    OriginAttendanceId = attendance.AttendanceId,
                                    Date = new DateTime(),
                                    EmployeeId = attendance.EmployeeId,
                                    Hours = regularHoursCount.Value.Hours,
                                    Type = Entities.Enums.RateType.Regular
                                };

                        _employeeHoursRepository.Add(regularHours);

                        // ********************
                        // *** OT Hours *******
                        // ********************

                        var otTimeStart = scheduledTimeOut;
                        var otTimeEnd = clockOut;

                        if (clockoutLaterThanScheduled &&
                                this.isForOT(otTimeStart, otTimeEnd))
                        {
                            TimeSpan? otHoursCount = clockOut - scheduledTimeOut;

                            EmployeeHours otHours =
                               new EmployeeHours
                               {
                                   OriginAttendanceId = attendance.AttendanceId,
                                   Date = new DateTime(),
                                   EmployeeId = attendance.EmployeeId,
                                   Hours = otHoursCount.Value.Hours,
                                   Type = Entities.Enums.RateType.OverTime
                               };

                            _employeeHoursRepository.Add(otHours);
                        }

                        // ************************************
                        // *** Night Differential Hours *******
                        // ************************************
                        // TODO I assume that the night dif start time starts at night time yesterday
                        // and end time is morning of today's date
                        var ndStartTime = DateTime.Parse(_settingService.GetByKey("SCHEDULE_NIGHTDIF_TIME_START"));
                        var ndEndTime = DateTime.Parse(_settingService.GetByKey("SCHEDULE_NIGHTDIF_TIME_END"));

                        var nightDifStartTime = day.AddDays(-1);
                        nightDifStartTime.ChangeTime(ndStartTime.Hour, ndStartTime.Minute, 0, 0);

                        var nightDifEndTime = day;
                        nightDifEndTime.ChangeTime(ndEndTime.Hour, ndEndTime.Minute, 0, 0);

                        // Re assigned clock in and clock out
                        // TODO recode this
                        clockIn = attendance.ClockIn;
                        clockOut = attendance.ClockOut;

                        // Count hour before 12 midnight
                        // If different date set clock in to 12AM
                        if (attendance.ClockIn.Day < day.Day)
                        {
                            clockIn = day;
                        }

                        if (clockIn >= nightDifStartTime && clockIn <= nightDifEndTime ){
                            //Night Diff Morning

                            //If clockin is less than night dif start time
                            // Set clockin to ND start time
                            if (clockIn.Hour > nightDifStartTime.Hour)
                            {
                                clockIn.ChangeTime(nightDifStartTime.Hour, nightDifStartTime.Minute, 0, 0);
                            }

                            //If clockout is greater than night dif end time
                            // Set clockout to ND end time
                            if (clockOut.Value.Hour > nightDifEndTime.Hour)
                            {
                                clockOut.Value.ChangeTime(nightDifEndTime.Hour, nightDifEndTime.Minute, 0, 0);
                            }

                            TimeSpan? ndHoursCount = clockOut - clockIn;

                            EmployeeHours nightDifHours =
                               new EmployeeHours
                               {
                                   OriginAttendanceId = attendance.AttendanceId,
                                   Date = new DateTime(),
                                   EmployeeId = attendance.EmployeeId,
                                   Hours = ndHoursCount.Value.Hours,
                                   Type = Entities.Enums.RateType.NightDifferential
                               };

                            _employeeHoursRepository.Add(nightDifHours);

                        }
           
                    }
                }
            }
            return 0;
        }

        private bool isWithinGracePeriod(DateTime clockIn, DateTime scheduledClockIn)
        {
            var gracePeriod =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_GRACE_PERIOD_MINUTES"));

            var gracePeriodDuration = new TimeSpan(gracePeriod);

            return (clockIn - scheduledClockIn) <= gracePeriodDuration;
        }

        private bool isForAdvanceOT(DateTime clockIn, DateTime scheduledClockIn)
        {
            var advanceOTPeriod =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_ADVANCE_OT_PERIOD_MINUTES"));

            var advanceOTDuration = new TimeSpan(advanceOTPeriod);

            return (clockIn - scheduledClockIn) > advanceOTDuration;
        }

        private bool isForOT(DateTime otTimeStart, DateTime? otTimeEnd)
        {
            var minimumOT =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_MINIMUM_OT_MINUTES"));

            var minimumOTDuration = new TimeSpan(minimumOT);

            return (otTimeEnd - otTimeStart) >= minimumOTDuration;
        }
    }
}
