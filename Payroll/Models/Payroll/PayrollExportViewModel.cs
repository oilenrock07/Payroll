using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Payroll.Models.Payroll
{
    public class PayrollExportViewModel
    {
        public string Name { get; set; }

        //Regular Hours
        public double TotalRegularHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal RegularHoursPay { get; set; }

        //OT
        public double TotalOTHours { get; set; }
        public double MyProperty { get; set; }
        public decimal OTPay { get; set; }
    }
}