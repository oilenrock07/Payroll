using Payroll.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Seeder
{
    public class TaxSeeder : ISeeders<Tax>
    {
        public IEnumerable<Tax> GetDefaultSeeds()
        {
            return new List<Tax>
            {
                /* Monthly for Single and Married Employee*/
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=4167, BaseTaxAmount = 0,  OverPercentage = 5},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=5000, BaseTaxAmount = 41.67M,  OverPercentage = 10},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=6667, BaseTaxAmount = 208.33M,  OverPercentage = 15},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=10000, BaseTaxAmount = 708.33M,  OverPercentage = 20},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=15833, BaseTaxAmount = 1875,  OverPercentage = 25},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=25000, BaseTaxAmount = 4166.67M,  OverPercentage = 30},
                new Tax { Frequency = FrequencyType.Monthly, Code = "S/ME", NoOfDependents=0, BaseAmount=45833, BaseTaxAmount = 10416.67M,  OverPercentage = 32},

                /* Monthly for Single and Married Employee with 1 Qualified Dependent */
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=6250, BaseTaxAmount = 0,  OverPercentage = 5},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=7083, BaseTaxAmount = 41.67M,  OverPercentage = 10},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=8750, BaseTaxAmount = 208.33M,  OverPercentage = 15},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=12083, BaseTaxAmount = 708.33M,  OverPercentage = 20},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=17917, BaseTaxAmount = 1875,  OverPercentage = 25},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=27083, BaseTaxAmount = 4166.67M,  OverPercentage = 30},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S1", NoOfDependents=1, BaseAmount=47197, BaseTaxAmount = 45833,  OverPercentage = 32},

                /* Monthly for Single and Married Employee with 2 Qualified Dependent */
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=8333, BaseTaxAmount = 0,  OverPercentage = 5},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=9167, BaseTaxAmount = 41.67M,  OverPercentage = 10},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=10833, BaseTaxAmount = 208.33M,  OverPercentage = 15},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=14167, BaseTaxAmount = 708.33M,  OverPercentage = 20},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=20000, BaseTaxAmount = 1875,  OverPercentage = 25},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=29167, BaseTaxAmount = 4166.67M,  OverPercentage = 30},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S2", NoOfDependents=2, BaseAmount=50000, BaseTaxAmount = 45833,  OverPercentage = 32},

                /* Monthly for Single and Married Employee with 3 Qualified Dependent */
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=10417, BaseTaxAmount = 0,  OverPercentage = 5},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=11250, BaseTaxAmount = 41.67M,  OverPercentage = 10},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=12917, BaseTaxAmount = 208.33M,  OverPercentage = 15},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=16250, BaseTaxAmount = 708.33M,  OverPercentage = 20},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=22083, BaseTaxAmount = 1875,  OverPercentage = 25},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=31250, BaseTaxAmount = 4166.67M,  OverPercentage = 30},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S3", NoOfDependents=3, BaseAmount=52083, BaseTaxAmount = 45833,  OverPercentage = 32},

                /* Monthly for Single and Married Employee with 4 Qualified Dependent */
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=12500, BaseTaxAmount = 0,  OverPercentage = 5},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=13333, BaseTaxAmount = 41.67M,  OverPercentage = 10},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=15000, BaseTaxAmount = 208.33M,  OverPercentage = 15},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=18333, BaseTaxAmount = 708.33M,  OverPercentage = 20},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=24167, BaseTaxAmount = 1875,  OverPercentage = 25},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=33333, BaseTaxAmount = 4166.67M,  OverPercentage = 30},
                new Tax { Frequency = FrequencyType.Monthly, Code = "ME1/S4", NoOfDependents=4, BaseAmount=54167, BaseTaxAmount = 45833,  OverPercentage = 32}
            };
        }
    }
}
