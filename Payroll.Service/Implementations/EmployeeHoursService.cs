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
        private bool isWithinGracePeriodTimeOut = false;
        private bool isWithinAdvanceOtPeriod = false;
        private bool clockoutLaterThanScheduled = false;
        private bool clockOutGreaterThanNDEndTime = false;
        private bool isClockInLaterThanScheduledTimeOut = false;

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
            var employeeHoursList = this.GetByDateRange(fromDate, toDate);
            _employeeHoursRepository.DeleteAll(employeeHoursList);
            _unitOfWork.Commit();

            foreach (var employee in employees)
            {
                employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);
              
                //Will not compute if work schedule is null
                if (employeeWorkSchedule != null)
                {
                    foreach (DateTime d in DatetimeExtension.EachDay(fromDate, toDate))
                    {
                        day = d;
                        //Get all employee attendance within date range
                            // Will not include attendance without clockout
                        IList<Attendance> attendanceList = _attendanceService
                            .GetAttendanceForProcessing(employee.EmployeeId, day);

                        ComputeEmployeeHours(attendanceList, day);
                    }
                }
                
            }

            _unitOfWork.Commit();

            return 0;
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

                /*_attendanceService.Update(a);
                if (a.ClockOut.Value.Date.Equals(day.Date))
                {
                    a.IsHoursCounted = true;
                }*/
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
            if (attendance.ClockIn.Day < day.Day)
            {
                clockIn = day;
            }

            clockOut = attendance.ClockOut;

            if (clockOut.Value.Day > day.Day)
            {
                clockOut = day.AddDays(1);
            }

            // TODO I assume that the night dif start time starts at night time yesterday
            // and end time is morning of today's date
            var ndStartTime = DateTime.Parse(_settingService.GetByKey("SCHEDULE_NIGHTDIF_TIME_START"));
            var ndEndTime = DateTime.Parse(_settingService.GetByKey("SCHEDULE_NIGHTDIF_TIME_END"));

            nightDifStartTime = day.AddDays(-1);
            nightDifStartTime = nightDifStartTime.ChangeTime(ndStartTime.Hour, ndStartTime.Minute, 0, 0);

            nightDifEndTime = day;
            nightDifEndTime = nightDifEndTime.ChangeTime(ndEndTime.Hour, ndEndTime.Minute, 0, 0);

            arrivedEarlierThanScheduled = clockIn < scheduledTimeIn;
            isWithinGracePeriod = this.isWtnGracePeriod(clockIn, scheduledTimeIn);
            isWithinGracePeriodTimeOut = this.isWtnGracePeriodTimeOut(clockOut.Value);
            isWithinAdvanceOtPeriod = this.isForAdvanceOT(clockIn, scheduledTimeIn);
            clockoutLaterThanScheduled = clockOut > scheduledTimeOut;
            clockOutGreaterThanNDEndTime = clockOut > nightDifEndTime;
            isClockInLaterThanScheduledTimeOut = clockIn > scheduledTimeOut;

            //Change clockout if within grace period of time out
            if (isWithinGracePeriodTimeOut)
            {
                clockOut = clockOut.Value.AddHours(1);
                clockOut = clockOut.Value.ChangeTime(clockOut.Value.Hour, 0,0,0);
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

                if (advancedOTHoursCount.TotalHours > 0)
                {
                   EmployeeHours advancedOTHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = ComputeTotalAllowedHours(Math.Round(advancedOTHoursCount.TotalHours, 2)),
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
            if (isClockInLaterThanScheduledTimeOut || !clockOutGreaterThanNDEndTime)
                return;

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
            // If clock out is greater than scheduled time out
            if (clockoutLaterThanScheduled)
            {
                //Set clock out to scheduled time out
                // Ot will be counted later
                tempClockOut = scheduledTimeOut;
            }

            TimeSpan regularHoursCount = tempClockOut.Value.ChangeSeconds(0, 0) - tempClockIn.ChangeSeconds(0,0);

            if (regularHoursCount.TotalHours > 0)
            {
                EmployeeHours regularHours =
                   new EmployeeHours
                   {
                       OriginAttendanceId = attendance.AttendanceId,
                       Date = day,
                       EmployeeId = attendance.EmployeeId,
                       Hours = ComputeTotalAllowedHours(Math.Round(regularHoursCount.TotalHours, 2)),
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

                if (otHoursCount != null && otHoursCount.TotalHours > 0)
                {
                  EmployeeHours otHours =
                  new EmployeeHours
                  {
                      OriginAttendanceId = attendance.AttendanceId,
                      Date = day,
                      EmployeeId = attendance.EmployeeId,
                      Hours = ComputeTotalAllowedHours(Math.Round(otHoursCount.TotalHours, 2)),
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
                if (ndHoursCount.TotalHours > 0)
                {
                    EmployeeHours nightDifHours =
                        new EmployeeHours
                        {
                            OriginAttendanceId = attendance.AttendanceId,
                            Date = day,
                            EmployeeId = attendance.EmployeeId,
                            Hours = ComputeTotalAllowedHours(Math.Round(ndHoursCount.TotalHours, 2)),
                            Type = Entities.Enums.RateType.NightDifferential
                        };

                    _employeeHoursRepository.Add(nightDifHours);
                }


            }

        }

        private bool isWtnGracePeriod(DateTime clockIn, DateTime scheduledClockIn)
        {
            var gracePeriod =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_GRACE_PERIOD_MINUTES"));

            var gracePeriodDuration = new TimeSpan(0, gracePeriod, 0);

            return (clockIn - scheduledClockIn) <= gracePeriodDuration;
        }

        private bool isWtnGracePeriodTimeOut(DateTime clockOut)
        {
            var gracePeriodTimeOut =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_GRACE_PERIOD_MINUTES_OUT"));

            var gracePeriodDuration = new TimeSpan(0, gracePeriodTimeOut, 0);
            var nextHour = clockOut.AddHours(1);
            nextHour = nextHour.ChangeTime(nextHour.Hour, 0, 0, 0);

            return (nextHour - clockOut) <= gracePeriodDuration;
        }

        private bool isForAdvanceOT(DateTime clockIn, DateTime scheduledClockIn)
        {
            var advanceOTPeriod =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_ADVANCE_OT_PERIOD_MINUTES"));

            var advanceOTDuration = new TimeSpan(0, advanceOTPeriod, 0);

            var clockInVsScheduled = (scheduledClockIn - clockIn);

            return clockInVsScheduled > advanceOTDuration;
        }


        // Remove checking of minimum OT since it should be checked in the payroll computation
        private bool isForOT(DateTime otTimeStart, DateTime? otTimeEnd)
        {
            var minimumOT =
                             Int32.Parse(_settingService.GetByKey("SCHEDULE_MINIMUM_OT_MINUTES"));

            var minimumOTDuration = new TimeSpan(minimumOT);

            return (otTimeEnd - otTimeStart) >= minimumOTDuration;
        }

        public double ComputeTotalAllowedHours(double TotalHours)
        {
            double total = TotalHours;
            //Total employee hours minimum is 5 mins
            //Get minimum OT minutes value
            double minimumTimeInMinutes = (Convert.ToDouble
                (_settingService.GetByKey(SCHEDULE_MINIMUM_OT_MINUTES)) / (double)60);
            double totalMinutes = total - Math.Truncate(total);
            if (Math.Round(totalMinutes, 2) < Math.Round(minimumTimeInMinutes, 2))
            {
                //Set total hours to floor
                total = Math.Floor(total);
            }

            return total;
        }

        public IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            return _employeeHoursRepository.GetByEmployeeAndDateRange(employeeId, fromDate, toDate);
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
