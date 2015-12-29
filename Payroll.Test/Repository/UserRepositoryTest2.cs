using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Implementations;
using Assert = NUnit.Framework.Assert;

namespace Payroll.Test.Repository
{
    /// <summary>
    /// Summary description for UserRepositoryTest2
    /// </summary>
    [TestClass]
    public class UserRepositoryTest2
    {
        public UserRepositoryTest2()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void GetUserById()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var userRepository = new Repository<User>(databaseFactory);

            //Act
            var user = userRepository.GetById(1);

            //Asset
            Assert.NotNull(user);
        }
    }
}
