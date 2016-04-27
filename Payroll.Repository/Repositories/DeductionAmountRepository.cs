using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Repositories
{
    public class DeductionAmountRepository : Repository<DeductionAmount>, IDeductionAmountRepository
    {
        public DeductionAmountRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().DeductionAmounts;
        }

        public IList<DeductionAmount> GetByDeduction(int deductionId)
        {
            return Find(d => d.IsActive && d.DeductionId == deductionId).OrderBy(d => d.Frequency).ToList();
        }
        
        public DeductionAmount GetByDeductionAndAmount(int deductionId, decimal amount)
        {
            return Find(d => d.IsActive && amount >= d.MinBaseAmount && amount <= d.MaxBaseAmount).First();
        }
    }
}
