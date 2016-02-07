using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_workschedule")]
    public class EmployeeWorkSchedule
    {
        [Key]
        public int EmployeeWorkScheduleId { get; set; }

        public int EmployeeId { get; set; }

        public int WorkScheduleId { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
