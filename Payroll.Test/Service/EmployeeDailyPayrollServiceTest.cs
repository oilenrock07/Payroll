using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeDailyPayrollServiceTest
    {
        private UnitOfWork _unitOfWork;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private IHolidayService _holidayService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeSalaryService _employeeSalaryService;

        private IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;
        private ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        private IHolidayRepository _holidayRepository;
        private IEmployeeInfoRepository _employeeInfoRepository;
        private IEmployeeHoursRepository _employeeHoursRepository;
        private ISettingRepository _settingRepository;

        private IEmployeeDailyPayrollService _employeeDailyPayrollService;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(databaseFactory);

            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            _employeeDailyPayrollRepository = new EmployeeDailyPayrollRepository(databaseFactory);
            _totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(databaseFactory);
            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            _holidayRepository = new HolidayRepository(databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);
            _employeeHoursRepository = new EmployeeHoursRepository(databaseFactory);
            _settingRepository = new SettingRepository(databaseFactory);

            _totalEmployeeHoursService = new TotalEmployeeHoursService(_unitOfWork, _totalEmployeeHoursRepository, null, _settingService);
            
            _employeeWorkScheduleService = new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            _holidayService = new HolidayService(_holidayRepository, _settingRepository, _unitOfWork);
            _settingService = new SettingService(_settingRepository);
            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _employeeSalaryService = new EmployeeSalaryService();

            _employeeDailyPayrollService = new EmployeeDailyPayrollService(_unitOfWork, _totalEmployeeHoursService, 
            _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);
        }

        public void DeleteData()
        {
            _employeeInfoRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE work_schedule");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_daily_payroll");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            _employeeInfoRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeRegular()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 1,
                Hours = 8,
                TotalEmployeeHoursId = 1,
                Type = RateType.Regular
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 2,
                Hours = 7.5,
                TotalEmployeeHoursId = 2,
                Type = RateType.Regular
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 1,
                Hours = 8.1,
                TotalEmployeeHoursId = 2,
                Type = RateType.Regular
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 2,
                Type = RateType.Regular
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("05/02/2016");
            var dateTo = DateTime.Parse("05/03/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(3, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1012.50, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1000, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[1].Date);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(4, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)800, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[2].Date);
 
            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(2, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1500, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[3].Date);
        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeOT()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 1,
                Hours = 8,
                TotalEmployeeHoursId = 1,
                Type = RateType.OverTime
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 2,
                Hours = 7.5,
                TotalEmployeeHoursId = 2,
                Type = RateType.OverTime
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 1,
                Hours = 8.1,
                TotalEmployeeHoursId = 2,
                Type = RateType.OverTime
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 2,
                Type = RateType.OverTime
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("05/02/2016");
            var dateTo = DateTime.Parse("05/03/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(3, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1265.625, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1250, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[1].Date);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(4, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1000, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[2].Date);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(2, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1875, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[3].Date);
        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeNightDiff()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 1,
                Hours = 8,
                TotalEmployeeHoursId = 1,
                Type = RateType.NightDifferential
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/02/2016"),
                EmployeeId = 2,
                Hours = 7.5,
                TotalEmployeeHoursId = 2,
                Type = RateType.NightDifferential
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 1,
                Hours = 8.1,
                TotalEmployeeHoursId = 2,
                Type = RateType.NightDifferential
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/03/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 2,
                Type = RateType.NightDifferential
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("05/02/2016");
            var dateTo = DateTime.Parse("05/03/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(3, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)6.48, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)6.4, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[1].Date);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(4, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)3.2, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/03/2016"), results[2].Date);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(2, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)6, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/02/2016"), results[3].Date);
        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeRestDay()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/07/2016"),
                EmployeeId = 1,
                Hours = 8,
                TotalEmployeeHoursId = 1,
                Type = RateType.Regular
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/07/2016"),
                EmployeeId = 2,
                Hours = 7.5,
                TotalEmployeeHoursId = 2,
                Type = RateType.Regular
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/07/2016"),
                EmployeeId = 1,
                Hours = 4.1,
                TotalEmployeeHoursId = 3,
                Type = RateType.OverTime
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/07/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 4,
                Type = RateType.NightDifferential
            };

            var totalEmployeeHours5 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/08/2016"),
                EmployeeId = 1,
                Hours = 8.1,
                TotalEmployeeHoursId = 5,
                Type = RateType.Regular
            };

            var totalEmployeeHours6 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("05/08/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 6,
                Type = RateType.OverTime
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours5);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours6);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("05/06/2016");
            var dateTo = DateTime.Parse("05/08/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(5, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1316.25, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/08/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1300, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/07/2016"), results[1].Date);

            Assert.AreEqual(1, results[2].EmployeeId);
            Assert.AreEqual(3, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)832.8125, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/07/2016"), results[2].Date);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(6, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1300, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/08/2016"), results[3].Date);

            Assert.AreEqual(2, results[4].EmployeeId);
            Assert.AreEqual(2, results[4].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1950, results[4].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/07/2016"), results[4].Date);

            Assert.AreEqual(2, results[5].EmployeeId);
            Assert.AreEqual(4, results[5].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)4.16, results[5].TotalPay);
            Assert.AreEqual(DateTime.Parse("05/07/2016"), results[5].Date);
        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeHoliday()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/01/2016"),
                EmployeeId = 1,
                Hours = 8,
                TotalEmployeeHoursId = 1,
                Type = RateType.Regular
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/01/2016"),
                EmployeeId = 1,
                Hours = 4.5,
                TotalEmployeeHoursId = 2,
                Type = RateType.OverTime
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/01/2016"),
                EmployeeId = 1,
                Hours = 2.5,
                TotalEmployeeHoursId = 3,
                Type = RateType.NightDifferential
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/02/2016"),
                EmployeeId = 2,
                Hours = 4,
                TotalEmployeeHoursId = 4,
                Type = RateType.Regular
            };

            var totalEmployeeHours5 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/02/2016"),
                EmployeeId = 2,
                Hours = 3.25,
                TotalEmployeeHoursId = 5,
                Type = RateType.OverTime
            };

            var totalEmployeeHours6 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/02/2016"),
                EmployeeId = 2,
                Hours = 0.25,
                TotalEmployeeHoursId = 6,
                Type = RateType.NightDifferential
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours5);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours6);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("01/01/2016");
            var dateTo = DateTime.Parse("01/02/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(7, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)2000, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(2, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1462.5, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[1].Date);

            Assert.AreEqual(1, results[2].EmployeeId);
            Assert.AreEqual(3, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)4, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[2].Date);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(4, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1200, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/02/2016"), results[3].Date);

            Assert.AreEqual(2, results[4].EmployeeId);
            Assert.AreEqual(5, results[4].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1267.5, results[4].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/02/2016"), results[4].Date);

            Assert.AreEqual(2, results[5].EmployeeId);
            Assert.AreEqual(6, results[5].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)0.3, results[5].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/02/2016"), results[5].Date);

            Assert.AreEqual(2, results[6].EmployeeId);
            Assert.AreEqual(null, results[6].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1600, results[6].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[6].Date);


        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeHolidayNoWork()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("01/01/2015");
            var dateTo = DateTime.Parse("01/02/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(null, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1000, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[0].Date);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(null, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1600, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[1].Date);

        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeHolidayNoWork2()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 6
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 1
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("01/01/2015");
            var dateTo = DateTime.Parse("01/02/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(null, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1000, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[0].Date);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(null, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1600, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[1].Date);

        }

        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRangeHolidayPartialWork()
        {
            //Arrange 
            Initialize();
            DeleteData();

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

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 5000,
                SalaryFrequency = FrequencyType.Weekly
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 8000,
                SalaryFrequency = FrequencyType.Weekly
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

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

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                EmployeeId = 2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            //Total EmployeeHours
            var totalEmployeeHours1 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/01/2016"),
                EmployeeId = 1,
                Hours = 4,
                TotalEmployeeHoursId = 1,
                Type = RateType.Regular
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                Date = DateTime.Parse("01/02/2016"),
                EmployeeId = 2,
                Hours = 2,
                TotalEmployeeHoursId = 4,
                Type = RateType.Regular
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);

            _unitOfWork.Commit();

            //Test
            var dateFrom = DateTime.Parse("01/01/2016");
            var dateTo = DateTime.Parse("01/02/2016");

            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            //Results Verification
            var results = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1000, results[0].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[0].Date);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(null, results[1].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)500, results[1].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[1].Date);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(null, results[3].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)1600, results[3].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/01/2016"), results[3].Date);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(2, results[2].TotalEmployeeHoursId);
            Assert.AreEqual((decimal)600, results[2].TotalPay);
            Assert.AreEqual(DateTime.Parse("01/02/2016"), results[2].Date);
        }
    }
}
