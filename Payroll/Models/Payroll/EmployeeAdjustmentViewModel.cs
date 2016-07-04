using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Repository.Models.Payroll;

namespace Payroll.Models.Payroll
{
    public class EmployeeAdjustmentViewModel
    {
        public IEnumerable<EmployeeAdjustmentDao> EmployeeAdjustments { get; set; }
        public string Date { get; set; }
        public IEnumerable<SelectListItem> Adjustments { get; set; }
    }
}