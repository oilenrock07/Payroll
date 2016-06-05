using Payroll.Entities.Seeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Seeder
{
    public class WorkScheduleSeeds : ISeeders<WorkSchedule>
    {
        public IEnumerable<WorkSchedule> GetDefaultSeeds()
        {
            return new List<WorkSchedule>(){
                //Frisco regular working hours
                    //7AM to 4PM, monday to saturday
                new WorkSchedule { TimeStart =  new TimeSpan(0, 7, 0, 0), TimeEnd =  new TimeSpan(0, 16, 0, 0), WeekStart=1, WeekEnd=6, WorkScheduleName = "Day" }
            };
        }
    }
}
