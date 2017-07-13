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

        private readonly string SCHEDULE_MINIMUM_OT_MINUTES = "SCHEDULE_MINIMUM_OT_MINUTES";
        private readonly string SCHEDULE_TIME_IN_AM = "SCHEDULE_TIME_IN_AM";
        private readonly string SCHEDULE_TIME_IN_PM = "SCHEDULE_TIME_IN_PM";
        private readonly string SCHEDULE_GRACE_PERIOD_IS_EVERY_HOUR = "SCHEDULE_GRACE_PERIOD_IS_EVERY_HOUR";
        private readonly string SCHEDULE_GRACE_PERIOD_MINUTES = "SCHEDULE_GRACE_PERIOD_MINUTES";
        private readonly string SCHEDULE_TIME_IN_ADJUSTMENT_PERIOD_MINUTES = "SCHEDULE_TIME_IN_ADJUSTMENT_PERIOD_MINUTES";
        private readonly string SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_AM = "SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_AM";
        private readonly string SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_PM = "SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_PM";
        private readonly string SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES = "SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES";
        private readonly string SCHEDULE_NIGHTDIF_TIME_START = "SCHEDULE_NIGHTDIF_TIME_START";
        private readonly string SCHEDULE_NIGHTDIF_TIME_END = "SCHEDULE_NIGHTDIF_TIME_END";
        private readonly string SCHEDULE_ADVANCE_OT_PERIOD_MINUTES = "SCHEDULE_ADVANCE_OT_PERIOD_MINUTES";

        private EmployeeWorkSchedule employeeWorkSchedule;
        private Attendance attendance;
        private DateTime day;
        private DateTime scheduledTimeIn;
        private DateTime scheduledTimeOut;
        private DateTime clockIn;
        private DateTime? clockOut;
        private DateTime nightDifStartTime;
        private DateTime nightDifEndTime;

        private bool arrivedEarlierThanScheduled = false;
        private bool isWithinGracePeriod = false;
        private bool isWithinAdvanceOtPeriod = false;
        private bool clockoutLaterThanScheduled = false;
        private bool clockOutGreaterThanNDEndTime = false;
        private bool isClockInLaterThanScheduledTimeOut = false;
        private bool isGracePeriodPerHour = false;

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

        public int GenerateEmployeeHours(DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
           var employees = _employeeInfoService.GetAllActive();

            //Delete existing entries with the same date
            //var employeeHoursList = this.GetByDateRange(fromDate, toDate);
            //_employeeHoursRepository.DeleteAll(employeeHoursList);
            //_unitOfWork.Commit();

            foreach (var employee in employees)
            {
                employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);
              
                //Will not compute if work schedule is null
                if (employeeWorkSchedule != null)
                {
                    foreach (DateTime d in DatetimeExtension.EachDay(fromDate, toDate))
                    {
                        ComputeEmployeeHours(d, employee.EmployeeId);
                    }
                }
            }

            _unitOfWork.Commit();

            return 0;
        }

        public void ComputeEmployeeHours(DateTime day, int employeeId)
        {
            if (employeeWorkSchedule == null)
                employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employeeId);

            this.day = day;
            //Get all employee attendance within date range
            // Will not include attendance without clockout
            var attendanceList = _attendanceService.GetAttendanceForProcessing(employeeId, day);
            ComputeEmployeeHours(attendanceList, day);
        }

        private void ComputeEmployeeHours(IList<Attendance> attendanceList, DateTime day)
        {
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

                _attendanceService.Update(a);
                if (a.ClockOut.Value.Date.Equals(day.Date))
                {
                    a.IsHoursCounted = true;
                }
            }

            _unitOfWork.Commit();
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
            if (attendance.ClockIn.Date.CompareTo(day.Date) < 0)
            {
                clockIn = day;
            }

            clockOut = attendance.ClockOut;

            if (clockOut.Value.Date.CompareTo(day.Date) > 0)
            {
                clockOut = day.AddDays(1);
            }

            // TODO I assume that the night dif start time starts at night time yesterday
            // and end time is morning of today's date
            var ndStartTime = DateTime.Parse(_settingService.GetByKey(SCHEDULE_NIGHTDIF_TIME_START));
            var ndEndTime = DateTime.Parse(_settingService.GetByKey(SCHEDULE_NIGHTDIF_TIME_END));

            nightDifStartTime = day.AddDays(-1);
            nightDifStartTime = nightDifStartTime.ChangeTime(ndStartTime.Hour, ndStartTime.Minute, 0, 0);

            nightDifEndTime = day;
            nightDifEndTime = nightDifEndTime.ChangeTime(ndEndTime.Hour, ndEndTime.Minute, 0, 0);

            arrivedEarlierThanScheduled = clockIn < scheduledTimeIn;
            isWithinGracePeriod = this.isWtnGracePeriod(clockIn, scheduledTimeIn);
            isWithinAdvanceOtPeriod = this.isForAdvanceOT(clockIn, scheduledTimeIn);
            clockoutLaterThanScheduled = clockOut > scheduledTimeOut;
            clockOutGreaterThanNDEndTime = clockOut > nightDifEndTime;
            isClockInLaterThanScheduledTimeOut = clockIn > scheduledTimeOut;

            //Change clockin if within grace period and is per hour
            if (isWithinGracePeriod && isGracePeriodPerHour)
            {
                clockIn = clockIn.ChangeTime(clockIn.Hour, 0, 0, 0);
            }else if (this.isWtnTimeInAdjustmentPeriod(clockIn))
                //Change clockin if within time adjustment period of time in
            {
                clockIn = clockIn.AddHours(1);
                clockIn = clockIn.ChangeTime(clockIn.Hour, 0,0,0);
            }

            //Change clockout if within time adjustment period of time out
            if (this.isWtnTimeOutAdjustmentBeforePeriod(clockOut.Value))
            {
                //Before clockout hour
                clockOut = clockOut.Value.AddHours(1);
                clockOut = clockOut.Value.ChangeTime(clockOut.Value.Hour, 0, 0, 0);
            }else if (this.isWtnTimeOutAdjustmentAfterPeriod(clockOut.Value))
            {
                //After clockout hour
                clockOut = clockOut.Value.ChangeTime(clockOut.Value.Hour, 0, 0, 0);
            }
        }

        private void computeAdvanceOT()
        {
            // ********************
            // *** Advance OT *****
            // ********************
            if (arrivedEarlierThanScheduled &&
                isWithinAdvanceOtPeriod)
            {
                var baseTimeIn = scheduledTimeIn;
                //If regular hour is less than an hour, all will be recorded as Advance ot
                if (!clockOutGreaterThanNDEndTime)
                {
                    baseTimeIn = clockOut.Value;
                }

                TimeSpan advancedOTHoursCount = baseTimeIn.ChangeSeconds(0,0) - clockIn.ChangeSeconds(0, 0);
                var allowedHours = ComputeTotalAllowedHours(Math.Round(advancedOTHoursCount.TotalHours, 2));
                if (allowedHours > 0)
                {
                   EmployeeHours advancedOTHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = allowedHours,
                       Type = Entities.Enums.RateType.OverTime
                   };

                    _employeeHoursRepository.Add(advancedOTHours);
                }
               
            }

        }

        private void computeRegular()
        {
            //If clock in is later than scheduled time out
                //FOR FRISCO
                //If logout and scheduled time in difference is not an hour.

            //No regular time will be recorded
            if (isClockInLaterThanScheduledTimeOut || 
                (!clockOutGreaterThanNDEndTime && arrivedEarlierThanScheduled && isWithinAdvanceOtPeriod))
                return;

            var tempClockIn = clockIn;
            // Check if within graceperiod or if advance OT
            if (isWithinGracePeriod && !isGracePeriodPerHour)
            {
                //Set clockIn to scheduled time in 
                // For counting of regular hours
                tempClockIn = clockIn.ChangeTime(clockIn.Hour, 0, 0, 0) ;
            }else if (arrivedEarlierThanScheduled)
            {
                //Set clockIn to scheduled time in 
                // For counting of regular hours
                tempClockIn = scheduledTimeIn;
            }

            // ********************
            // *** Regular Hours **
            // ********************
           
            var tempClockOut = clockOut;
            // If clock out is greater than scheduled time out
            if (clockoutLaterThanScheduled)
            {
                //Set clock out to scheduled time out
                // Ot will be counted later
                tempClockOut = scheduledTimeOut;
            }

            TimeSpan regularHoursCount = tempClockOut.Value.ChangeSeconds(0, 0) - tempClockIn.ChangeSeconds(0,0);

            var allowedHours = ComputeTotalAllowedHours(Math.Round(regularHoursCount.TotalHours, 2));
            if (allowedHours > 0)
            {
                EmployeeHours regularHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = allowedHours,
                       Type = Entities.Enums.RateType.Regular
                   };

                _employeeHoursRepository.Add(regularHours);
            }
           
        }

        private void computeOT()
        {
            // ********************
            // *** OT Hours *******
            // ********************
            var otTimeStart = scheduledTimeOut;
            var otTimeEnd = clockOut;

            //If clock in is later than scheduled time out ot time start should be clock in
            // e.g. scheduled time out is 4pm.
            // Last attendance time out is 6pm, 2 hrs of OT is already recorded
            // Another clock in for 11pm, the  otTimeStart should be 11pm not 4pm
            if (isClockInLaterThanScheduledTimeOut)
            {
                otTimeStart = clockIn;
            }
            
            //Set ot time end to 12 am of next days
            if (otTimeEnd.Value.Date > day.Date)
            {
                otTimeEnd = day.AddDays(1);
            }
            if (clockoutLaterThanScheduled)
            {
                TimeSpan otHoursCount = otTimeEnd.Value.ChangeSeconds(0, 0) - otTimeStart.ChangeSeconds(0, 0);

                var totalHours = ComputeTotalAllowedHours(Math.Round(otHoursCount.TotalHours, 2));
                if (otHoursCount != null && otHoursCount.TotalHours > 0 && totalHours > 0)
                {
                  EmployeeHours otHours =
                  new EmployeeHours
                  {
                      OriginAttendanceId = attendance.AttendanceId,
                      Date = day,
                      EmployeeId = attendance.EmployeeId,
                      Hours = totalHours,
                      Type = Entities.Enums.RateType.OverTime
                  };

                    _employeeHoursRepository.Add(otHours);
                }
               
            }
        }

        private void computeNightDifferential() {
            // ************************************
            // *** Night Differential Hours *******
            // ************************************

            //Morning night dif
            var morningNightDifStartTime = nightDifStartTime;
            var morningNightDifEndTime = nightDifEndTime;

            if (isWithinAdvanceOtPeriod)
            {
                computeNightDifferential(morningNightDifStartTime, morningNightDifEndTime);
            }

            //Evening night dif
            var eveningNightDifStartTime = nightDifStartTime.AddDays(1);
            var eveningNightDifEndTime = nightDifEndTime.AddDays(1);

            computeNightDifferential(eveningNightDifStartTime, eveningNightDifEndTime);
        }

        private void computeNightDifferential(DateTime startTime, DateTime endTime)
        {
            // ************************************
            // *** Night Differential Hours *******
            // ************************************
            if ((clockIn >= startTime && clockIn <= endTime) ||
                    (clockOut >= startTime && clockOut <= endTime))
            {
                //If clockin is less than night dif start time
                // Set clockin to ND start time
                if (clockIn < startTime)
                {
                    clockIn = clockIn.ChangeTime(startTime.Hour, startTime.Minute, 0, 0);
                }

                //If clockout is greater than night dif end time
                // Set clockout to ND end time
                if (clockOut.Value > endTime)
                {
                    //This handles if NightDif overlaps schedule
                    // If timeout is later nightDifEnd set out to less than 1 hour than actual
                    if (nightDifEndTime.TimeOfDay > employeeWorkSchedule.WorkSchedule.TimeStart &&
                        clockOut.Value.TimeOfDay >= employeeWorkSchedule.WorkSchedule.TimeStart)
                    {
                        clockOut = clockOut.Value.ChangeTime(endTime.Hour, 0, 0, 0);
                    }
                    else
                    {
                        clockOut = clockOut.Value.ChangeTime(endTime.Hour, endTime.Minute, 0, 0);
                    }
                }

                TimeSpan ndHoursCount = clockOut.Value.ChangeSeconds(0, 0) - clockIn.ChangeSeconds(0, 0);

                //Create entry if have night differential hours to record
                var totalHours = ComputeTotalAllowedHours(Math.Round(ndHoursCount.TotalHours, 2));
                if (totalHours > 0)
                {
                    EmployeeHours nightDifHours =
                        new EmployeeHours
                        {
                            OriginAttendanceId = attendance.AttendanceId,
                            Date = day,
                            EmployeeId = attendance.EmployeeId,
                            Hours = totalHours,
                            Type = Entities.Enums.RateType.NightDifferential
                        };

                    _employeeHoursRepository.Add(nightDifHours);
                }


            }

        }

        //TODO make work schedule to have 2 parts, on and after break time 
            //to be able to compute grace period for both without using settings
        private bool isWtnGracePeriod(DateTime clockIn, DateTime scheduledClockIn)
        {
            var timeInAM = DateTime.Parse(_settingService.GetByKey(SCHEDULE_TIME_IN_AM));
            var timeInPM = DateTime.Parse(_settingService.GetByKey(SCHEDULE_TIME_IN_PM));

            isGracePeriodPerHour = Boolean.Parse(_settingService.GetByKey(SCHEDULE_GRACE_PERIOD_IS_EVERY_HOUR));

            var gracePeriod = Int32.Parse(_settingService.GetByKey(SCHEDULE_GRACE_PERIOD_MINUTES));

            var gracePeriodDuration = new TimeSpan(0, gracePeriod, 0);
            var timeSpanZero = new TimeSpan();

            if (isGracePeriodPerHour)
            {
                var clockInMinutes = clockIn.TimeOfDay.Minutes;

                return new TimeSpan(0, clockInMinutes, 0) <= gracePeriodDuration;
            }
            else
            {
                var timeDifferenceAM = clockIn.TimeOfDay - timeInAM.TimeOfDay;
                var timeDifferencePM = clockIn.TimeOfDay - timeInPM.TimeOfDay;
                
                return (timeDifferenceAM <= gracePeriodDuration && timeDifferenceAM > timeSpanZero) ||
                     (timeDifferencePM <= gracePeriodDuration && timeDifferencePM > timeSpanZero);
            }
          
        }

        private bool isWtnTimeInAdjustmentPeriod(DateTime clockIn)
        {
            var timeInAdjustmentPeriodMinutes =
                             Int32.Parse(_settingService.GetByKey(SCHEDULE_TIME_IN_ADJUSTMENT_PERIOD_MINUTES));

            var gracePeriodDuration = new TimeSpan(0, timeInAdjustmentPeriodMinutes, 0);
            var nextHour = clockIn.AddHours(1);
            nextHour = nextHour.ChangeTime(nextHour.Hour, 0, 0, 0);

            return (nextHour - clockIn) <= gracePeriodDuration;
        }

        private bool isWtnTimeOutAdjustmentBeforePeriod(DateTime clockOut)
        {
            var adjustmentPeriodOutAM = DateTime.Parse(_settingService.GetByKey(SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_AM));
            var adjustmentPeriodOutPM = DateTime.Parse(_settingService.GetByKey(SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES_SCHED_PM));
            var timeOutAdjustmentPeriodMinutes =
                             Int32.Parse(_settingService.GetByKey(SCHEDULE_TIME_OUT_ADJUSTMENT_PERIOD_MINUTES));

            var timeOutAdjustmentDuration = new TimeSpan(0, timeOutAdjustmentPeriodMinutes, 0);
            var zeroTimeSpan = new TimeSpan();
            var timeDifferenceAM = adjustmentPeriodOutAM.TimeOfDay - clockOut.TimeOfDay;
            var timeDifferencePM = adjustmentPeriodOutPM.TimeOfDay - clockOut.TimeOfDay;

            return (timeDifferenceAM <= timeOutAdjustmentDuration && timeDifferenceAM > zeroTimeSpan) ||
                (timeDifferencePM <= timeOutAdjustmentDuration && timeDifferencePM > zeroTimeSpan);
        }

        private bool isWtnTimeOutAdjustmentAfterPeriod(DateTime clockOut)
        {
            var timeOutAdjustmentPeriodMinutes =
                             Int32.Parse(_settingService.GetByKey(SCHEDULE_MINIMUM_OT_MINUTES));

            var timeOutAdjustmentDuration = new TimeSpan(0, timeOutAdjustmentPeriodMinutes, 0);
            var clockOutMinutes = clockOut.TimeOfDay.Minutes;

            return new TimeSpan(0, clockOutMinutes, 0) <= timeOutAdjustmentDuration;
        }

        private bool isForAdvanceOT(DateTime clockIn, DateTime scheduledClockIn)
        {
            var advanceOTPeriod =
                             Int32.Parse(_settingService.GetByKey(SCHEDULE_ADVANCE_OT_PERIOD_MINUTES));

            var advanceOTDuration = new TimeSpan(0, advanceOTPeriod, 0);

            var clockInVsScheduled = (scheduledClockIn - clockIn);

            return clockInVsScheduled > advanceOTDuration;
        }


        // Remove checking of minimum OT since it should be checked in the payroll computation
        private bool isForOT(DateTime otTimeStart, DateTime? otTimeEnd)
        {
            var minimumOT =
                             Int32.Parse(_settingService.GetByKey(SCHEDULE_MINIMUM_OT_MINUTES));

            var minimumOTDuration = new TimeSpan(minimumOT);

            return (otTimeEnd - otTimeStart) >= minimumOTDuration;
        }

        public double ComputeTotalAllowedHours(double TotalHours)
        {
            double total = TotalHours;
            //Change implementation of minimum excess minutes computation
            //Total employee hours minimum is 5 mins
            //Get minimum OT minutes value
            /*double minimumTimeInMinutes = (Convert.ToDouble
                (_settingService.GetByKey(SCHEDULE_MINIMUM_OT_MINUTES)) / (double)60);
            double totalMinutes = total - Math.Truncate(total);
            if (Math.Round(totalMinutes, 2) < Math.Round(minimumTimeInMinutes, 2))
            {
                //Set total hours to floor
                total = Math.Floor(total);
            }*/

            return total;
        }

        public IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            return _employeeHoursRepository.GetByEmployeeAndDateRange(employeeId, fromDate, toDate);
        }

        public IList<EmployeeHours> GetForProcessingByEmployeeAndDate(int employeeId, DateTime date)
        {
            var toDate = date.AddDays(1);
            return _employeeHoursRepository.GetForProcessingByEmployeeAndDate(employeeId, date, toDate);
        }

        public IList<EmployeeHours> GetForProcessingByDateRange(bool isManual, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            return _employeeHoursRepository.GetForProcessingByDateRange(isManual, fromDate, toDate);
        }

        public void Update(EmployeeHours employeeHours)
        {
            _employeeHoursRepository.Update(employeeHours);
        }

        public IList<EmployeeHours> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            return _employeeHoursRepository.GetByDateRange(fromDate, toDate);
        }
    }
}
