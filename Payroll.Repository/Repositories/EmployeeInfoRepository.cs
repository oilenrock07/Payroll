using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class EmployeeInfoRepository : Repository<EmployeeInfo>, IEmployeeInfoRepository
    {
        public EmployeeInfoRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeInfos;
        }

        public EmployeeInfo GetByEmployeeId(int employeeId)
        {
            return Find(x => x.Employee.EmployeeId == employeeId).FirstOrDefault();
        }
    }
}
