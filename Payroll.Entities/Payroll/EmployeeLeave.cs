using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;
using Payroll.Entities.Users;

namespace Payroll.Entities.Payroll
{
    [Table("employee_leave")]
    public class EmployeeLeave : BaseEntity
    {
        [Key]
        public int EmployeeLeaveId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [ForeignKey("Leave")]
        public int LeaveId { get; set; }
        public virtual Leave Leave { get; set; }

        public DateTime Date { get; set; }

        [StringLength(5000)]
        public string Reason { get; set; }

        public LeaveStatus LeaveStatus { get; set; }

        [ForeignKey("User")]
        [Column("Id")]
        public string ApprovedBy { get; set; } //ManagerId
        public virtual User User { get; set; }

        public int Hours { get; set; } //Default 8 hrs, 1 day
    }
}
