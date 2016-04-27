using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Seeder
{
    public class DeductionSeeds : ISeeders<Deduction>
    {
        public IEnumerable<Deduction> GetDefaultSeeds()
        {
            return new List<Deduction>
            {
                new Deduction {DeductionName = "Tax", Remarks = "Tax Deductions"},
                new Deduction {DeductionName = "HDMF", Remarks = "HDMF / Pag-Ibig Contribution"},
                new Deduction {DeductionName = "SSS", Remarks = "SSS Contribution"},
                new Deduction {DeductionName = "Philhealth", Remarks = "Philhealth Contribution"}
            };
        }
    }
}
