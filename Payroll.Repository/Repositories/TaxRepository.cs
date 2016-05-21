using Payroll.Entities;
using Payroll.Entities.Enums;
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
    public class TaxRepository : Repository<Tax>, ITaxRepository
    {
        public TaxRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Taxes;
        }

        public Tax GetByTaxableAmount(FrequencyType frequency, int numberOfDependents, decimal taxableAmount)
        {
            return Find(t => t.IsActive && t.Frequency == frequency
                && t.NoOfDependents == numberOfDependents
                && taxableAmount >= t.BaseAmount 
                && (taxableAmount < t.MaxAmount || t.MaxAmount == 0)).FirstOrDefault();
        }
    }
}
