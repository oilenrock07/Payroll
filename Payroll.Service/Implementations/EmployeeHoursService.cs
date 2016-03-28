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
        private readonly IEmployeeInfoService _employeeInfoService;
        private readonly ISettingService _settingService;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;

        private EmployeeWorkSchedule employeeWorkSchedule;
        private Attendance attendance;
        private DateTime day;
        private DateTime scheduledTimeIn;
        private DateTime scheduledTimeOut;
        private DateTime clockIn;
        private DateTime? clockOut;
        private bool arrivedEarlierThanScheduled = false;
        private bool isWithinGracePeriod = false;
        private bool isWithinAdvanceOtPeriod = false;
        private bool clockoutLaterThanScheduled = false;

        public EmployeeHoursService(IUnitOfWork unitOfWork, 
            IEmployeeHoursRepository employeeHoursRepository,
            IAttendanceService attendanceService, ISettingService settingService,
            IEmployeeWorkScheduleService employeeWorkScheduleService,
            IEmployeeInfoService employeeInfoService)
        {
            _employeeHoursRepository = employeeHoursRepository;
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
            _settingService = settingService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
            _employeeInfoService = employeeInfoService;
        }

        public int GenerateEmployeeHours(int PaymentFrequencyId, DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
            IList<EmployeeInfo> employees = _employeeInfoService.GetActiveByPaymentFrequency(PaymentFrequencyId);

            foreach (var employee in employees)
            {
                employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);

                foreach (DateTime d in DatetimeExtension.EachDay(fromDate, toDate))
                {
                    day = d;
                    //Get all employee attendance within date range
                        // Will not include attendance without clockout
                    IList<Attendance> attendanceList = _attendanceService
                        .GetAttendanceByDate(employee.EmployeeId, day);

                    //Compute hours
                    foreach (var a in attendanceList)
                    {
                        attendance = a;

                        //Initiate Variables
                        initiateComputationVariables();

                        //Computations
                        computeAdvanceOT();
                        computeRegular();
                        computeOT();
                        computeNightDifferential();    
                    }
                }
            }

            _unitOfWork.Commit();

            return 0;
        }

        private void initiateComputationVariables()
        {
            // Early OT or OT of from yesterday
            //  This may be special for client
            scheduledTimeIn = day;
            scheduledTimeIn = scheduledTimeIn.ChangeTime(employeeWorkSchedule.WorkSchedule.TimeStart.Hours,
                 employeeWorkSchedule.WorkSchedule.TimeStart.Minutes, 0, 0);

            scheduledTimeOut = day;
            // If scheduled clock out time is less than clock in time
            // scheduled out should be tomorrow
            if (employeeWorkSchedule.WorkSchedule.TimeEnd < employeeWorkSchedule.WorkSchedule.TimeStart)
                scheduledTimeOut = day.AddDays(1);

            scheduledTimeOut = scheduledTimeOut.ChangeTime(employeeWorkSchedule.WorkSchedule.TimeEnd.Hours,
                 employeeWorkSchedule.WorkSchedule.TimeEnd.Minutes, 0, 0);

            clockIn = attendance.ClockIn;
            // Count hour before 12 midnight
            // If different date set clock in to 12AM
            if (attendance.ClockIn.Day < day.Day)
            {
                clockIn = day;
            }

            clockOut = attendance.ClockOut;

            arrivedEarlierThanScheduled = clockIn < scheduledTimeIn;
            isWithinGracePeriod = this.isWtnGracePeriod(clockIn, scheduledTimeIn);
            isWithinAdvanceOtPeriod = this.isForAdvanceOT(clockIn, scheduledTimeIn);
            clockoutLaterThanScheduled = clockOut > scheduledTimeOut;
        }

        private void computeAdvanceOT()
        {
            // ********************
            // *** Advance OT *****
            // ********************
            if (arrivedEarlierThanScheduled &&
                isWithinAdvanceOtPeriod)
            {
                TimeSpan? advancedOTHoursCount = scheduledTimeIn - clockIn;

                EmployeeHours advancedOTHours =
                    new EmployeeHours
                    {
                        OriginAttendanceId = attendance.AttendanceId,
                        Date = day,
                        EmployeeId = attendance.EmployeeId,
                        Hours = advancedOTHoursCount.Value.TotalHours,
                        Type = Entities.Enums.RateType.OverTime
                    };

                _employeeHoursRepository.Add(advancedOTHours);
            }

        }

        private void computeRegular()
        {
            var tempClockIn = clockIn;
            // Check if within graceperiod or if advance OT
            if (isWithinGracePeriod || arrivedEarlierThanScheduled)
            {
                //Set clockIn to scheduled time in 
                // For counting of regular hours
                tempClockIn = scheduledTimeIn;
            }

            // ********************
            // *** Regular Hours **
            // ********************
           
            var tempClockOut = clockOut;
            // If clock out is greater than scheduled time ouot
            if (clockoutLaterThanScheduled)
            {
                //Set clock out to scheduled time out
                // Ot will be counted later
                tempClockOut = scheduledTimeOut;
            }

            TimeSpan? regularHoursCount = tempClockOut - tempClockIn;

            EmployeeHours regularHours =
                    new EmployeeHours
                    {
                        OriginAttendanceId = attendance.AttendanceId,
                        Date = day,
                        EmployeeId = attendance.EmployeeId,
                        Hours = regularHoursCount.Value.TotalHours,
                        Type = Entities.Enums.RateType.Regular
                    };

            _employeeHoursRepository.Add(regularHours);
        }

        private void computeOT()
        {
            // ********************
            // *** OT Hours *******
            // ********************

            var otTimeStart = scheduledTimeOut;
            var otTimeEnd = clockOut;

            if (clockoutLaterThanScheduled)
            {
                TimeSpan? otHoursCount = clockOut - scheduledTimeOut;

                EmployeeHours otHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = otHoursCount.Value.TotalHours,
                       Type = Entities.Enums.RateType.OverTime
                   };

                _employeeHoursRepository.Add(otHours);
            }
        }

        private void computeNightDifferential() {
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

            if (clockIn >= nightDifStartTime && clockIn <= nightDifEndTime)
            {
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
                    //This handles if NightDif overlaps schedule
                        // If timeout is later nightDifEnd set out to less than 1 hour than actual
                    if (nightDifEndTime.TimeOfDay > employeeWorkSchedule.WorkSchedule.TimeStart &&
                        clockOut.Value.TimeOfDay >= employeeWorkSchedule.WorkSchedule.TimeStart)
                    {
                        clockOut.Value.ChangeTime(nightDifEndTime.Hour-1, 0, 0, 0);
                    }
                    else
                    {
                        clockOut.Value.ChangeTime(nightDifEndTime.Hour, nightDifEndTime.Minute, 0, 0);
                    }
                }

                TimeSpan? ndHoursCount = clockOut - clockIn;

                EmployeeHours nightDifHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = ndHoursCount.Value.TotalHours,
                       Type = Entities.Enums.RateType.NightDifferential
                   };

                _employeeHoursRepository.Add(nightDifHours);

            }

        }
        private bool isWtnGracePeriod(DateTime clockIn, DateTime scheduledClockIn)
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


        // Remove checking of minimum OT since it should be checked in the payroll computation
        private bool isForOT(DateTime otTimeStart, DateTime? otTimeEnd)
        {
            var minimumOT =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_MINIMUM_OT_MINUTES"));

            var minimumOTDuration = new TimeSpan(minimumOT);

            return (otTimeEnd - otTimeStart) >= minimumOTDuration;
        }
    }
}
