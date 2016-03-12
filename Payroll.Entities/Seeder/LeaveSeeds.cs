using System.Collections.Generic;

namespace Payroll.Entities.Seeder
{
    public class LeaveSeeds : ISeeders<Leave>
    {
        public IEnumerable<Leave> GetDefaultSeeds()
        {
            return new List<Leave>
            {
                new Leave { LeaveName = "Sick" },
                new Leave { LeaveName = "Vacation" },
                new Leave { LeaveName = "Maternity" },
                new Leave { LeaveName = "Paternity" },
            };
        }
    }
}
