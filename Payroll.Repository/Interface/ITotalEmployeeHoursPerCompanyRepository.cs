using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface ITotalEmployeeHoursPerCompanyRepository : IRepository<TotalEmployeeHoursPerCompany>
    {
        void DeleteByTotalEmployeeHoursPerCompanyIds(IEnumerable<int> ids);
    }
}
