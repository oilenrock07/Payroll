
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
        NightDifferential = 2,
        OverTime = 3,
        RestDay = 4,
        HolidayRegular = 5,
        HolidaySpecial = 6
    }
}
