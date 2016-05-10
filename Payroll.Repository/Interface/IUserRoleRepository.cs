using System.Collections.Generic;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        UserRole FindUserByRole(string userId, string roleId);
        void UpdateUserRole(string userId, IEnumerable<string> roleIds);
    }
}
