using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll
{
    [Table("employee_daily_payroll")]
    public class EmployeeDailyPayroll : BaseEntity
    {
        [Key]
        public int EmployeeDailySalaryId { get; set; }

        public int EmployeeId { get; set; }
        
        public int TotalEmployeeHoursId { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalPay { get; set; }
    }
}
