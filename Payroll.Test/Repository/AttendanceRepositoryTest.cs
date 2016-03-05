using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Payroll.Test.Repository
{
    [TestClass]
    public class AttendanceRepositoryTest
    {
        private TestContext testContextInstance;

        public AttendanceRepositoryTest()
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
        public void GetAttendanceByDateRange()
        {
            var databaseFactory = new DatabaseFactory();
            var attendanceRepository = new AttendanceRepository(databaseFactory);

            //Test Data
            var attendance1 = new Attendance()
            {
                AttendanceId = 1,
                EmployeeCode = "001",
                ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                ClockOut = DateTime.Parse("201-02-02 06:50:00"),
            };

        }

        [TestMethod]
        public void GetLastAttendance()
        {
            var employeeCode1 = "001";
            var employeeCode2 = "002";

            var data = new List<Attendance>
            {
                new Attendance()
                {
                    AttendanceId = 1,
                    EmployeeCode = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                    ClockOut = DateTime.Parse("201-02-02 06:50:00"),
                },
                new Attendance()
                {
                    AttendanceId = 2,
                    EmployeeCode = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 3,
                    EmployeeCode = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-02 01:00:00"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 4,
                    EmployeeCode = employeeCode2,
                    ClockIn = DateTime.Parse("2016-02-02 02:00:00"),
                    ClockOut = null
                }
            }.AsQueryable();

            var dbSetAttendanceMock = new Mock<IDbSet<Attendance>>();
            dbSetAttendanceMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetAttendanceMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetAttendanceMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetAttendanceMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.Attendance).Returns(dbSetAttendanceMock.Object);
            context.Object.SaveChanges();
            var databaseFactory = new DatabaseFactory(context.Object);

            var attendanceRepository = new AttendanceRepository(databaseFactory);

            //Test
            var attendance = attendanceRepository.GetLastAttendance(employeeCode2);

            Assert.AreEqual(attendance.AttendanceId, 3);

        }
    }
}
