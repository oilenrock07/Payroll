using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payroll.Models.Payroll
{
    public class EmployeeAdjustmentCreateViewModel
    {
        public int EmployeeAdjustmentId { get; set; }

        [Required]
        public int AdjustmentId { get; set; }
        [Required]
        public int EmployeeId { get; set; }

        public Entities.Employee Employee { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string Remarks { get; set; }

        public IEnumerable<SelectListItem> Adjustments { get; set; }
    }
}