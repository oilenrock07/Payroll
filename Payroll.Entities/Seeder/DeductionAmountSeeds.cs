using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Seeder
{
    public class DeductionAmountSeeds : ISeeders<DeductionAmount>
    {
        public IEnumerable<DeductionAmount> GetDefaultSeeds()
        {
            return new List<DeductionAmount>
            {
                //Pag-ibig / HDMF
                new DeductionAmount { DeductionId = 3, Frequency = 1, MaxBaseAmount = 15000, MinBaseAmount = 0, Value = 1, IsPercentage = true},
                new DeductionAmount { DeductionId = 3, Frequency = 2, MaxBaseAmount = 0, MinBaseAmount = 15001, Value = 2, IsPercentage = true},

                //Philhealth
                new DeductionAmount { DeductionId = 2, Frequency = 1, MaxBaseAmount = 8999.99M, MinBaseAmount = 0, Value = 100, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 2, MaxBaseAmount = 9999.99M, MinBaseAmount = 9000, Value = 112.50M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 3, MaxBaseAmount = 10999.99M, MinBaseAmount = 10000, Value = 125, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 4, MaxBaseAmount = 11999.99M, MinBaseAmount = 11000, Value = 137.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 5, MaxBaseAmount = 12999.99M, MinBaseAmount = 12000, Value = 150M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 6, MaxBaseAmount = 13999.99M, MinBaseAmount = 13000, Value = 162.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 7, MaxBaseAmount = 14999.99M, MinBaseAmount = 14000, Value = 175, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 8, MaxBaseAmount = 15999.99M, MinBaseAmount = 15000, Value = 187.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 9, MaxBaseAmount = 16999.99M, MinBaseAmount = 16000, Value = 200, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 10, MaxBaseAmount = 17999.99M, MinBaseAmount = 17000, Value = 212.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 11, MaxBaseAmount = 18999.99M, MinBaseAmount = 18000, Value = 225, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 12, MaxBaseAmount = 19999.99M, MinBaseAmount = 19000, Value = 237.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 13, MaxBaseAmount = 20999.99M, MinBaseAmount = 20000, Value = 250, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 14, MaxBaseAmount = 21999.99M, MinBaseAmount = 21000, Value = 262.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 15, MaxBaseAmount = 22999.99M, MinBaseAmount = 22000, Value = 275, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 16, MaxBaseAmount = 23999.99M, MinBaseAmount = 23000, Value = 287, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 17, MaxBaseAmount = 24999.99M, MinBaseAmount = 24000, Value = 300, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 18, MaxBaseAmount = 25999.99M, MinBaseAmount = 25000, Value = 312.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 19, MaxBaseAmount = 26999.99M, MinBaseAmount = 26000, Value = 325, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 20, MaxBaseAmount = 27999.99M, MinBaseAmount = 27000, Value = 337, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 21, MaxBaseAmount = 28999.99M, MinBaseAmount = 28000, Value = 350, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 22, MaxBaseAmount = 29999.99M, MinBaseAmount = 29000, Value = 362, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 23, MaxBaseAmount = 30999.99M, MinBaseAmount = 30000, Value = 375, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 24, MaxBaseAmount = 31999.99M, MinBaseAmount = 31000, Value = 387.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 25, MaxBaseAmount = 32999.99M, MinBaseAmount = 32000, Value = 400, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 26, MaxBaseAmount = 33999.99M, MinBaseAmount = 33000, Value = 412.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 27, MaxBaseAmount = 34999.99M, MinBaseAmount = 34000, Value = 425, IsPercentage = false},
                new DeductionAmount { DeductionId = 2, Frequency = 28, MaxBaseAmount = 0, MinBaseAmount = 35000, Value = 437.5M, IsPercentage = false},

                //SSS
                new DeductionAmount { DeductionId = 3, Frequency = 1, MaxBaseAmount = 1249.99M, MinBaseAmount = 1000, Value = 36.3M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 2, MaxBaseAmount = 1749.99M, MinBaseAmount = 1250, Value = 54.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 3, MaxBaseAmount = 2249.99M, MinBaseAmount = 1750, Value = 72.7M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 4, MaxBaseAmount = 2279.99M, MinBaseAmount = 2250, Value = 90.80M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 5, MaxBaseAmount = 3249.99M, MinBaseAmount = 2750, Value = 109, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 6, MaxBaseAmount = 3749.99M, MinBaseAmount = 3250, Value = 127.2M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 7, MaxBaseAmount = 4729.99M, MinBaseAmount = 3750, Value = 145.3M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 8, MaxBaseAmount = 4749.99M, MinBaseAmount = 4250, Value = 163.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 9, MaxBaseAmount = 5249.99M, MinBaseAmount = 4750, Value = 181.7M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 10, MaxBaseAmount = 5749.99M, MinBaseAmount = 5250, Value = 199.8M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 11, MaxBaseAmount = 6249.99M, MinBaseAmount = 5750, Value = 218, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 12, MaxBaseAmount = 6749.99M, MinBaseAmount = 6250, Value = 236.2M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 13, MaxBaseAmount = 7249.99M, MinBaseAmount = 6750, Value = 254.3M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 14, MaxBaseAmount = 7749.99M, MinBaseAmount = 7250, Value = 272.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 15, MaxBaseAmount = 8249.99M, MinBaseAmount = 7750, Value = 290.7M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 16, MaxBaseAmount = 8749.99M, MinBaseAmount = 8250, Value = 308.8M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 17, MaxBaseAmount = 9249.99M, MinBaseAmount = 8750, Value = 327, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 18, MaxBaseAmount = 9749.99M, MinBaseAmount = 9250, Value = 345.2M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 19, MaxBaseAmount = 10249.99M, MinBaseAmount = 9750, Value = 363.3M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 20, MaxBaseAmount = 10749.99M, MinBaseAmount = 10250, Value = 381.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 21, MaxBaseAmount = 11249.99M, MinBaseAmount = 10750, Value = 399.7M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 22, MaxBaseAmount = 11749.99M, MinBaseAmount = 11250, Value = 417.8M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 23, MaxBaseAmount = 12249.99M, MinBaseAmount = 11750, Value = 436, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 24, MaxBaseAmount = 12749.99M, MinBaseAmount = 12250, Value = 454.2M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 25, MaxBaseAmount = 13249.99M, MinBaseAmount = 12750, Value = 472.3M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 26, MaxBaseAmount = 13749.99M, MinBaseAmount = 13250, Value = 490.5M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 27, MaxBaseAmount = 14249.99M, MinBaseAmount = 13750, Value = 508.7M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 28, MaxBaseAmount = 14749.99M, MinBaseAmount = 14250, Value = 526.8M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 29, MaxBaseAmount = 15249.99M, MinBaseAmount = 14750, Value = 545, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 30, MaxBaseAmount = 15749.99M, MinBaseAmount = 15250, Value = 563.2M, IsPercentage = false},
                new DeductionAmount { DeductionId = 3, Frequency = 31, MaxBaseAmount = 0, MinBaseAmount = 15750, Value = 581.3M, IsPercentage = false}
            };
        }
    }
}
