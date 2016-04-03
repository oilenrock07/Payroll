using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_leave")]
    public class EmployeeLeave : BaseEntity
    {
        [Key]
        public int EmployeeLeaveId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(5000)]
        public string Reason { get; set; }

        public bool IsApproved { get; set; }

        public int ApprovedBy { get; set; } //ManagerId

        public int Hours { get; set; } //Default 8 hrs, 1 day
    }
}
