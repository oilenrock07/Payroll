using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
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
        [TestMethod]
        public void GenerateEmployeeHoursTest()
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
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService) ;
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

            /*
                    // Standard
                        // with OT
                    new Attendance()
                    {
                        AttendanceId = 3,
                        EmployeeId = employeeId2,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 4,
                        EmployeeId = employeeId2,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:00:00")
                    },

                    // Standard 
                        // with OT
                            // with butal 5 minutes
                                // after work shed
                    new Attendance()
                    {
                        AttendanceId = 5,
                        EmployeeId = employeeId3,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 6,
                        EmployeeId = employeeId3,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:05:00")
                    },

                     // Standard 
                        // with OT
                            // with butal 5 minutes
                                // after work shed
                    new Attendance()
                    {
                        AttendanceId = 7,
                        EmployeeId = employeeId4,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 8,
                        EmployeeId = employeeId4,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:05:00")
                    },

                     // Standard 
                        // with OT
                            // with butal 10 minutes
                                // lunch break
                    new Attendance()
                    {
                        AttendanceId = 9,
                        EmployeeId = employeeId5,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:10:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 10,
                        EmployeeId = employeeId5,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:00:00")
                    },

                    // Half day 
                    new Attendance()
                    {
                        AttendanceId = 11,
                        EmployeeId = employeeId6,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },

                    // Half day 
                        // with advanced OT
                    new Attendance()
                    {
                        AttendanceId = 12,
                        EmployeeId = employeeId7,
                        ClockIn = DateTime.Parse("2016-02-01 06:30:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },

                    // Standard 
                      // with OT
                         // with butal 10 minutes
                            // lunch break
                    new Attendance()
                    {
                        AttendanceId = 13,
                        EmployeeId = employeeId8,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:10:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 14,
                        EmployeeId = employeeId8,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:00:00")
                    },

                    // Standard 
                      // with OT
                         // with damag
                    new Attendance()
                    {
                        AttendanceId = 15,
                        EmployeeId = employeeId9,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 16,
                        EmployeeId = employeeId9,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 23:00:00")
                    },

                    // Standard 
                      // with OT
                         // with damag that overlaps scheduled time in equal to 1hr
                    new Attendance()
                    {
                        AttendanceId = 17,
                        EmployeeId = employeeId10,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 18,
                        EmployeeId = employeeId10,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 18:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 19,
                        EmployeeId = employeeId10,
                        ClockIn = DateTime.Parse("2016-02-02 05:00:00"),
                        ClockOut = DateTime.Parse("2016-02-02 08:30:00")
                    },*/
        }
    }
}
