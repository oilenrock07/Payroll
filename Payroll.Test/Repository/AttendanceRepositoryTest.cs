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
            //Test Data
            var employeeCode1 = 1;
            var employeeCode2 = 2;

            var data = new List<Attendance>
            {
                new Attendance()
                {
                    AttendanceId = 1,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                    ClockOut = DateTime.Parse("201-02-02 06:50:00"),
                },
                new Attendance()
                {
                    AttendanceId = 2,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-02 07:00:00"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 3,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-02 01:00:00"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 4,
                    EmployeeId = employeeCode2,
                    ClockIn = DateTime.Parse("2016-02-02 02:00:00"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 5,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-03 00:00:00"),
                    ClockOut = null
                }
            }.AsQueryable();

            var dbSetAttendanceMock = new Mock<IDbSet<Attendance>>();
            dbSetAttendanceMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetAttendanceMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetAttendanceMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetAttendanceMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.Attendances).Returns(dbSetAttendanceMock.Object);
            context.Object.SaveChanges();
            var databaseFactory = new DatabaseFactory(context.Object);

            var attendanceRepository = new AttendanceRepository(databaseFactory);

            //Test

            var dateFrom = DateTime.Parse("2016-02-02");
            var dateTo = DateTime.Parse("2016-02-03");

            var attendance = attendanceRepository.GetAttendanceByDateRange(employeeCode1, dateFrom, dateTo);

            Assert.AreEqual(2, attendance.Count());
            Assert.AreEqual(3, attendance[0].AttendanceId);
            Assert.AreEqual(2, attendance[1].AttendanceId);

        }

        [TestMethod]
        public void GetLastAttendance()
        {
            var employeeCode1 = 1;
            var employeeCode2 = 2;

            var data = new List<Attendance>
            {
                new Attendance()
                {
                    AttendanceId = 1,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                    ClockOut = DateTime.Parse("201-02-02 06:50:00"),
                },
                new Attendance()
                {
                    AttendanceId = 2,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                    ClockOut = null
                },
                new Attendance()
                {
                    AttendanceId = 3,
                    EmployeeId = employeeCode1,
                    ClockIn = DateTime.Parse("2016-02-02 01:00:00"),
                    ClockOut = DateTime.Parse("201-02-02 06:50:00")
                },
                new Attendance()
                {
                    AttendanceId = 4,
                    EmployeeId = employeeCode2,
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
            context.Setup(x => x.Attendances).Returns(dbSetAttendanceMock.Object);
            context.Object.SaveChanges();
            var databaseFactory = new DatabaseFactory(context.Object);

            var attendanceRepository = new AttendanceRepository(databaseFactory);

            //Test
            var attendance = attendanceRepository.GetLastAttendance(employeeCode1);

            Assert.AreEqual(3, attendance.AttendanceId);

        }
    }
}
