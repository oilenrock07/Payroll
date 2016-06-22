using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payroll.Models.Maintenance
{
    public class WorkScheduleViewModel
    {
        public int WorkScheduleId { get; set; }

        [Required]
        public string WorkScheduleName { get; set; }

        [Required]
        public DateTime TimeStart { get; set; }

        [Required]
        public DateTime TimeEnd { get; set; }

        [Required]
        public int WeekStart { get; set; }

        [Required]
        public int WeekEnd { get; set; }

        public IEnumerable<SelectListItem> WeekList { get; set; }
    }
}