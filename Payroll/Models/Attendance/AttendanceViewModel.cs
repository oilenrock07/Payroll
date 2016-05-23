
using System;

namespace Payroll.Models.Attendance
{
    public class AttendanceViewModel
    {
        public int AttendanceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime ClockOut { get; set; }
        public bool Editable { get; set; }
    }
}