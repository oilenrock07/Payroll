using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Interface
{
    public interface IDeductionAmountRepository : IRepository<DeductionAmount>
    {
        IList<DeductionAmount> GetByDeduction(int deductionId);

        DeductionAmount GetByDeductionAndAmount(int deductionId,  decimal amount);
    }
}
