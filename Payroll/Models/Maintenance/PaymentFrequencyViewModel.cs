using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Entities;

namespace Payroll.Models.Maintenance
{
    public class PaymentFrequencyViewModel
    {
        public PaymentFrequency PaymentFrequency { get; set; }
        public IEnumerable<SelectListItem> DayOfWeeks { get; set; }
        public IEnumerable<SelectListItem> Frequencies { get; set; }
    }
}