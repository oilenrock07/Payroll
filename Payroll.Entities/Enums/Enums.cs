
namespace Payroll.Entities.Enums
{
    public enum AttendanceType
    {
        ClockIn = 1,
        ClockOut = 2
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
        NightDifferential = 3
        //,
        //RestDay = 4,
        //HolidayRegular = 5,
        //HolidaySpecial = 6
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
}
