using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Models
{
    public class AttendanceDao
    {
        public int AttendanceId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }

        public IEnumerable<EmployeeHours> EmployeeHours { get; set; }
        public bool HasEmployeeHours { get; set; }
    }
}
