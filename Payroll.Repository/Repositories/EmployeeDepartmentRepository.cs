using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Repositories
{
    public class EmployeeDepartmentRepository : Repository<EmployeeDepartment>, IEmployeeDepartmentRepository
    {
        public EmployeeDepartmentRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeDepartments;
        }
    }
}
