using System.Collections.Generic;

namespace Payroll.Entities.Seeder
{
    public class FrequencySeeds : ISeeders<Frequency>
    {
        public IEnumerable<Frequency> GetDefaultSeeds()
        {
            return new List<Frequency>
            {
                new Frequency { FrequencyName = "Weekly" },
                new Frequency { FrequencyName = "Bimonthly" },
                new Frequency { FrequencyName = "Monthly" },
            };
        }
    }
}
