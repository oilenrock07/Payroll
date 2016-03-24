using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
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
            var frequencyRepository = new FrequencyRepository(databaseFactory);
            var paymentFrequencyRepository = new PaymentFrequencyRepository(databaseFactory);
        
            var attendanceLogService = new AttendanceLogService(unitOfWork, attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService) ;
            var employeeService = new EmployeeService(employeeRepository, unitOfWork);
            var settingService = new SettingService(settingRepository, unitOfWork);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);
            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, attendanceService, settingService, employeeWorkScheduleService);

            var frequency = new Frequency
            {
                FrequencyName = "Weekly",
            };

            frequency = frequencyRepository.Add(frequency);

            var paymentFrequency = new PaymentFrequency
            {
                Frequency = frequency,
                FrequencyId = frequency.FrequencyId,
                IsActive = true
            };

            paymentFrequency = paymentFrequencyRepository.Add(paymentFrequency);

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

            employee = employeeRepository.Add(employee);

            var employeeId1 = employee.EmployeeId;

            var employeeInfo = new EmployeeInfo
            {
                EmployeeId = employeeId1,
                PaymentFrequencyId = paymentFrequency.PaymentFrequencyId,
            };

            employeeInfoRepository.Add(employeeInfo);
          
            var dataAttendance = new List<Attendance>
                {
                    // Standard time
                    new Attendance()
                    {
                        AttendanceId = 1,
                        EmployeeId = employeeId1,
                        ClockIn = DateTime.Parse("2016-02-01 07:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 12:00:00")
                    },
                    new Attendance()
                    {
                        AttendanceId = 2,
                        EmployeeId = employeeId1,
                        ClockIn = DateTime.Parse("2016-02-01 13:00:00"),
                        ClockOut = DateTime.Parse("2016-02-01 16:00:00")
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

            employeeHoursService.GenerateEmployeeHours(paymentFrequency.PaymentFrequencyId, dateFrom, dateTo);

            var employeeHours = employeeHoursRepository.GetByEmployeeAndDateRange(employeeId1, dateFrom, dateTo);

            Assert.IsNotNull(employeeHours);
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
