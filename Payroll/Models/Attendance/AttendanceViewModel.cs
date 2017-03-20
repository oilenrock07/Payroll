using System;
using System.Collections.Generic;

namespace Payroll.Models.Attendance
{
    public class AttendanceViewModel
    {
        public int AttendanceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public bool Editable { get; set; }

        public bool IsHoliday { get; set; }
        public bool IsRegularHoliday { get; set; }
        public double RegularHours { get; set; }
        public double NightDifferential { get; set; }
        public double Overtime { get; set; }

        public IEnumerable<AttendanceBreakdownViewModel> Breakdown { get; set; }
    }
}