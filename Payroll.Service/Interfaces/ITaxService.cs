using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface ITaxService : IBaseEntityService<Tax>
    {
        decimal ComputeTax(FrequencyType frequency, int dependentCount, decimal totalTaxableIncome);
    }

}
