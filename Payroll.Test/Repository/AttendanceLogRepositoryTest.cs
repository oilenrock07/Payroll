using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
            var data = new List<AttendanceLog>
            {
                 new AttendanceLog()
                    {
                        AttendanceLogId = 1,
                        EmployeeId =1,
                        ClockInOut = DateTime.Parse("2016-02-02 06:50:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 new AttendanceLog
                    {
                        AttendanceLogId = 2,
                        EmployeeId = 1,
                        ClockInOut = DateTime.Parse("2016-02-02 06:51:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 3,
                        EmployeeId = 2,
                        ClockInOut = DateTime.Parse("2016-02-02 06:51:05"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 4,
                        EmployeeId = 2,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 5,
                        EmployeeId =2,
                        ClockInOut = DateTime.Parse("2016-02-02 18:05:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                   new AttendanceLog
                    {
                        AttendanceLogId = 6,
                        EmployeeId = 1,
                        ClockInOut = DateTime.Parse("2016-02-02 18:10:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 7,
                        EmployeeId =3,
                        ClockInOut = DateTime.Parse("2016-02-02 23:59:59"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 8,
                        EmployeeId = 3,
                        ClockInOut = DateTime.Parse("2016-02-03 00:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 9,
                        EmployeeId = 3,
                        ClockInOut = DateTime.Parse("2016-02-02 00:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                  new AttendanceLog
                    {
                        AttendanceLogId = 10,
                        EmployeeId = 3,
                        ClockInOut = DateTime.Parse("2016-02-02 01:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = true
                    }
            }.AsQueryable();

            var dbSetAttendanceLogMock = new Mock<IDbSet<AttendanceLog>>();
            dbSetAttendanceLogMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetAttendanceLogMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetAttendanceLogMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetAttendanceLogMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.AttendanceLog).Returns(dbSetAttendanceLogMock.Object);
            context.Object.SaveChanges();
            var databaseFactory = new DatabaseFactory(context.Object);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);

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

        [TestMethod]
        public void GetAttendanceWithName()
        {
            //This will connect to database not mock data
            //Arrange
            var payrollContext = new PayrollContext();
            var databaseFactory = new DatabaseFactory(payrollContext);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);

            var startDate = new DateTime(2016, 03, 06);
            var endDate = new DateTime(2016, 03, 07);
            
            //Act
            var result = attendanceLogRepository.GetAttendanceLogsWithName(startDate, endDate, 0);
            
            //Assert
            Assert.IsNotNull(result);
        }
    }
}
