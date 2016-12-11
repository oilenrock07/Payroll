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
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);

            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService, employeeHoursRepository);

            //Delete Data
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");

            //Add employee
            var employee1 = new Employee
            {
                EmployeeId = 1,
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employee2 = new Employee
            {
                EmployeeId = 2,
                EmployeeCode = "11002",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employee3 = new Employee
            {
                EmployeeId = 3,
                EmployeeCode = "11003",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            //Insert employee
            employeeRepository.Add(employee1);
            employeeRepository.Add(employee2);
            employeeRepository.Add(employee3);

            var dataAttendanceLog = new List<AttendanceLog>
            {
                // Will not be considered
                new AttendanceLog()
                    {
                        EmployeeId = employee1.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 00:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 log in
                 new AttendanceLog()
                    {
                        EmployeeId = employee2.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                    
                 // Employee 1 Logout
                 new AttendanceLog
                    {  
                        EmployeeId = employee1.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 04:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 Clockout
                 new AttendanceLog
                    {
                        EmployeeId = employee2.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 12:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 //Employee 3 login
                 new AttendanceLog
                    {
                        EmployeeId = employee3.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 07:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 2 login
                 new AttendanceLog
                    {
                        EmployeeId = employee2.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 13:00:00"),
                        Type = AttendanceType.ClockIn,
                        IsRecorded = false
                    },
                 // Employee 3 logout
                 new AttendanceLog
                    {
                        EmployeeId = employee3.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Employee 2 logout
                 new AttendanceLog
                    {
                        EmployeeId = employee2.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-02 18:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    },
                 // Will not be considered
                 new AttendanceLog
                    {
                        EmployeeId = employee3.EmployeeId,
                        ClockInOut = DateTime.Parse("2016-02-03 00:00:00"),
                        Type = AttendanceType.ClockOut,
                        IsRecorded = false
                    }
            };

            var dataAttendance = new List<Attendance>
                {
                    new Attendance()
                    {
                        Employee = employee1,
                        ClockIn = DateTime.Parse("2016-02-01 23:00:00"),
                        ClockOut = null,
                        IsManuallyEdited = false
                    },
                    new Attendance()
                    {
                        Employee = employee2,
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

            attendanceService.CreateWorkSchedules();
           // unitOfWork.Commit();

            var attendanceListEmployee1 = attendanceService.GetAttendanceByDateRange(employee1.EmployeeId, dateFrom, dateTo);
            
            Assert.AreEqual(1, attendanceListEmployee1.Count());
            Assert.AreEqual(DateTime.Parse("2016-02-01 23:00:00"), attendanceListEmployee1[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 04:00:00"), attendanceListEmployee1[0].ClockOut);

            var attendanceListEmployee2 = attendanceService.GetAttendanceByDateRange(employee2.EmployeeId, dateFrom, dateTo);

            Assert.AreEqual(2, attendanceListEmployee2.Count());
            Assert.AreEqual(DateTime.Parse("2016-02-02 07:00:00"), attendanceListEmployee2[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 12:00:00"), attendanceListEmployee2[0].ClockOut);
            Assert.AreEqual(DateTime.Parse("2016-02-02 13:00:00"), attendanceListEmployee2[1].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 18:00:00"), attendanceListEmployee2[1].ClockOut);

            var attendanceListEmployee3 = attendanceService.GetAttendanceByDateRange(employee3.EmployeeId, dateFrom, dateTo);

            Assert.AreEqual(1, attendanceListEmployee3.Count());
            Assert.AreEqual(DateTime.Parse("2016-02-02 07:00:00"), attendanceListEmployee3[0].ClockIn);
            Assert.AreEqual(DateTime.Parse("2016-02-02 18:00:00"), attendanceListEmployee3[0].ClockOut);



        }

        [TestMethod]
        public void GetAttendanceHoursByDateTest()
        {
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);

            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService, employeeHoursRepository);

            var result = attendanceService.GetAttendanceAndHoursByDate(new DateTime(2016, 6, 8), new DateTime(2016, 6,8), 0);
        }
    }
}
