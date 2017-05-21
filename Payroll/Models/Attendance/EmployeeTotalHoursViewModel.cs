using System;
using Payroll.Entities.Payroll;

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
        public int TotalRegularHoursId { get; set; }

        public double NightDifferential { get; set; }
        public int TotalNightDifferentialId { get; set; }

        public double Overtime { get; set; }
        public int TotalOvertimeId { get; set; }
    }
}