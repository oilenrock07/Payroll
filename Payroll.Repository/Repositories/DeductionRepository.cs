using Payroll.Entities.Payroll;
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
    public class DeductionRepository : Repository<Deduction>, IDeductionRepository
    {
        public DeductionRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Deductions;
        }

        public Deduction GetByName(string name)
        {
            return Find(d => d.IsActive && d.DeductionName == name).FirstOrDefault();
        }

        public IList<Deduction> GetAllCustomizable()
        {
            return Find(d => d.IsActive && d.IsCustomizable).ToList();
        }
    }
}
