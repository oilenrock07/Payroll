using Payroll.Entities;
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
        decimal ComputeTax(decimal taxableAmount);
    }
}
