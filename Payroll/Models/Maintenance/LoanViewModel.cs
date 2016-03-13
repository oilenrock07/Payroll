using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Entities;

namespace Payroll.Models.Maintenance
{
    public class LoanViewModel
    {
        public Loan Loan { get; set; }
        public IEnumerable<SelectListItem> LoanPaymentPeriod { get; set; }
    }
}