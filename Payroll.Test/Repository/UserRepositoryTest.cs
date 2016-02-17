using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
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
            var employee = employeeRepository.GetById(1);

            //Asset
            Assert.NotNull(employee);
        }


        [TestMethod]
        public void TestFakeData()
        {
            using (var context = new PayrollContext("test"))
            {
                context.Employees.Add(new Employee() { FirstName = "Cawi", BirthDate = new DateTime(1989, 10, 30) });
                context.Employees.Add(new Employee() { FirstName = "Jona", BirthDate = new DateTime(1992, 02, 02) });

                var databaseFactory = new DatabaseFactory(context);
                var unitOfWork = new UnitOfWork(databaseFactory);
                unitOfWork.Commit();

                var employeeRepository = new Repository<Employee>(databaseFactory);

                var employees = employeeRepository.GetAll();
                Assert.AreEqual(employees.Count(), 2);
            }
        }
    }
}
