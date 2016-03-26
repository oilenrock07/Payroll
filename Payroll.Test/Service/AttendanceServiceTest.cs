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
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
           
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);

            var employeeId1 = 1001;
            var employeeId2 = 2001;
            var employeeId3 = 3001;

            var dataAttendanceLog = new List<AttendanceLog>
            {
                // Will not be considered
                 new AttendanceLog()
                    {
                        EmployeeId = employeeId1,
                        ClockInOut = DateTime.Parse("2016-02-02 00:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 log in
                 new AttendanceLog()
                    {
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                    
                 // Employee 1 Logout
                 new AttendanceLog
                    {  
                        EmployeeId = employeeId1,
                        ClockInOut = DateTime.Parse("2016-02-02 04:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 Clockout
                 new AttendanceLog
                    {
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 12:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 //Employee 3 login
                 new AttendanceLog
                    {
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 login
                 new AttendanceLog
                    {
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 13:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 3 logout
                 new AttendanceLog
                    {
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 logout
                 new AttendanceLog
                    {
                        EmployeeId = employeeId2,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Will not be considered
                 new AttendanceLog
                    {
                        EmployeeId = employeeId3,
                        ClockInOut = DateTime.Parse("2016-02-03 00:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    }
            };

            var dataAttendance = new List<Attendance>
                {
                    new Attendance()
                    {
                        EmployeeId = employeeId1,
                        ClockIn = DateTime.Parse("2016-02-01 23:00:00"),
                        ClockOut = null,
                        IsManuallyEdited = false
                    },
                    new Attendance()
                    {
                        EmployeeId = employeeId2,
                        ClockIn = DateTime.Parse("2016-02-01 23:30:00"),
                        ClockOut = DateTime.Parse("2016-02-01 23:55:00"),
                        IsManuallyEdited = false
                    }
                };

            //Save test info
            foreach (var attendanceLog in dataAttendanceLog)
            {
                attendanceLogRepository.Add(attendanceLog);
                unitOfWork.Commit();
            }

            foreach (var attendance in dataAttendance)
            {
                attendanceRepository.Add(attendance);
                unitOfWork.Commit();
            }
   
            var dateFrom = DateTime.Parse("2016-02-02 00:00:00");
            var dateTo = DateTime.Parse("2016-02-03 00:00:00");

            attendanceService.CreateWorkSchedulesByDateRange(dateFrom, dateTo);
            unitOfWork.Commit();

            var attendanceListEmployee1 = attendanceService.GetAttendanceByDateRange(employeeId1, dateFrom, dateTo);
            
            Assert.AreEqual(1, attendanceListEmployee1.Count());
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

            //TODO Delete created data
           
        }
    }
}
