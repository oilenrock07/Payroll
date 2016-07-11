
namespace Payroll.Entities.Enums
{
    public enum AttendanceType
    {
        ClockIn = 0,
        ClockOut = 1,
        ClockInOt = 4,
        ClockOutOt = 5
    }

    public enum TimePeriod
    {
        AM = 1,
        PM = 2
    }

    public enum RateType
    {
        Regular = 1,
        OverTime = 2,
        NightDifferential = 3,
        RestDay = 4, 
        RestDayOT = 5,
        RegularHolidayNotWorked = 6,
        SpecialHolidayNotWorked = 7,
        RegularHoliday = 8,
        RegularHolidayOT = 9,
        SpecialHoliday = 10,
        SpecialHolidayOT = 11,
        RegularHolidayRestDay = 12,
        RegularHolidayRestDayOT = 13,
        SpecialHolidayRestDay = 14,
        SpecialHolidayRestDayOT = 15
    }

    public enum FrequencyType
    {
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        BiWeekly = 4,
        SemiMonthly = 5,
        Monthly = 6
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        NotApproved = 3
    }

    public enum AuditTrailTransaction
    {
        Employee = 1,
        Maintenance = 2,
        Payroll = 3,
        Attendance = 4,
        Users = 5
    }

    public enum AuditTrailTransactionType
    {
        Create = 1,
        View = 2,
        Update = 3,
        Delete = 4,
    }

    public enum SchedulerLogType
    {
        Success = 1,
        Exception = 2
    }
}
