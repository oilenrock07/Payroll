using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
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
    public class TotalEmployeeHoursServiceTest
    {
       [TestMethod]
        public void GenerateTotalByDateRange()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);
            var frequencyRepository = new FrequencyRepository(databaseFactory);
            var paymentFrequencyRepository = new PaymentFrequencyRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);
            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);

            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository, 
                attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);
            var totalEmployeeHoursService = new TotalEmployeeHoursService(unitOfWork, totalEmployeeHoursRepository, employeeHoursService, settingService);

            //Delete info
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");

            unitOfWork.Commit();

            var employee1 = new Employee
            {
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
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var frequency = new Frequency
            {
                FrequencyName = "Weekly",
                FrequencyType = Entities.Enums.FrequencyType.Weekly
            };

            frequencyRepository.Add(frequency);

            var paymentFrequency = new PaymentFrequency
            {
                 Frequency = frequency
            };

            paymentFrequencyRepository.Add(paymentFrequency);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                SalaryFrequency = FrequencyType.Weekly
            };

            employeeInfoRepository.Add(employeeInfo1);
            employeeInfoRepository.Add(employeeInfo2);

            /* "03/01/2016 - Employee 1 Regular Total hour 8 */
            var employeeHours1 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4,
                OriginAttendanceId = 1,
                Type = Entities.Enums.RateType.Regular
            };

            var employeeHours3 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/01/2016 - Employee 1 OT Total hour 3 */
            var employeeHours4 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 3,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/01/2016 - Employee 1 ND Total hour 1 */
            var employeeHours5 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 1,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* 03/01/2016 - Employee 2 OT Total hour 3 */
            var employeeHours2 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee2,
                Hours = 3,
                OriginAttendanceId = 2,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 1 OT Total hour 1 */
            var employeeHours8 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee1,
                Hours = 1,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 2 Regular Total hour 4 */
            var employeeHours6 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/02/2016 - Employee 2 NightDifferential Total hour 6 */
            var employeeHours7 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            var employeeHours10 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 2,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* NOT INCLUDED */
            var employeeHours9 = new EmployeeHours
            {
                Date = DateTime.Parse("03/03/2016"),
                Employee = employee1,
                Hours = 10,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            employeeHoursRepository.Add(employeeHours1);
            employeeHoursRepository.Add(employeeHours2);
            employeeHoursRepository.Add(employeeHours3);
            employeeHoursRepository.Add(employeeHours4);
            employeeHoursRepository.Add(employeeHours5);
            employeeHoursRepository.Add(employeeHours6);
            employeeHoursRepository.Add(employeeHours7);
            employeeHoursRepository.Add(employeeHours8);
            employeeHoursRepository.Add(employeeHours9);
            employeeHoursRepository.Add(employeeHours10);

            unitOfWork.Commit();

            DateTime dateFrom = DateTime.Parse("03/01/2016");
            DateTime dateTo = DateTime.Parse("03/02/2016");

            totalEmployeeHoursService.GenerateTotalByDateRange(dateFrom, dateTo);

            var totalEmployeeHours = totalEmployeeHoursService.GetByDateRange(dateFrom, dateTo);

            Assert.AreEqual(7, totalEmployeeHours.Count());

            Assert.AreEqual(1, totalEmployeeHours[0].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[0].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[0].Type);
            Assert.AreEqual(1, totalEmployeeHours[0].Hours);

            Assert.AreEqual(2, totalEmployeeHours[1].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[1].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[1].Type);
            Assert.AreEqual(4, totalEmployeeHours[1].Hours);

            Assert.AreEqual(2, totalEmployeeHours[2].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[2].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[2].Type);
            Assert.AreEqual(6, totalEmployeeHours[2].Hours);

            Assert.AreEqual(1, totalEmployeeHours[3].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[3].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[3].Type);
            Assert.AreEqual(8, totalEmployeeHours[3].Hours);

            Assert.AreEqual(1, totalEmployeeHours[4].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[4].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[4].Type);
            Assert.AreEqual(3, totalEmployeeHours[4].Hours);

            Assert.AreEqual(1, totalEmployeeHours[5].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[5].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[5].Type);
            Assert.AreEqual(1, totalEmployeeHours[5].Hours);

            Assert.AreEqual(2, totalEmployeeHours[6].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[6].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[6].Type);
            Assert.AreEqual(3, totalEmployeeHours[6].Hours);

            Assert.AreEqual(true, employeeHours1.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours2.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours3.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours4.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours5.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours6.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours7.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours8.IsIncludedInTotal);
            Assert.AreEqual(false, employeeHours9.IsIncludedInTotal);

        }

        [TestMethod]
        public void GenerateTotalByDateRangeWithExisting()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);

            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);

            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository,
                attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);
            var totalEmployeeHoursService = new TotalEmployeeHoursService(unitOfWork, totalEmployeeHoursRepository, employeeHoursService, settingService);

            //Delete info
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");

            unitOfWork.Commit();

            var employee1 = new Employee
            {
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
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            //var paymentFrequencyId = 1;

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                SalaryFrequency = FrequencyType.Hourly,
                //PaymentFrequencyId = paymentFrequencyId
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                SalaryFrequency = FrequencyType.Hourly,
                //PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo1);
            employeeInfoRepository.Add(employeeInfo2);

            /* "03/01/2016 - Employee 1 Regular Total hour 8 */
            var employeeHours1 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4,
                OriginAttendanceId = 1,
                Type = Entities.Enums.RateType.Regular
            };

            var employeeHours3 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/01/2016 - Employee 1 OT Total hour 3 */
            var employeeHours4 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 3,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/01/2016 - Employee 1 ND Total hour 1 */
            var employeeHours5 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 1,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* 03/01/2016 - Employee 2 OT Total hour 3 */
            var employeeHours2 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee2,
                Hours = 3,
                OriginAttendanceId = 2,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 1 OT Total hour 1 */
            var employeeHours8 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee1,
                Hours = 1,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 2 Regular Total hour 4 */
            var employeeHours6 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/02/2016 - Employee 2 NightDifferential Total hour 6 */
            var employeeHours7 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            var employeeHours10 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 2,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* NOT INCLUDED */
            var employeeHours9 = new EmployeeHours
            {
                Date = DateTime.Parse("03/03/2016"),
                Employee = employee1,
                Hours = 10,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            employeeHoursRepository.Add(employeeHours1);
            employeeHoursRepository.Add(employeeHours2);
            employeeHoursRepository.Add(employeeHours3);
            employeeHoursRepository.Add(employeeHours4);
            employeeHoursRepository.Add(employeeHours5);
            employeeHoursRepository.Add(employeeHours6);
            employeeHoursRepository.Add(employeeHours7);
            employeeHoursRepository.Add(employeeHours8);
            employeeHoursRepository.Add(employeeHours9);
            employeeHoursRepository.Add(employeeHours10);

            //Existing total employee hours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("03/01/2016"),
                Type = Entities.Enums.RateType.OverTime,
                Hours = 2
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("03/02/2016"),
                Type = Entities.Enums.RateType.NightDifferential,
                Hours = 4
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("03/01/2016"),
                Type = Entities.Enums.RateType.Regular,
                Hours = 8
            };

            totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            totalEmployeeHoursRepository.Add(totalEmployeeHours3);

            unitOfWork.Commit();

            DateTime dateFrom = DateTime.Parse("03/01/2016");
            DateTime dateTo = DateTime.Parse("03/02/2016");

            totalEmployeeHoursService.GenerateTotalByDateRange(dateFrom, dateTo);

            var totalEmployeeHours = totalEmployeeHoursService.GetByDateRange(dateFrom, dateTo);

            Assert.AreEqual(7, totalEmployeeHours.Count());

            Assert.AreEqual(1, totalEmployeeHours[0].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[0].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[0].Type);
            Assert.AreEqual(1, totalEmployeeHours[0].Hours);

            Assert.AreEqual(2, totalEmployeeHours[1].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[1].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[1].Type);
            Assert.AreEqual(4, totalEmployeeHours[1].Hours);

            Assert.AreEqual(2, totalEmployeeHours[2].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[2].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[2].Type);
            Assert.AreEqual(6, totalEmployeeHours[2].Hours);

            Assert.AreEqual(1, totalEmployeeHours[3].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[3].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[3].Type);
            Assert.AreEqual(8, totalEmployeeHours[3].Hours);

            Assert.AreEqual(1, totalEmployeeHours[4].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[4].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[4].Type);
            Assert.AreEqual(3, totalEmployeeHours[4].Hours);

            Assert.AreEqual(1, totalEmployeeHours[5].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[5].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[5].Type);
            Assert.AreEqual(1, totalEmployeeHours[5].Hours);

            /*Assert.AreEqual(2, totalEmployeeHours[6].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[6].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[6].Type);
            Assert.AreEqual(8, totalEmployeeHours[6].Hours);*/

            Assert.AreEqual(2, totalEmployeeHours[6].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[6].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[6].Type);
            Assert.AreEqual(3, totalEmployeeHours[6].Hours);

            Assert.AreEqual(true, employeeHours1.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours2.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours3.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours4.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours5.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours6.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours7.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours8.IsIncludedInTotal);
            Assert.AreEqual(false, employeeHours9.IsIncludedInTotal);

        }

        [TestMethod]
        public void GenerateTotalByDateRangeMinimumOT()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            var totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var attendanceRepository = new AttendanceRepository(databaseFactory);
            var attendanceLogRepository = new AttendanceLogRepository(databaseFactory, employeeRepository);
            var settingRepository = new SettingRepository(databaseFactory);
            var employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);

            var employeeInfoService = new EmployeeInfoService(employeeInfoRepository);
            var attendanceLogService = new AttendanceLogService(attendanceLogRepository);
            var attendanceService = new AttendanceService(unitOfWork, attendanceRepository, attendanceLogService);

            var settingService = new SettingService(settingRepository);
            var employeeWorkScheduleService = new EmployeeWorkScheduleService(employeeWorkScheduleRepository);

            var employeeHoursService = new EmployeeHoursService(unitOfWork, employeeHoursRepository,
                attendanceService, settingService, employeeWorkScheduleService, employeeInfoService);
            var totalEmployeeHoursService = new TotalEmployeeHoursService(unitOfWork, totalEmployeeHoursRepository, employeeHoursService, settingService);

            //Delete info
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            employeeRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            employeeRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");

            unitOfWork.Commit();

            var employee1 = new Employee
            {
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
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            //var paymentFrequencyId = 1;

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                SalaryFrequency = FrequencyType.Hourly,
                //PaymentFrequencyId = paymentFrequencyId
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                SalaryFrequency = FrequencyType.Hourly,
                //PaymentFrequencyId = paymentFrequencyId
            };

            employeeInfoRepository.Add(employeeInfo1);
            employeeInfoRepository.Add(employeeInfo2);

            /* "03/01/2016 - Employee 1 Regular Total hour 8 */
            var employeeHours1 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4.1,
                OriginAttendanceId = 1,
                Type = Entities.Enums.RateType.Regular
            };

            var employeeHours3 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 4,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/01/2016 - Employee 1 OT Total hour 3 */
            var employeeHours4 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 3.05,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/01/2016 - Employee 1 ND Total hour 1 */
            var employeeHours5 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee1,
                Hours = 1.06,
                OriginAttendanceId = 3,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* 03/01/2016 - Employee 2 OT Total hour 3 */
            var employeeHours2 = new EmployeeHours
            {
                Date = DateTime.Parse("03/01/2016"),
                Employee = employee2,
                Hours = 3.07,
                OriginAttendanceId = 2,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 1 OT Total hour 1 */
            var employeeHours8 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee1,
                Hours = 1.08,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            /* 03/02/2016 - Employee 2 Regular Total hour 4.09 */
            var employeeHours6 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4.09,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.Regular
            };

            /* 03/02/2016 - Employee 2 NightDifferential Total hour 6.5 */
            var employeeHours7 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 4,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            var employeeHours10 = new EmployeeHours
            {
                Date = DateTime.Parse("03/02/2016"),
                Employee = employee2,
                Hours = 2.5,
                OriginAttendanceId = 4,
                Type = Entities.Enums.RateType.NightDifferential
            };

            /* NOT INCLUDED */
            var employeeHours9 = new EmployeeHours
            {
                Date = DateTime.Parse("03/03/2016"),
                Employee = employee1,
                Hours = 10,
                OriginAttendanceId = 5,
                Type = Entities.Enums.RateType.OverTime
            };

            employeeHoursRepository.Add(employeeHours1);
            employeeHoursRepository.Add(employeeHours2);
            employeeHoursRepository.Add(employeeHours3);
            employeeHoursRepository.Add(employeeHours4);
            employeeHoursRepository.Add(employeeHours5);
            employeeHoursRepository.Add(employeeHours6);
            employeeHoursRepository.Add(employeeHours7);
            employeeHoursRepository.Add(employeeHours8);
            employeeHoursRepository.Add(employeeHours9);
            employeeHoursRepository.Add(employeeHours10);

            //Existing total employee hours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("03/01/2016"),
                Type = Entities.Enums.RateType.OverTime,
                Hours = 2
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("03/02/2016"),
                Type = Entities.Enums.RateType.NightDifferential,
                Hours = 4
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("03/01/2016"),
                Type = Entities.Enums.RateType.Regular,
                Hours = 8
            };

            totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            totalEmployeeHoursRepository.Add(totalEmployeeHours3);

            unitOfWork.Commit();

            DateTime dateFrom = DateTime.Parse("03/01/2016");
            DateTime dateTo = DateTime.Parse("03/02/2016");

            totalEmployeeHoursService.GenerateTotalByDateRange(dateFrom, dateTo);

            var totalEmployeeHours = totalEmployeeHoursService.GetByDateRange(dateFrom, dateTo);

            Assert.AreEqual(7, totalEmployeeHours.Count());

            Assert.AreEqual(1, totalEmployeeHours[0].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[0].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[0].Type);
            Assert.AreEqual(1, totalEmployeeHours[0].Hours);

            Assert.AreEqual(2, totalEmployeeHours[1].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[1].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[1].Type);
            Assert.AreEqual(4.09, totalEmployeeHours[1].Hours);

            Assert.AreEqual(2, totalEmployeeHours[2].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/02/2016"), totalEmployeeHours[2].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[2].Type);
            Assert.AreEqual(6.5, totalEmployeeHours[2].Hours);

            Assert.AreEqual(1, totalEmployeeHours[3].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[3].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[3].Type);
            Assert.AreEqual(8.1, totalEmployeeHours[3].Hours);

            Assert.AreEqual(1, totalEmployeeHours[4].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[4].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[4].Type);
            Assert.AreEqual(3, totalEmployeeHours[4].Hours);

            Assert.AreEqual(1, totalEmployeeHours[5].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[5].Date);
            Assert.AreEqual(Entities.Enums.RateType.NightDifferential, totalEmployeeHours[5].Type);
            Assert.AreEqual(1, totalEmployeeHours[5].Hours);

            /*Assert.AreEqual(2, totalEmployeeHours[6].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[6].Date);
            Assert.AreEqual(Entities.Enums.RateType.Regular, totalEmployeeHours[6].Type);
            Assert.AreEqual(8, totalEmployeeHours[6].Hours);*/

            Assert.AreEqual(2, totalEmployeeHours[6].EmployeeId);
            Assert.AreEqual(DateTime.Parse("03/01/2016"), totalEmployeeHours[6].Date);
            Assert.AreEqual(Entities.Enums.RateType.OverTime, totalEmployeeHours[6].Type);
            Assert.AreEqual(3, totalEmployeeHours[6].Hours);

            Assert.AreEqual(true, employeeHours1.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours2.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours3.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours4.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours5.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours6.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours7.IsIncludedInTotal);
            Assert.AreEqual(true, employeeHours8.IsIncludedInTotal);
            Assert.AreEqual(false, employeeHours9.IsIncludedInTotal);

        }
    }
}
