using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeHoursServiceTest
    {
        public void DeleteInfo(EmployeeRepository repository)
        {
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");

        }

        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursTest()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);
            var employeeService = new EmployeeService(employeeRepository);
            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);
            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);

            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("DELETE FROM work_schedule");
            employeeRepository.ExecuteSqlCommand("DELETE FROM employee");

            var paymentFrequencyId = 1;

            var employee = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                Employee = employee
            };

            employeeWorkScheduleRepository.Add(employeeWorkSchedule);

            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                {
                    // Standard time
                    new Attendance()
                    {
                        AttendanceId = attendanceId1,
                        Employee = employee,
                        ClockIn = new DateTime(2016,2,1,7,0,0),
                        ClockOut = new DateTime(2016,2,1,12,0,0)
                    },
                    new Attendance()
                    {
                        AttendanceId = attendanceId2,
                        Employee = employee,
                        ClockIn = new DateTime(2016,2,1,13,0,0),
                        ClockOut = new DateTime(2016,2,1,16,0,0)
                    }
                };

            //Save data
            foreach (var attendance in dataAttendance)
            {
                attendanceService.Add(attendance);
            }

            unitOfWork.Commit();

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            /*********************************
             ******   REGULAR HOURS **********
             *********************************/
            employeeHoursService.GenerateEmployeeHours(paymentFrequencyId, dateFrom, dateTo);

            var employeeHours = employeeHoursRepository.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(2, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);

        }


        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOT()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);
            var employeeService = new EmployeeService(employeeRepository);
            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);
            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);

            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("DELETE FROM work_schedule");
            employeeRepository.ExecuteSqlCommand("DELETE FROM employee");

            var paymentFrequencyId = 1;

            var employee = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                Employee = employee
            };

            employeeWorkScheduleRepository.Add(employeeWorkSchedule);

            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                    {
                        // Standard time
                        new Attendance()
                        {
                            AttendanceId = attendanceId1,
                            Employee = employee,
                            ClockIn = new DateTime(2016,2,1,7,0,0),
                            ClockOut = new DateTime(2016,2,1,12,0,0)
                        },
                        new Attendance()
                        {
                            AttendanceId = attendanceId2,
                            Employee = employee,
                            ClockIn = new DateTime(2016,2,1,13,0,0),
                            ClockOut = new DateTime(2016,2,1,18,0,0)
                        }
                    };

            //Save data
            foreach (var attendance in dataAttendance)
            {
                attendanceService.Add(attendance);
            }

            unitOfWork.Commit();

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            /*****************************************
             ******   REGULAR HOURS with OT **********
             *****************************************/
            employeeHoursService.GenerateEmployeeHours(paymentFrequencyId, dateFrom, dateTo);

            var employeeHours = employeeHoursRepository.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
        }
    

        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOTAndExcessWithinRegularTime()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);
            var employeeService = new EmployeeService(employeeRepository);
            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);
            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);

            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("DELETE FROM work_schedule");
            employeeRepository.ExecuteSqlCommand("DELETE FROM employee");

            var paymentFrequencyId = 1;

            var employee = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                Employee = employee
            };

            employeeWorkScheduleRepository.Add(employeeWorkSchedule);

            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,0,0),
                                ClockOut = new DateTime(2016,2,1,12,30,0)
                            },
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            }
                        };

            //Save data
            foreach (var attendance in dataAttendance)
            {
                attendanceService.Add(attendance);
            }

            unitOfWork.Commit();

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            /*****************************************************************
             ******   REGULAR HOURS with OT and Excess with Regular **********
             *****************************************************************/
            employeeHoursService.GenerateEmployeeHours(paymentFrequencyId, dateFrom, dateTo);

            var employeeHours = employeeHoursRepository.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5.5
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
        }

        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOTAndExcessOT()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);
            var employeeService = new EmployeeService(employeeRepository);
            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);
            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);

            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("DELETE FROM work_schedule");
            employeeRepository.ExecuteSqlCommand("DELETE FROM employee");

            var paymentFrequencyId = 1;

            var employee = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                Employee = employee
            };

            employeeWorkScheduleRepository.Add(employeeWorkSchedule);

            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,0,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                            },
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,18,6,0)
                            }
                        };

            //Save data
            foreach (var attendance in dataAttendance)
            {
                attendanceService.Add(attendance);
            }

            unitOfWork.Commit();

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            /*****************************************************************
             ******   REGULAR HOURS with OT and Excess OT **********
             *****************************************************************/
            employeeHoursService.GenerateEmployeeHours(paymentFrequencyId, dateFrom, dateTo);

            var employeeHours = employeeHoursRepository.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2.1
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
        }
    }
}
