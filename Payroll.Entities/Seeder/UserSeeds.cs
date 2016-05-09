using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Users;

namespace Payroll.Entities.Seeder
{
    public class UserSeeds : ISeeders<User>
    {
        public IEnumerable<User> GetDefaultSeeds()
        {
            return new List<User>
            {
                new User
                {
                    Discriminator = "ApplicationUser",
                    FirstName = "admin",
                    Id = "289da4b9-7b7c-4d9c-8b65-9a74eea9ac46",
                    LastName = "admin",
                    PasswordHash = "ABVR6KcBnBVWJMrbWIGs36v5/g8ALNxoRQu3FeV4g1YsNRg/W16fd6PWfsoXRhUeVQ==",
                    SecurityStamp = "48ea89ef-a951-45dd-b8f7-ee2ec1d29a53",
                    UserName = "frisco",
                    IsActive = true,
                    CreateDate = DateTime.Now
                }
            };
        }
    }
}
