using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface ICompanyRepository : IRepository<Company>
    {
        IEnumerable<Company> SearchCompany(string criteria);
    }
}
