using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities.Enums;
using System.Data.Entity;
using Moq;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;

namespace Payroll.Test.Service
{
    [TestClass]
    public class AttendanceServiceTest
    {
        [TestMethod]
        public void TestClockIn()
        {
        }

        [TestMethod]
        //Scenarios
        // * Last login is not within the date range
        // * Last login within date range
        public void CreateWorkSchedulesByDateRange()
        {
            var employeeId1 = 1;
            var employeeId2 = 2;
            var employeeId3 = 3;

            var dataAttendanceLog = new List<AttendanceLog>
            {
                // Will not be considered
                 new AttendanceLog()
                    {
                        AttendanceLogId = 1,
                        EmployeeId = employeeId1,
                        ClockInOut = DateTime.Parse("2016-02-02 00:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 log in
                 new AttendanceLog()
                    {
                        AttendanceLogId = 2,
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },

                 // Employee 1 Logout
                 new AttendanceLog
                    {
                        AttendanceLogId = 3,
                        EmployeeId = employeeId1,
                        ClockInOut = DateTime.Parse("2016-02-02 04:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 Clockout
                 new AttendanceLog
                    {
                        AttendanceLogId = 4,
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 12:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 //Employee 3 login
                 new AttendanceLog
                    {
                        AttendanceLogId = 5,
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 login
                 new AttendanceLog
                    {
                        AttendanceLogId = 6,
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 13:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 3 logout
                 new AttendanceLog
                    {
                        AttendanceLogId = 7,
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 logout
                 new AttendanceLog
                    {
                        AttendanceLogId = 8,
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Will not be considered
                 new AttendanceLog
                    {
                        AttendanceLogId = 9,
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-03 00:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    }
            }.AsQueryable();

            var dataAttendance = new List<Attendance>
                {
                    new Attendance()
                    {
                        AttendanceId = 1,
                        EmployeeId = employeeId1,
                        ClockIn = DateTime.Parse("2016-02-01 23:00:00"),
                        ClockOut = null
                    },
                    new Attendance()
                    {
                        AttendanceId = 2,
                        EmployeeId = employeeId2,
                        ClockIn = DateTime.Parse("2016-02-01 23:30:00"),
                        ClockOut = DateTime.Parse("2016-02-01 23:55:00")
                    }
                }.AsQueryable();

            var dbSetAttendanceLogMock = new Mock<IDbSet<AttendanceLog>>();
            dbSetAttendanceLogMock.Setup(m => m.Provider).Returns(dataAttendanceLog.Provider);
            dbSetAttendanceLogMock.Setup(m => m.Expression).Returns(dataAttendanceLog.Expression);
            dbSetAttendanceLogMock.Setup(m => m.ElementType).Returns(dataAttendanceLog.ElementType);
            dbSetAttendanceLogMock.Setup(m => m.GetEnumerator()).Returns(dataAttendanceLog.GetEnumerator());

            var dbSetAttendanceMock = new Mock<IDbSet<Attendance>>();
            dbSetAttendanceMock.Setup(m => m.Provider).Returns(dataAttendance.Provider);
            dbSetAttendanceMock.Setup(m => m.Expression).Returns(dataAttendance.Expression);
            dbSetAttendanceMock.Setup(m => m.ElementType).Returns(dataAttendance.ElementType);
            dbSetAttendanceMock.Setup(m => m.GetEnumerator()).Returns(dataAttendance.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.AttendanceLog).Returns(dbSetAttendanceLogMock.Object);
            context.Setup(x => x.Attendances).Returns(dbSetAttendanceMock.Object);

            context.Object.SaveChanges();

            var databaseFactory = new DatabaseFactory(context.Object);
           
            var unitOfWork = new UnitOfWork(databaseFactory);
            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository, unitOfWork);
            var attendanceRepository = new AttendanceRepository(databaseFactory);

            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);

            var dateFrom = DateTime.Parse("2016-02-02 00:00:00");
            var dateTo = DateTime.Parse("2016-02-03 00:00:00");

            attendanceService.CreateWorkSchedulesByDateRange(dateFrom, dateTo);

            var attendanceListEmployee1 = attendanceService.GetAttendanceByDateRange(employeeId1, dateFrom, dateTo);

            Assert.AreEqual(1, attendanceListEmployee1.Count());
            Assert.AreEqual(1, attendanceListEmployee1[0].AttendanceId);
            Assert.AreEqual(DateTime.Parse("2016-02-01 23:00:00"), attendanceListEmployee1[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 04:00:00"), attendanceListEmployee1[0].ClockOut);

            var attendanceListEmployee2 = attendanceService.GetAttendanceByDateRange(employeeId2, dateFrom, dateTo);

            Assert.AreEqual(2, attendanceListEmployee2.Count());
            Assert.AreEqual(DateTime.Parse("2016-02-02 07:00:00"), attendanceListEmployee2[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 12:00:00"), attendanceListEmployee2[0].ClockOut);
            Assert.AreEqual(DateTime.Parse("2016-02-02 13:00:00"), attendanceListEmployee2[1].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 18:00:00"), attendanceListEmployee2[1].ClockOut);

            var attendanceListEmployee3 = attendanceService.GetAttendanceByDateRange(employeeId3, dateFrom, dateTo);

            Assert.AreEqual(1, attendanceListEmployee3.Count());
            Assert.AreEqual(DateTime.Parse("2016-02-02 07:00:00"), attendanceListEmployee3[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 18:00:00"), attendanceListEmployee3[0].ClockOut);

        }
    }
}
