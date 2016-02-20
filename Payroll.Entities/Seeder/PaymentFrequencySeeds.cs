using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Seeder
{
    public class PaymentFrequencySeeds : ISeeders<PaymentFrequency>
    {
        public IEnumerable<PaymentFrequency> GetDefaultSeeds()
        {
            return new List<PaymentFrequency>()
            {
                new PaymentFrequency {IsActive = true, }
            };
        }
    }
}
