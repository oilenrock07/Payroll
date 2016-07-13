
using System.ComponentModel;

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
        [Description("Regular")]
        Regular = 1,

        [Description("Over Time")]
        OverTime = 2,

        [Description("Night Differential")]
        NightDifferential = 3,

        [Description("Rest Day")]
        RestDay = 4,

        [Description("Rest Day OT")]
        RestDayOT = 5,

        [Description("Regular Holiday Not Worked")]
        RegularHolidayNotWorked = 6,

        [Description("Special Holiday Not Worked")]
        SpecialHolidayNotWorked = 7,

        [Description("Regular Holiday")]
        RegularHoliday = 8,

        [Description("Regular Holiday OT")]
        RegularHolidayOT = 9,

        [Description("Special Holiday")]
        SpecialHoliday = 10,

        [Description("Special Holiday OT")]
        SpecialHolidayOT = 11,

        [Description("Regular Holiday Rest Day")]
        RegularHolidayRestDay = 12,

        [Description("Regular Holiday Rest Day OT")]
        RegularHolidayRestDayOT = 13,

        [Description("Special Holiday Rest Day")]
        SpecialHolidayRestDay = 14,

        [Description("Special HolidayRest Day OT")]
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
