using System;
using System.Collections.Generic;
using Payroll.Entities.Users;

namespace Payroll.Repository.Models.User
{
    public class UserRoleDao
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<Role> Roles { get; set; }

        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}
