using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;

namespace Payroll.Test.Service
{
    [TestClass]
    public class UserRoleServiceTest
    {
        [TestMethod]
        public void GetUserRoles()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();

            var userRepository = new UserRepository(databaseFactory);
            var roleRepository = new RoleRepository(databaseFactory);
            var userRoleRepository = new UserRoleRepository(databaseFactory);
            var userRoleService = new UserRoleService(userRepository, roleRepository, userRoleRepository);

            var users = userRoleService.GetUsers();
            Assert.IsNotNull(users);
        }
    }
}
