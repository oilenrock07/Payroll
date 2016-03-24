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
    public class FrequencyRepository : Repository<Frequency>, IFrequencyRepository
    {
        public FrequencyRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Frequencies;
        }
    }
}
