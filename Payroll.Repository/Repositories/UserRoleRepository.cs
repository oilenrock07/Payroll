using System.Linq;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().UserRoles;
        }

        public virtual UserRole FindUserByRole(string userId, string roleId)
        {
            return Find(x => x.RoleId == roleId && x.UserId == userId).FirstOrDefault();
        }
    }
}
