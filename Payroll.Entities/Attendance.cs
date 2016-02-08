using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [StringLength(50)]
        public string EmployeeCode { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime ? ClockOut { get; set; }
        public Boolean IsManuallyEdited { get; set; }
    }
}
