using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;

namespace Payroll.Test.Service
{
    /// <summary>
    /// Summary description for HolidayServiceTest
    /// </summary>
    [TestClass]
    public class HolidayServiceTest
    {
        public HolidayServiceTest()
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GoodFridaysAreAllFridayTest()
        {
            //arrange
            //var payrollContext = new PayrollContext();
            //var databasefactory = new DatabaseFactory(payrollContext);
            //var holidayRepository = new HolidayRepository(databasefactory);

            //no need to pass the neccessary interfaces since im only going to test the date values
            var holidayService = new HolidayService(null, null, null);
            bool hasNotFriday = false;

            foreach (var date in holidayService.GoodFridays)
            {
                if (date.Value.DayOfWeek != DayOfWeek.Friday)
                {
                    hasNotFriday = true;
                    break;
                }
            }

            Assert.IsFalse(hasNotFriday);
        }
    }
}
