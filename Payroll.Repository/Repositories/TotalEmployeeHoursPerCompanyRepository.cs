using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class TotalEmployeeHoursPerCompanyRepository : Repository<TotalEmployeeHoursPerCompany>, ITotalEmployeeHoursPerCompanyRepository
    {
        public TotalEmployeeHoursPerCompanyRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            
        }
    }
}
