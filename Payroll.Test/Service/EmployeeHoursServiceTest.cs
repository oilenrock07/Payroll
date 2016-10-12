using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
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
        private EmployeeHoursService employeeHoursService;
        private Employee employee;
        private int paymentFrequencyId;
        private IAttendanceRepository attendanceRepository;

        public void InitiateServiceAndTestData(List<Attendance> dataAttendance)
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService, employeeHoursRepository);

            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);

            employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);

            DeleteInfo(employeeRepository, unitOfWork);

            paymentFrequencyId = 1;

            employee = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            /*var frequency = new Frequency
            {
                FrequencyName = "Weekly",
                FrequencyType = Entities.Enums.FrequencyType.Weekly
            };*/

            //var paymentFrequency = new PaymentFrequency
            //{
            //    FrequencyType = FrequencyType.Weekly
            //};

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                SalaryFrequency = FrequencyType.Weekly
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
                EmployeeId = 1
            };

            employeeWorkScheduleRepository.Add(employeeWorkSchedule);

            //Save data
            foreach (var attendance in dataAttendance)
            {
                attendance.Employee = employee;

                attendanceService.Add(attendance);
            }

            unitOfWork.Commit();
        }

        public void DeleteInfo(EmployeeRepository repository, UnitOfWork unitOfwork)
        {
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            repository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            repository.ExecuteSqlCommand("TRUNCATE TABLE work_schedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");

            //unitOfwork.Commit();
        }

        /*
          Regular 8 hours
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursTest()
        {
            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                {
                    // Standard time
                    new Attendance()
                    {
                        AttendanceId = attendanceId1,
                        ClockIn = new DateTime(2016,2,1,7,0,0),
                        ClockOut = new DateTime(2016,2,1,12,0,0)
                    },
                    new Attendance()
                    {
                        AttendanceId = attendanceId2,
                        ClockIn = new DateTime(2016,2,1,13,0,0),
                        ClockOut = new DateTime(2016,2,1,16,0,0)
                    }
                };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(2, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }


        /*
          Regular 8 hours and 2 hours OT
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOT()
        {
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

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
          Regular 8 hours with OT and Excess Minutes within Regular Work Schedule
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOTAndExcessWithinRegularTime()
        {
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

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5.5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
         Regular 8 hours with OT and Excess Minutes within Regular Work Schedule
       */
        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOTAndExcessWithinRegularTime2()
        {
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
                                ClockIn = new DateTime(2016,2,1,12,30,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3.5,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
          Regular 8 hours with OT and Excess time within Scheduled working Hours and OT excess minutes 
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularHoursWithOTAndExcessOT()
        {
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
                                ClockOut = new DateTime(2016,2,1,18,6,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5.5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2.1,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
          Half day with Advance OT covered by Night Differential
        */
        [TestMethod]
        public void GenerateEmployeeHoursHalfDayOTWithNightDiff()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,30,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
          Advance OT covered by Night Differential
        */
        [TestMethod]
        public void GenerateEmployeeHoursOTWithNightDiff()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,4,0,0),
                                ClockOut = new DateTime(2016,2,1,7,59,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(2, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 3.98,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 3.98,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }
        /*
          Regular hours with OT covered by Night Differential
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiff()
        {
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

                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,23,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(4, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 7,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
          Regular hours with OT covered by Night Differential overnight
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffOvernight()
        {
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

                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,2,2,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(6, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 8,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateTo
            };

            var employeeHourEntry6 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
  Regular hours with OT covered by Night Differential overnight
*/
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffOvernight2()
        {
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

                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,2,7,30,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(6, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 8,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 7.5,
                Date = dateTo
            };

            var employeeHourEntry6 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 7.5,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }


        /*
          Regular hours with OT covered by Night Differential 
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffNextDay()
        {
            var attendanceId1 = 1;
            var attendanceId2 = 2;
            var attendanceId3 = 3;

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

                            // Standard time
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            },
                            //Next day
                            new Attendance()
                            {
                                AttendanceId = attendanceId3,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,23,0,0),
                                ClockOut = new DateTime(2016,2,2,2,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(7, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateFrom
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateFrom
            };

            var employeeHourEntry6 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateTo
            };

            var employeeHourEntry7 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            Assert.AreEqual(employeeHourEntry6.Type, employeeHours[5].Type);
            Assert.AreEqual(employeeHourEntry6.OriginAttendanceId, employeeHours[5].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry6.EmployeeId, employeeHours[5].EmployeeId);
            Assert.AreEqual(employeeHourEntry6.Hours, employeeHours[5].Hours);
            Assert.AreEqual(employeeHourEntry6.Date, employeeHours[5].Date);

            Assert.AreEqual(employeeHourEntry7.Type, employeeHours[6].Type);
            Assert.AreEqual(employeeHourEntry7.OriginAttendanceId, employeeHours[6].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry7.EmployeeId, employeeHours[6].EmployeeId);
            Assert.AreEqual(employeeHourEntry7.Hours, employeeHours[6].Hours);
            Assert.AreEqual(employeeHourEntry7.Date, employeeHours[6].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);

            //var attendance3 = attendanceRepository.GetById(3);
            //Assert.AreEqual(attendance3.IsHoursCounted, true);
        }

        /*
          OT covered by Night Differential only
        */
        [TestMethod]
        public void GenerateEmployeeHoursOTNightDiffNextDay()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,22,0,0),
                                ClockOut = new DateTime(2016,2,2,0,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(2, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
          OT covered by Night Differential only
        */
        [TestMethod]
        public void GenerateEmployeeHoursOTNightDiffNextDay2()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,18,0,0),
                                ClockOut = new DateTime(2016,2,1,23,30,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(2, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5.5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 1.5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
         Regular + OT covered by Night Differential
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffNextDay2()
        {
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
                            // Standard time with OT
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,2,0,30,0)
                            },
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(6, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 8,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = .5,
                Date = dateTo
            };

            var employeeHourEntry6 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = .5,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            Assert.AreEqual(employeeHourEntry6.Type, employeeHours[5].Type);
            Assert.AreEqual(employeeHourEntry6.OriginAttendanceId, employeeHours[5].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry6.EmployeeId, employeeHours[5].EmployeeId);
            Assert.AreEqual(employeeHourEntry6.Hours, employeeHours[5].Hours);
            Assert.AreEqual(employeeHourEntry6.Date, employeeHours[5].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
         Regular + OT covered by Night Differential
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularAdvanceOTOTNightDiffNextDay()
        {
            var attendanceId1 = 1;
            var attendanceId2 = 2;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,40,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           },
                            // Standard time with OT
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,2,2,30,0)
                            },
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(8, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.33,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.33,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 8,
                Date = dateFrom
            };

            var employeeHourEntry6 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry7 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2.5,
                Date = dateTo
            };

            var employeeHourEntry8 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2.5,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            Assert.AreEqual(employeeHourEntry6.Type, employeeHours[5].Type);
            Assert.AreEqual(employeeHourEntry6.OriginAttendanceId, employeeHours[5].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry6.EmployeeId, employeeHours[5].EmployeeId);
            Assert.AreEqual(employeeHourEntry6.Hours, employeeHours[5].Hours);
            Assert.AreEqual(employeeHourEntry6.Date, employeeHours[5].Date);

            Assert.AreEqual(employeeHourEntry7.Type, employeeHours[6].Type);
            Assert.AreEqual(employeeHourEntry7.OriginAttendanceId, employeeHours[6].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry7.EmployeeId, employeeHours[6].EmployeeId);
            Assert.AreEqual(employeeHourEntry7.Hours, employeeHours[6].Hours);
            Assert.AreEqual(employeeHourEntry7.Date, employeeHours[6].Date);

            Assert.AreEqual(employeeHourEntry8.Type, employeeHours[7].Type);
            Assert.AreEqual(employeeHourEntry8.OriginAttendanceId, employeeHours[7].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry8.EmployeeId, employeeHours[7].EmployeeId);
            Assert.AreEqual(employeeHourEntry8.Hours, employeeHours[7].Hours);
            Assert.AreEqual(employeeHourEntry8.Date, employeeHours[7].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        /*
         Regular + OT covered by Night Differential Next Day Clock in
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffClockInNextDay()
        {
            var attendanceId1 = 1;
            var attendanceId2 = 2;
            var attendanceId3 = 3;

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
                            // Standard time with OT
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            },
                            // Night dif
                            new Attendance()
                            {
                                AttendanceId = attendanceId3,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,2,1,0,0),
                                ClockOut = new DateTime(2016,2,2,2,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(5, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateTo
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            /*var attendance1 = attendanceRepository.GetById(1);
            Assert.AreEqual(attendance1.IsHoursCounted, true);

            var attendance2 = attendanceRepository.GetById(2);
            Assert.AreEqual(attendance2.IsHoursCounted, true);

            var attendance3 = attendanceRepository.GetById(3);
            Assert.AreEqual(attendance3.IsHoursCounted, true);*/
        }

        /*
         Regular + OT covered by Night Differential Next Day Clock in
        */
        [TestMethod]
        public void GenerateEmployeeHoursRegularOTNightDiffClockInNextDay2()
        {
            var attendanceId1 = 1;
            var attendanceId2 = 2;
            var attendanceId3 = 3;

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
                            // Standard time with OT
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,0,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            },
                            // Night dif
                            new Attendance()
                            {
                                AttendanceId = attendanceId3,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,2,5,0,0),
                                ClockOut = new DateTime(2016,2,2,7,30,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(5, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry4 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 2.5,
                Date = dateTo
            };

            var employeeHourEntry5 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId3,
                EmployeeId = employee.EmployeeId,
                Hours = 2.5,
                Date = dateTo
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            Assert.AreEqual(employeeHourEntry4.Type, employeeHours[3].Type);
            Assert.AreEqual(employeeHourEntry4.OriginAttendanceId, employeeHours[3].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry4.EmployeeId, employeeHours[3].EmployeeId);
            Assert.AreEqual(employeeHourEntry4.Hours, employeeHours[3].Hours);
            Assert.AreEqual(employeeHourEntry4.Date, employeeHours[3].Date);

            Assert.AreEqual(employeeHourEntry5.Type, employeeHours[4].Type);
            Assert.AreEqual(employeeHourEntry5.OriginAttendanceId, employeeHours[4].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry5.EmployeeId, employeeHours[4].EmployeeId);
            Assert.AreEqual(employeeHourEntry5.Hours, employeeHours[4].Hours);
            Assert.AreEqual(employeeHourEntry5.Date, employeeHours[4].Date);

            /*var attendance1 = attendanceRepository.GetById(1);
            Assert.AreEqual(attendance1.IsHoursCounted, true);

            var attendance2 = attendanceRepository.GetById(2);
            Assert.AreEqual(attendance2.IsHoursCounted, true);

            var attendance3 = attendanceRepository.GetById(3);
            Assert.AreEqual(attendance3.IsHoursCounted, true);*/
        }

        /*
        Advance OT with Regular Hours
        */
        [TestMethod]
        public void GenerateEmployeeHoursAdvanceOTWithRegularHours()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,5,0,0),
                                ClockOut = new DateTime(2016,2,1,8,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 1,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
        Advance OT with Regular Hours
        */
        [TestMethod]
        public void GenerateEmployeeHoursUndertime()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,12,30,0),
                                ClockOut = new DateTime(2016,2,1,14,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 1.5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
            Regular Hours with Late
        */
        [TestMethod]
        public void GenerateEmployeeHoursLate()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,6,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 4.9,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }


        /*
          Regular Hours with Late but Within Grace period
        */
        [TestMethod]
        public void GenerateEmployeeHoursWithinGracePeriod()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,5,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
         Regular Hours + OT Within Advance OT period
        */
        [TestMethod]
        public void GenerateEmployeeHoursWithinAdvanceOTPeriod()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,44,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);
            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);
        }

        /*
          Regular Hours + OT not Within Advance OT period
        */
        [TestMethod]
        public void GenerateEmployeeHoursNotWithinAdvanceOTPeriod()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,45,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        /*
          Regular 8 hours with OT and Excess Minutes within Regular Work Schedule
        */
        [TestMethod]
        public void GenerateEmployeeHoursCheckMinMinutes()
        {
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
                                ClockOut = new DateTime(2016,2,1,12,4,3)
                            },
                            new Attendance()
                            {
                                AttendanceId = attendanceId2,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,12,30,0),
                                ClockOut = new DateTime(2016,2,1,18,0,0)
                            }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 3.5,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId2,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

            //var attendance2 = attendanceRepository.GetById(2);
            //Assert.AreEqual(attendance2.IsHoursCounted, true);
        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario1()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,43,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario2()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,59,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario3()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,6,42,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(3, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.3,
                Date = dateFrom
            };

            var employeeHourEntry2 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            var employeeHourEntry3 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.NightDifferential,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 0.3,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            Assert.AreEqual(employeeHourEntry2.Type, employeeHours[1].Type);
            Assert.AreEqual(employeeHourEntry2.OriginAttendanceId, employeeHours[1].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry2.EmployeeId, employeeHours[1].EmployeeId);
            Assert.AreEqual(employeeHourEntry2.Hours, employeeHours[1].Hours);
            Assert.AreEqual(employeeHourEntry2.Date, employeeHours[1].Date);

            Assert.AreEqual(employeeHourEntry3.Type, employeeHours[2].Type);
            Assert.AreEqual(employeeHourEntry3.OriginAttendanceId, employeeHours[2].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry3.EmployeeId, employeeHours[2].EmployeeId);
            Assert.AreEqual(employeeHourEntry3.Hours, employeeHours[2].Hours);
            Assert.AreEqual(employeeHourEntry3.Date, employeeHours[2].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario4()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,03,0),
                                ClockOut = new DateTime(2016,2,1,12,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };
       
            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario5()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,13,05,0),
                                ClockOut = new DateTime(2016,2,1,16,0,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 3,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario6()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,07,04,0),
                                ClockOut = new DateTime(2016,2,1,10,15,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 3.25,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario7()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,16,0,0),
                                ClockOut = new DateTime(2016,2,1,17,59,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.OverTime,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }


        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario8()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,0,0),
                                ClockOut = new DateTime(2016,2,1,11,59,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario9()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,0,0),
                                ClockOut = new DateTime(2016,2,1,12,05,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 5,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario10()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,14,0,0),
                                ClockOut = new DateTime(2016,2,1,16,05,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 2,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }

        [TestMethod]
        public void GenerateEmployeeHoursExcessHoursScenario11()
        {
            var attendanceId1 = 1;

            var dataAttendance = new List<Attendance>
                        {
                            // Standard time
                            new Attendance()
                           {
                                AttendanceId = attendanceId1,
                                Employee = employee,
                                ClockIn = new DateTime(2016,2,1,7,06,0),
                                ClockOut = new DateTime(2016,2,1,12,00,0)
                           }
                        };

            InitiateServiceAndTestData(dataAttendance);

            var dateFrom = DateTime.Parse("2016-02-01 00:00:00");
            var dateTo = DateTime.Parse("2016-02-02 00:00:00");

            employeeHoursService.GenerateEmployeeHours(dateFrom, dateTo);

            var employeeHours = employeeHoursService.GetByEmployeeAndDateRange(employee.EmployeeId, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
            Assert.AreEqual(1, employeeHours.Count);

            var employeeHourEntry1 = new EmployeeHours
            {
                Type = Entities.Enums.RateType.Regular,
                OriginAttendanceId = attendanceId1,
                EmployeeId = employee.EmployeeId,
                Hours = 4.9,
                Date = dateFrom
            };

            Assert.AreEqual(employeeHourEntry1.Type, employeeHours[0].Type);
            Assert.AreEqual(employeeHourEntry1.OriginAttendanceId, employeeHours[0].OriginAttendanceId);
            Assert.AreEqual(employeeHourEntry1.EmployeeId, employeeHours[0].EmployeeId);
            Assert.AreEqual(employeeHourEntry1.Hours, employeeHours[0].Hours);
            Assert.AreEqual(employeeHourEntry1.Date, employeeHours[0].Date);

            //var attendance1 = attendanceRepository.GetById(1);
            //Assert.AreEqual(attendance1.IsHoursCounted, true);

        }
    }
}
