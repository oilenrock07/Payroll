using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Entities;
using Payroll.Entities.Enums;
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

            //Test Data
            var attendanceLog1 = new AttendanceLog
            {
                AttendanceLogId = 1,
                EmployeeCode = "001",
                ClockInOut = DateTime.Parse("2016-02-02 06:50:00"),
                Type = AttendanceType.ClockIn,
                IsRecorded = false
            };

            var attendanceLog2 = new AttendanceLog
            {
                AttendanceLogId = 2,
                EmployeeCode = "001",
                ClockInOut = DateTime.Parse("2016-02-02 06:51:00"),
                Type = AttendanceType.ClockIn,
                IsRecorded = false
            };
            
            var attendanceLog3 = new AttendanceLog
            {
                AttendanceLogId = 3,
                EmployeeCode = "002",
                ClockInOut = DateTime.Parse("2016-02-02 06:51:05"),
                Type = AttendanceType.ClockIn,
                IsRecorded = false
            };

            var attendanceLog4 = new AttendanceLog
            {
                AttendanceLogId = 4,
                EmployeeCode = "002",
                ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                Type = AttendanceType.ClockOut,
                IsRecorded = false
            };

            var attendanceLog5 = new AttendanceLog
            {
                AttendanceLogId = 5,
                EmployeeCode = "002",
                ClockInOut = DateTime.Parse("2016-02-02 18:05:00"),
                Type = AttendanceType.ClockOut,
                IsRecorded = false
            };

            var attendanceLog6 = new AttendanceLog
            {
                AttendanceLogId = 6,
                EmployeeCode = "001",
                ClockInOut = DateTime.Parse("2016-02-02 18:10:00"),
                Type = AttendanceType.ClockOut,
                IsRecorded = false
            };

            var attendanceLog7 = new AttendanceLog
            {
                AttendanceLogId = 7,
                EmployeeCode = "003",
                ClockInOut = DateTime.Parse("2016-02-02 23:59:59"),
                Type = AttendanceType.ClockIn,
                IsRecorded = false
            };

            var attendanceLog8 = new AttendanceLog
            {
                AttendanceLogId = 8,
                EmployeeCode = "003",
                ClockInOut = DateTime.Parse("2016-02-03 00:00:00"),
                Type = AttendanceType.ClockIn,
                IsRecorded = false
            };

            var attendanceLog9 = new AttendanceLog
            {
                AttendanceLogId = 9,
                EmployeeCode = "003",
                ClockInOut = DateTime.Parse("2016-02-02 00:00:00"),
                Type = AttendanceType.ClockOut,
                IsRecorded = false
            };

            var attendanceLog10 = new AttendanceLog
            {
                AttendanceLogId = 10,
                EmployeeCode = "003",
                ClockInOut = DateTime.Parse("2016-02-02 01:00:00"),
                Type = AttendanceType.ClockOut,
                IsRecorded = true
            };


            //TODO INSERT THE DATA
            var fromDate = DateTime.Parse("2016-02-02 00:00:00");
            var toDate = DateTime.Parse("2016-02-03 00:00:00");

            var attendanceLogs =
                attendanceLogRepository.GetAttendanceLogs(fromDate, toDate, false);

            Assert.IsNotNull(attendanceLogs);
            Assert.AreEqual(attendanceLogs.Count, 8);
            Assert.AreEqual(attendanceLogs[0].AttendanceLogId, 1);
            Assert.AreEqual(attendanceLogs[1].AttendanceLogId, 2);
            Assert.AreEqual(attendanceLogs[2].AttendanceLogId, 6);
            Assert.AreEqual(attendanceLogs[3].AttendanceLogId, 3);
            Assert.AreEqual(attendanceLogs[4].AttendanceLogId, 4);
            Assert.AreEqual(attendanceLogs[5].AttendanceLogId, 5);
            Assert.AreEqual(attendanceLogs[6].AttendanceLogId, 9);
            Assert.AreEqual(attendanceLogs[7].AttendanceLogId, 7);
        }
    }
}
