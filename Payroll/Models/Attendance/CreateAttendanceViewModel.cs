using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payroll.Models.Attendance
{
    public class CreateAttendanceViewModel
    {
        public int AttendanceId { get; set; }
        public string FullName { get; set; }

        public int EmployeeId { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; }

        [Required]
        public DateTime ClockIn { get; set; }

        [Required]
        public string ClockInTime { get; set; }

        [Required]
        public DateTime ClockOut { get; set; }

        [Required]
        public string ClockOutTime { get; set; }
    }
}