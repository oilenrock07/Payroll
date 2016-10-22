using System;
namespace Payroll.Models.Attendance
{
    public class AttendanceBreakdownViewModel
    {
        public DateTime Date { get; set; }
        public double RegularHours { get; set; }
        public double Overtime { get; set; }
        public double NightDifferential { get; set; }
    }
}