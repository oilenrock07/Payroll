using Payroll.Entities;
using Payroll.Entities.Payroll;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Models.Payroll
{
    public class PayrollDetailsViewModel
    {
        public EmployeePayroll Payroll { get; set; }
        public IEnumerable<EmployeePayrollItem> PayrollItems { get; set; }
        public IEnumerable<EmployeePayrollDeduction> Deductions { get; set; }
        public IEnumerable<EmployeeAdjustment> Adjustments { get; set; }


        public decimal TotalPayrollItems
        {
            get
            {
                return PayrollItems != null ? PayrollItems.Sum(x => x.TotalAmount) : 0;
            }
        }

        //Adjustments
        public bool HasAdjustment
        {
            get
            {
                return Adjustments != null && Adjustments.Any();
            }
        }

        public decimal TotalAdjustments
        {
            get
            {
                var amount = 0m;
                if (HasAdjustment)
                {
                    foreach(var item in Adjustments)
                    {
                        amount += item.Adjustment.AdjustmentType == Entities.Enums.AdjustmentType.Less ? -item.Amount : item.Amount;
                    }
                }

                return amount;
            }
        }

        public bool HasDeductions
        {
            get
            {
                return Deductions != null && Deductions.Any();
            }
        }

        public decimal TotalDeductions
        {
            get
            {
                return HasDeductions ? Deductions.Sum(x => x.Amount) : 0;
            }
        }
    }
}