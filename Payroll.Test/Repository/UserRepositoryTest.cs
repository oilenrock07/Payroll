using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Assert = NUnit.Framework.Assert;

namespace Payroll.Test.Repository
{
    /// <summary>
    /// Summary description for UserRepositoryTest2
    /// </summary>
    [TestClass]
    public class UserRepositoryTest
    {
        public UserRepositoryTest()
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
        public void GetEmployeeById()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var employeeRepository = new Repository<Employee>(databaseFactory);

            //Act
            var employee = employeeRepository.GetById(1001);

            //Asset
            Assert.NotNull(employee);
        }
    }
}
