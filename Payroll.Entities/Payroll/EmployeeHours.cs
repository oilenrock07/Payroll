using Payroll.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Payroll
{
    [Table("employee_hours")]
    public class EmployeeHours
    {
        [Key]
        public int EmployeeHoursId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public RateType Type { get; set; }
        public int OriginAttendanceId { get; set; }
    }
}
