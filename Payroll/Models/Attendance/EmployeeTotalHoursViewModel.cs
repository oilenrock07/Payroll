using System;

namespace Payroll.Models.Attendance
{
    public class EmployeeTotalHoursViewModel
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Date { get; set; }

        public double RegularHours { get; set; }
        public double NightDifferential { get; set; }
        public double Overtime { get; set; }
    }
}