using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Payroll.Models.Payroll
{
    public class PayrollDetailsViewModel
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OtRate { get; set; }
        public double NumberOfDays { get; set; } //could be half day?
        public decimal SalaryPay { get; set; }
        public decimal ButalPay { get; set; }
        public double TotalHoursOfOt { get; set; }
        public decimal OtPay { get; set; }
        
        //Deductions
    }
}