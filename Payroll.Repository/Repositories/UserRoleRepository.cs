using System;
using System.Collections.Generic;
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

        public virtual void UpdateUserRole(string userId, IEnumerable<string> roleIds)
        {
            //delete the existing
            ExecuteSqlCommand(String.Format("DELETE FROM AspNetUserRoles WHERE UserId='{0}'", userId));

            //add the new one
            foreach (var roleId in roleIds)
            {
                Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
        }
    }
}
