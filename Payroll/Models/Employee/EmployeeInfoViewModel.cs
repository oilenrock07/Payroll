using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Entities;

namespace Payroll.Models.Employee
{
    public class EmployeeInfoViewModel
    {
        public EmployeeInfoViewModel()
        {
            EmployeeInfo = new EmployeeInfo();
        }

        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int PositionId { get; set; }
        public int PaymentFrequency { get; set; }
        public bool IsPrivate { get; set; }

        public EmployeeInfo EmployeeInfo { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }
    }
}