using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service;
using Payroll.Service.Implementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeDailyPayrollServiceTest
    {
        public void DeleteInfo(EmployeeRepository repository, UnitOfWork unitOfwork)
        {
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            repository.ExecuteSqlCommand("TRUNCATE TABLE holiday");
            repository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total"); 
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_daily_payroll");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_salary");
            repository.ExecuteSqlCommand("TRUNCATE TABLE work_schedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        //Test Regular Rate 
        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeRegular()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            TotalEmployeeHoursRepository _totalEmployeeHoursRepository = 
                new TotalEmployeeHoursRepository(databaseFactory);
            EmployeeHoursRepository _employeeHoursRepository = 
                new EmployeeHoursRepository(databaseFactory);
            EmployeeWorkScheduleRepository _employeeWorkScheduleRepository = 
                new EmployeeWorkScheduleRepository(databaseFactory);
            HolidayRepository _holidayRepository = 
                new HolidayRepository(databaseFactory);
            SettingRepository _settingsRepository = 
                new SettingRepository(databaseFactory);
            EmployeeDepartmentRepository _employeeDepartmentRepository = 
                new EmployeeDepartmentRepository(databaseFactory);
            EmployeeRepository _employeeRepository = 
                new EmployeeRepository(databaseFactory, _employeeDepartmentRepository);
            AttendanceLogRepository _attendanceLogRepository = 
                new AttendanceLogRepository(databaseFactory, _employeeRepository);
            AttendanceRepository _attendanceRepository = 
                new AttendanceRepository(databaseFactory);
            EmployeeInfoRepository _employeeInfoRepository = 
                new EmployeeInfoRepository(databaseFactory);
            EmployeeDailyPayrollRepository _employeeDailyPayrollRepository = 
                new EmployeeDailyPayrollRepository(databaseFactory);
            //EmployeeSalaryRepository _employeeSalaryRepository = 
            //    new EmployeeSalaryRepository(databaseFactory);

            EmployeeWorkScheduleService _employeeWorkScheduleService = 
                new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            SettingService _settingService = 
                new SettingService(_settingsRepository);
            HolidayService _holidayService = 
                new HolidayService(_holidayRepository, _settingsRepository, unitOfWork);
            AttendanceLogService _attendanceLogService = 
                new AttendanceLogService(_attendanceLogRepository);
            AttendanceService _attendanceService = 
                new AttendanceService(unitOfWork, _attendanceRepository, _attendanceLogService);

            EmployeeInfoService _employeeInfoService =
                new EmployeeInfoService(_employeeInfoRepository);
            EmployeeHoursService _employeeHoursService = 
                new EmployeeHoursService(unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            TotalEmployeeHoursService _totalEmployeeHoursService =
                new TotalEmployeeHoursService(unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService);
            EmployeeSalaryService _employeeSalaryService = new EmployeeSalaryService();
            EmployeeDailyPayrollService employeeDailyPayrollService = 
                new EmployeeDailyPayrollService(unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);

            DeleteInfo(_employeeRepository, unitOfWork);

            //Data
            var employeeId1 = 1;
            var employeeId2 = 2;

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
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("04/22/1989"),
                Gender = 1,
                IsActive = true
            };

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
                EmployeeId = employeeId1
            };

            var workSchedule2 = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule2,
                EmployeeId = employeeId2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                Salary = 100,
                SalaryFrequency = FrequencyType.Hourly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 3000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo1);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHoursRegular1 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 8,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular5 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular2 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 10.5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/22/2016")
            };

            var totalEmployeeHoursRegular3 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 0.5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/23/2016")
            };

            var totalEmployeeHoursRegular4 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 4.1,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/24/2016")
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular5);

            unitOfWork.Commit();

            //Test
            DateTime dateFrom = DateTime.Parse("04/21/2016");
            DateTime dateTo = DateTime.Parse("04/23/2016");

            employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            var employeeDailyPayroll = employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(employeeDailyPayroll);
            Assert.AreEqual(employeeDailyPayroll.Count, 4);

            var employeeDailyPayroll1 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 3,
                TotalPay = (decimal)48.75
            };

            var employeeDailyPayroll2 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/22/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = (decimal)1050
            };

            var employeeDailyPayroll3 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = (decimal)800
            };

            var employeeDailyPayroll4 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = (decimal)375
            };

            Assert.AreEqual(employeeDailyPayroll1.Date, employeeDailyPayroll[0].Date);
            Assert.AreEqual(employeeDailyPayroll1.EmployeeId, employeeDailyPayroll[0].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll1.TotalEmployeeHoursId, employeeDailyPayroll[0].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll1.TotalPay, employeeDailyPayroll[0].TotalPay);

            Assert.AreEqual(employeeDailyPayroll2.Date, employeeDailyPayroll[1].Date);
            Assert.AreEqual(employeeDailyPayroll2.EmployeeId, employeeDailyPayroll[1].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll2.TotalEmployeeHoursId, employeeDailyPayroll[1].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll2.TotalPay, employeeDailyPayroll[1].TotalPay);

            Assert.AreEqual(employeeDailyPayroll3.Date, employeeDailyPayroll[2].Date);
            Assert.AreEqual(employeeDailyPayroll3.EmployeeId, employeeDailyPayroll[2].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll3.TotalEmployeeHoursId, employeeDailyPayroll[2].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll3.TotalPay, employeeDailyPayroll[2].TotalPay);

            Assert.AreEqual(employeeDailyPayroll4.Date, employeeDailyPayroll[3].Date);
            Assert.AreEqual(employeeDailyPayroll4.EmployeeId, employeeDailyPayroll[3].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll4.TotalEmployeeHoursId, employeeDailyPayroll[3].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll4.TotalPay, employeeDailyPayroll[3].TotalPay);
        }

        //Test Regular Rate 
        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeOvertime()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            TotalEmployeeHoursRepository _totalEmployeeHoursRepository =
                new TotalEmployeeHoursRepository(databaseFactory);
            EmployeeHoursRepository _employeeHoursRepository =
                new EmployeeHoursRepository(databaseFactory);
            EmployeeWorkScheduleRepository _employeeWorkScheduleRepository =
                new EmployeeWorkScheduleRepository(databaseFactory);
            HolidayRepository _holidayRepository =
                new HolidayRepository(databaseFactory);
            SettingRepository _settingsRepository =
                new SettingRepository(databaseFactory);
            EmployeeDepartmentRepository _employeeDepartmentRepository =
                new EmployeeDepartmentRepository(databaseFactory);
            EmployeeRepository _employeeRepository =
                new EmployeeRepository(databaseFactory, _employeeDepartmentRepository);
            AttendanceLogRepository _attendanceLogRepository =
                new AttendanceLogRepository(databaseFactory, _employeeRepository);
            AttendanceRepository _attendanceRepository =
                new AttendanceRepository(databaseFactory);
            EmployeeInfoRepository _employeeInfoRepository =
                new EmployeeInfoRepository(databaseFactory);
            EmployeeDailyPayrollRepository _employeeDailyPayrollRepository =
                new EmployeeDailyPayrollRepository(databaseFactory);
            //EmployeeSalaryRepository _employeeSalaryRepository =
            //    new EmployeeSalaryRepository(databaseFactory);

            EmployeeWorkScheduleService _employeeWorkScheduleService =
                new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            SettingService _settingService =
                new SettingService(_settingsRepository);
            HolidayService _holidayService =
                new HolidayService(_holidayRepository, _settingsRepository, unitOfWork);
            AttendanceLogService _attendanceLogService =
                new AttendanceLogService(_attendanceLogRepository);
            AttendanceService _attendanceService =
                new AttendanceService(unitOfWork, _attendanceRepository, _attendanceLogService);

            EmployeeInfoService _employeeInfoService =
                new EmployeeInfoService(_employeeInfoRepository);
            EmployeeHoursService _employeeHoursService =
                new EmployeeHoursService(unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            TotalEmployeeHoursService _totalEmployeeHoursService =
                new TotalEmployeeHoursService(unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService);
            EmployeeSalaryService _employeeSalaryService = new EmployeeSalaryService();
            EmployeeDailyPayrollService employeeDailyPayrollService =
                new EmployeeDailyPayrollService(unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);

            DeleteInfo(_employeeRepository, unitOfWork);

            //Data
            var employeeId1 = 1;
            var employeeId2 = 2;

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
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("04/22/1989"),
                Gender = 1,
                IsActive = true
            };

            //var EmployeeSalary1 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 100,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Hourly
            //};

            //var EmployeeSalary2 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 3000,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Weekly
            //};

            //_employeeSalaryRepository.Add(EmployeeSalary1);
            //_employeeSalaryRepository.Add(EmployeeSalary2);

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
                EmployeeId = employeeId1
            };

            var workSchedule2 = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule2,
                EmployeeId = employeeId2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                Salary = 100,
                SalaryFrequency = Entities.Enums.FrequencyType.Hourly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                SalaryFrequency = FrequencyType.Weekly,
                Salary = 3000
            };

            _employeeInfoRepository.Add(employeeInfo1);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHoursRegular1 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 8,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular2 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 2,
                Type = Entities.Enums.RateType.OverTime,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular3 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 3.5,
                Type = Entities.Enums.RateType.OverTime,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular4 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 8,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/22/2016")
            };

            var totalEmployeeHoursRegular5 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 0.5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/23/2016")
            };

            var totalEmployeeHoursRegular6 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 4.1,
                Type = Entities.Enums.RateType.OverTime,
                Date = DateTime.Parse("04/23/2016")
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular5);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular6);

            unitOfWork.Commit();

            //Test
            DateTime dateFrom = DateTime.Parse("04/21/2016");
            DateTime dateTo = DateTime.Parse("04/23/2016");

            employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            var employeeDailyPayroll = employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(employeeDailyPayroll);
            Assert.AreEqual(employeeDailyPayroll.Count, 6);

            var employeeDailyPayroll1 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = (decimal)48.75
            };

            var employeeDailyPayroll2 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 6,
                TotalPay = (decimal)476.625
            };

            var employeeDailyPayroll3 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/22/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 4,
                TotalPay = (decimal)800
            };

            var employeeDailyPayroll4 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = (decimal)800
            };

            var employeeDailyPayroll5 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = (decimal)250
            };

            var employeeDailyPayroll6 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 3,
                TotalPay = (decimal)328.125
            };

            Assert.AreEqual(employeeDailyPayroll1.Date, employeeDailyPayroll[0].Date);
            Assert.AreEqual(employeeDailyPayroll1.EmployeeId, employeeDailyPayroll[0].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll1.TotalEmployeeHoursId, employeeDailyPayroll[0].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll1.TotalPay, employeeDailyPayroll[0].TotalPay);

            Assert.AreEqual(employeeDailyPayroll2.Date, employeeDailyPayroll[1].Date);
            Assert.AreEqual(employeeDailyPayroll2.EmployeeId, employeeDailyPayroll[1].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll2.TotalEmployeeHoursId, employeeDailyPayroll[1].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll2.TotalPay, employeeDailyPayroll[1].TotalPay);

            Assert.AreEqual(employeeDailyPayroll3.Date, employeeDailyPayroll[2].Date);
            Assert.AreEqual(employeeDailyPayroll3.EmployeeId, employeeDailyPayroll[2].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll3.TotalEmployeeHoursId, employeeDailyPayroll[2].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll3.TotalPay, employeeDailyPayroll[2].TotalPay);

            Assert.AreEqual(employeeDailyPayroll4.Date, employeeDailyPayroll[3].Date);
            Assert.AreEqual(employeeDailyPayroll4.EmployeeId, employeeDailyPayroll[3].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll4.TotalEmployeeHoursId, employeeDailyPayroll[3].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll4.TotalPay, employeeDailyPayroll[3].TotalPay);

            Assert.AreEqual(employeeDailyPayroll5.Date, employeeDailyPayroll[4].Date);
            Assert.AreEqual(employeeDailyPayroll5.EmployeeId, employeeDailyPayroll[4].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll5.TotalEmployeeHoursId, employeeDailyPayroll[4].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll5.TotalPay, employeeDailyPayroll[4].TotalPay);

            Assert.AreEqual(employeeDailyPayroll6.Date, employeeDailyPayroll[5].Date);
            Assert.AreEqual(employeeDailyPayroll6.EmployeeId, employeeDailyPayroll[5].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll6.TotalEmployeeHoursId, employeeDailyPayroll[5].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll6.TotalPay, employeeDailyPayroll[5].TotalPay);
        }

        //Test NightDif Rate 
        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeNightDif()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            TotalEmployeeHoursRepository _totalEmployeeHoursRepository =
                new TotalEmployeeHoursRepository(databaseFactory);
            EmployeeHoursRepository _employeeHoursRepository =
                new EmployeeHoursRepository(databaseFactory);
            EmployeeWorkScheduleRepository _employeeWorkScheduleRepository =
                new EmployeeWorkScheduleRepository(databaseFactory);
            HolidayRepository _holidayRepository =
                new HolidayRepository(databaseFactory);
            SettingRepository _settingsRepository =
                new SettingRepository(databaseFactory);
            EmployeeDepartmentRepository _employeeDepartmentRepository =
                new EmployeeDepartmentRepository(databaseFactory);
            EmployeeRepository _employeeRepository =
                new EmployeeRepository(databaseFactory, _employeeDepartmentRepository);
            AttendanceLogRepository _attendanceLogRepository =
                new AttendanceLogRepository(databaseFactory, _employeeRepository);
            AttendanceRepository _attendanceRepository =
                new AttendanceRepository(databaseFactory);
            EmployeeInfoRepository _employeeInfoRepository =
                new EmployeeInfoRepository(databaseFactory);
            EmployeeDailyPayrollRepository _employeeDailyPayrollRepository =
                new EmployeeDailyPayrollRepository(databaseFactory);
            //EmployeeSalaryRepository _employeeSalaryRepository =
            //    new EmployeeSalaryRepository(databaseFactory);

            EmployeeWorkScheduleService _employeeWorkScheduleService =
                new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            SettingService _settingService =
                new SettingService(_settingsRepository);
            HolidayService _holidayService =
                new HolidayService(_holidayRepository, _settingsRepository, unitOfWork);
            AttendanceLogService _attendanceLogService =
                new AttendanceLogService(_attendanceLogRepository);
            AttendanceService _attendanceService =
                new AttendanceService(unitOfWork, _attendanceRepository, _attendanceLogService);
            EmployeeInfoService _employeeInfoService =
                new EmployeeInfoService(_employeeInfoRepository);
            EmployeeHoursService _employeeHoursService =
                new EmployeeHoursService(unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            TotalEmployeeHoursService _totalEmployeeHoursService =
                new TotalEmployeeHoursService(unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService);
            EmployeeSalaryService _employeeSalaryService = new EmployeeSalaryService();
            EmployeeDailyPayrollService employeeDailyPayrollService =
                new EmployeeDailyPayrollService(unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);

            DeleteInfo(_employeeRepository, unitOfWork);

            //Data
            var employeeId1 = 1;
            var employeeId2 = 2;

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
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("04/22/1989"),
                Gender = 1,
                IsActive = true
            };

            //var EmployeeSalary1 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 100,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Hourly
            //};

            //var EmployeeSalary2 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 3000,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Weekly
            //};

            //_employeeSalaryRepository.Add(EmployeeSalary1);
            //_employeeSalaryRepository.Add(EmployeeSalary2);

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
                EmployeeId = employeeId1
            };

            var workSchedule2 = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule2,
                EmployeeId = employeeId2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                Salary = 100,
                SalaryFrequency = FrequencyType.Hourly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 3000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo1);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHoursRegular1 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 4,
                Type = Entities.Enums.RateType.NightDifferential,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular5 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 1,
                Type = Entities.Enums.RateType.NightDifferential,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular2 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 1.5,
                Type = Entities.Enums.RateType.NightDifferential,
                Date = DateTime.Parse("04/22/2016")
            };

            var totalEmployeeHoursRegular3 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 0.5,
                Type = Entities.Enums.RateType.NightDifferential,
                Date = DateTime.Parse("04/23/2016")
            };

            var totalEmployeeHoursRegular4 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 4.1,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/24/2016")
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular5);

            unitOfWork.Commit();

            //Test
            DateTime dateFrom = DateTime.Parse("04/21/2016");
            DateTime dateTo = DateTime.Parse("04/23/2016");

            employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            var employeeDailyPayroll = employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(employeeDailyPayroll);
            Assert.AreEqual(employeeDailyPayroll.Count, 4);

            var employeeDailyPayroll1 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 3,
                TotalPay = (decimal)49.15
            };

            var employeeDailyPayroll2 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/22/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = (decimal)151.20
            };

            var employeeDailyPayroll3 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = (decimal)403.2
            };

            var employeeDailyPayroll4 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = (decimal)75.8
            };

            Assert.AreEqual(employeeDailyPayroll1.Date, employeeDailyPayroll[0].Date);
            Assert.AreEqual(employeeDailyPayroll1.EmployeeId, employeeDailyPayroll[0].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll1.TotalEmployeeHoursId, employeeDailyPayroll[0].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll1.TotalPay, employeeDailyPayroll[0].TotalPay);

            Assert.AreEqual(employeeDailyPayroll2.Date, employeeDailyPayroll[1].Date);
            Assert.AreEqual(employeeDailyPayroll2.EmployeeId, employeeDailyPayroll[1].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll2.TotalEmployeeHoursId, employeeDailyPayroll[1].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll2.TotalPay, employeeDailyPayroll[1].TotalPay);

            Assert.AreEqual(employeeDailyPayroll3.Date, employeeDailyPayroll[2].Date);
            Assert.AreEqual(employeeDailyPayroll3.EmployeeId, employeeDailyPayroll[2].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll3.TotalEmployeeHoursId, employeeDailyPayroll[2].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll3.TotalPay, employeeDailyPayroll[2].TotalPay);

            Assert.AreEqual(employeeDailyPayroll4.Date, employeeDailyPayroll[3].Date);
            Assert.AreEqual(employeeDailyPayroll4.EmployeeId, employeeDailyPayroll[3].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll4.TotalEmployeeHoursId, employeeDailyPayroll[3].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll4.TotalPay, employeeDailyPayroll[3].TotalPay);
        }

        //Test NightDif Rate 
        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeHoliday()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            TotalEmployeeHoursRepository _totalEmployeeHoursRepository =
                new TotalEmployeeHoursRepository(databaseFactory);
            EmployeeHoursRepository _employeeHoursRepository =
                new EmployeeHoursRepository(databaseFactory);
            EmployeeWorkScheduleRepository _employeeWorkScheduleRepository =
                new EmployeeWorkScheduleRepository(databaseFactory);
            HolidayRepository _holidayRepository =
                new HolidayRepository(databaseFactory);
            SettingRepository _settingsRepository =
                new SettingRepository(databaseFactory);
            EmployeeDepartmentRepository _employeeDepartmentRepository =
                new EmployeeDepartmentRepository(databaseFactory);
            EmployeeRepository _employeeRepository =
                new EmployeeRepository(databaseFactory, _employeeDepartmentRepository);
            AttendanceLogRepository _attendanceLogRepository =
                new AttendanceLogRepository(databaseFactory, _employeeRepository);
            AttendanceRepository _attendanceRepository =
                new AttendanceRepository(databaseFactory);
            EmployeeInfoRepository _employeeInfoRepository =
                new EmployeeInfoRepository(databaseFactory);
            EmployeeDailyPayrollRepository _employeeDailyPayrollRepository =
                new EmployeeDailyPayrollRepository(databaseFactory);
            //EmployeeSalaryRepository _employeeSalaryRepository =
            //    new EmployeeSalaryRepository(databaseFactory);

            EmployeeWorkScheduleService _employeeWorkScheduleService =
                new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            SettingService _settingService =
                new SettingService(_settingsRepository);
            HolidayService _holidayService =
                new HolidayService(_holidayRepository, _settingsRepository, unitOfWork);
            AttendanceLogService _attendanceLogService =
                new AttendanceLogService(_attendanceLogRepository);
            AttendanceService _attendanceService =
                new AttendanceService(unitOfWork, _attendanceRepository, _attendanceLogService);
            EmployeeInfoService _employeeInfoService =
                new EmployeeInfoService(_employeeInfoRepository);
            EmployeeHoursService _employeeHoursService =
                new EmployeeHoursService(unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            TotalEmployeeHoursService _totalEmployeeHoursService =
                new TotalEmployeeHoursService(unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService);
            EmployeeSalaryService _employeeSalaryService = new EmployeeSalaryService();
            EmployeeDailyPayrollService employeeDailyPayrollService =
                new EmployeeDailyPayrollService(unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);

            DeleteInfo(_employeeRepository, unitOfWork);

            var holiday = new Holiday
            {
                Date = DateTime.Parse("04/21/2016"),
                IsRegularHoliday = true,
                HolidayName = "Test Regular",
                Description = "Test Regular Description"
            };

            var holiday2 = new Holiday
            {
                Date = DateTime.Parse("04/22/2016"),
                IsRegularHoliday = false,
                HolidayName = "Test Special",
                Description = "Test Special Description"
            };

            _holidayRepository.Add(holiday);
            _holidayRepository.Add(holiday2);

            //Data
            var employeeId1 = 1;
            var employeeId2 = 2;

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
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("04/22/1989"),
                Gender = 1,
                IsActive = true
            };

            //var EmployeeSalary1 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 100,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Hourly
            //};

            //var EmployeeSalary2 = new EmployeeSalary
            //{
            //    EmployeeId = employeeId1,
            //    Salary = 3000,
            //    SalaryFrequency = Entities.Enums.FrequencyType.Weekly
            //};

            //_employeeSalaryRepository.Add(EmployeeSalary1);
            //_employeeSalaryRepository.Add(EmployeeSalary2);

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
                EmployeeId = employeeId1
            };

            var workSchedule2 = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule2,
                EmployeeId = employeeId2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                Salary = 100,
                SalaryFrequency = FrequencyType.Hourly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 3000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo1);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHoursRegular1 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 4,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular5 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 1,
                Type = Entities.Enums.RateType.NightDifferential,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular2 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 1.5,
                Type = Entities.Enums.RateType.OverTime,
                Date = DateTime.Parse("04/22/2016")
            };

            var totalEmployeeHoursRegular3 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 0.5,
                Type = Entities.Enums.RateType.OverTime,
                Date = DateTime.Parse("04/23/2016")
            };

            var totalEmployeeHoursRegular4 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 4.1,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/24/2016")
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular5);

            unitOfWork.Commit();

            //Test
            DateTime dateFrom = DateTime.Parse("04/21/2016");
            DateTime dateTo = DateTime.Parse("04/23/2016");

            employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            var employeeDailyPayroll = employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(employeeDailyPayroll);
            Assert.AreEqual(employeeDailyPayroll.Count, 4);

            var employeeDailyPayroll1 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 3,
                TotalPay = (decimal)58.125
            };

            var employeeDailyPayroll2 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/22/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = (decimal)232.5
            };

            var employeeDailyPayroll3 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = (decimal)800
            };

            var employeeDailyPayroll4 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = (decimal)150.8
            };

            Assert.AreEqual(employeeDailyPayroll1.Date, employeeDailyPayroll[0].Date);
            Assert.AreEqual(employeeDailyPayroll1.EmployeeId, employeeDailyPayroll[0].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll1.TotalEmployeeHoursId, employeeDailyPayroll[0].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll1.TotalPay, employeeDailyPayroll[0].TotalPay);

            Assert.AreEqual(employeeDailyPayroll2.Date, employeeDailyPayroll[1].Date);
            Assert.AreEqual(employeeDailyPayroll2.EmployeeId, employeeDailyPayroll[1].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll2.TotalEmployeeHoursId, employeeDailyPayroll[1].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll2.TotalPay, employeeDailyPayroll[1].TotalPay);

            Assert.AreEqual(employeeDailyPayroll3.Date, employeeDailyPayroll[2].Date);
            Assert.AreEqual(employeeDailyPayroll3.EmployeeId, employeeDailyPayroll[2].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll3.TotalEmployeeHoursId, employeeDailyPayroll[2].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll3.TotalPay, employeeDailyPayroll[2].TotalPay);

            Assert.AreEqual(employeeDailyPayroll4.Date, employeeDailyPayroll[3].Date);
            Assert.AreEqual(employeeDailyPayroll4.EmployeeId, employeeDailyPayroll[3].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll4.TotalEmployeeHoursId, employeeDailyPayroll[3].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll4.TotalPay, employeeDailyPayroll[3].TotalPay);
        }
    }
}
