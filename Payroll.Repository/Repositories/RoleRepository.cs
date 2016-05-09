using Payroll.Entities.Users;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Roles;
        }
    }
}
