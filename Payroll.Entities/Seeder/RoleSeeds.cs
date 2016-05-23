using System;
using System.Collections.Generic;
using Payroll.Entities.Users;

namespace Payroll.Entities.Seeder
{
    public class RoleSeeds : ISeeders<Role>
    {
        public IEnumerable<Role> GetDefaultSeeds()
        {
            return new List<Role>
            {
                new Role
                {
                    Id = "Admin",
                    Name = "Admin",
                    IsActive = true,
                    CreateDate = DateTime.Now
                },
                new Role
                {
                    Id = "Manager",
                    Name = "Manager",
                    IsActive = true,
                    CreateDate = DateTime.Now
                },
                new Role
                {
                    Id = "Encoder",
                    Name = "Encoder",
                    IsActive = true,
                    CreateDate = DateTime.Now
                },
                new Role
                {
                    Id = "Employee",
                    Name = "Employee",
                    IsActive = true,
                    CreateDate = DateTime.Now
                }
            };
        }
    }
}
