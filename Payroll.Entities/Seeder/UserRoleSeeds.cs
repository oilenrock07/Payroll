using System;
using System.Collections.Generic;
using Payroll.Entities.Users;

namespace Payroll.Entities.Seeder
{
    public class UserRoleSeeds : ISeeders<UserRole>
    {
        public IEnumerable<UserRole> GetDefaultSeeds()
        {
            return new List<UserRole>
            {
                new UserRole
                {
                    UserId = "289da4b9-7b7c-4d9c-8b65-9a74eea9ac46",
                    RoleId = "Admin",
                    IsActive = true,
                    CreateDate = DateTime.Now
                }
            };
        }
    }
}
