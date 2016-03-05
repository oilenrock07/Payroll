
namespace Payroll.Common.Enums
{
    public enum AttendanceCode
    {
        ClockIn = 0,
        ClockOut = 1,
        ClockInOt = 4,
        ClockOutOt = 5
    }

    public enum Privilege
    {
        Common = 1,
        Admin = 3
    }

    public enum Frequency
    {
        Weekly = 1,
        Bimonthly = 2,
        Monthly = 3
    }

    public enum EmploymentStatus
    {
        Regular = 1,
        Contractual = 2
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }
}
