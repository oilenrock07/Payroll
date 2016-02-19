using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
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
            var data = new List<Employee>
            {
                new Employee() {FirstName = "Cawi", BirthDate = new DateTime(1989, 10, 30)},
                new Employee() {FirstName = "Jona", BirthDate = new DateTime(1992, 02, 02)}
            }.AsQueryable();

            var dbSetEmployeesMock = new Mock<IDbSet<Employee>>();
            dbSetEmployeesMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetEmployeesMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetEmployeesMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetEmployeesMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.Employees).Returns(dbSetEmployeesMock.Object);
            context.Object.SaveChanges();
            var databaseFactory = new DatabaseFactory(context.Object);


            var employeeRepository = new EmployeeRepository(databaseFactory);
            Assert.AreEqual(employeeRepository.GetAll().Count(), 2);
        }
    }
}
