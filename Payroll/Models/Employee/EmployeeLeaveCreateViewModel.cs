using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payroll.Models.Employee
{
    public class EmployeeLeaveCreateViewModel
    {
        public int EmployeeLeaveId { get; set; }
        public string EmployeeName { get; set; }

        [DisplayName("Employee")]
        [Required]
        public int EmployeeId { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; }

        [DisplayName("Leave")]
        [Required]
        public int LeaveId { get; set; }
        public IEnumerable<SelectListItem> Leaves { get; set; }

        [Required]
        public int Hours { get; set; }
        public IEnumerable<SelectListItem> LeaveHours { get; set; }

        public int SpecifiedHours { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Reason { get; set; }

        [DisplayName("Mark as Approved")]
        public bool MarkAsApproved { get; set; }

    }
}