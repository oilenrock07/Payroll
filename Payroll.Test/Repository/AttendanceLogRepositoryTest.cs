using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Repositories;
using System;

namespace Payroll.Test.Repository
{
    [TestClass]
    public class AttendanceLogRepositoryTest
    {
        private TestContext testContextInstance;

        public AttendanceLogRepositoryTest()
        {

        }

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
        public void GetAttendanceLogs()
        {
            var databaseFactory = new DatabaseFactory();
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory);

            var fromDate =  DateTime.Parse("2015-11-17 00:00:00");
            var toDate =    DateTime.Parse("2015-11-18 00:00:00");

            var attendanceLogs = 
                attendanceLogRepository.GetAttendanceLogs(fromDate, toDate, false);

            Assert.AreEqual(attendanceLogs.Count, 39);
            Assert.IsNotNull(attendanceLogs);
        }
    }
}
