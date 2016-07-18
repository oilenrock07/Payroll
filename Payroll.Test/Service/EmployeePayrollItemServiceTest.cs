using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
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
    public class EmployeePayrollItemServiceTest
    {
        private IUnitOfWork _unitOfWork;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private IHolidayService _holidayService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeSalaryService _employeeSalaryService;

        private IEmployeePayrollItemRepository _employeePayrollItemRepository;
        private ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        private IHolidayRepository _holidayRepository;
        private IEmployeeInfoRepository _employeeInfoRepository;
        private IEmployeeHoursRepository _employeeHoursRepository;
        private ISettingRepository _settingRepository;
        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;

        private IEmployeePayrollItemService _employeePayrollItemService;

        private const String RATE_REST_DAY = "RATE_REST_DAY";
        private const String RATE_OT = "RATE_OT";
        private const String RATE_NIGHTDIF = "RATE_NIGHTDIF";
        private const String RATE_HOLIDAY_SPECIAL = "RATE_HOLIDAY_SPECIAL";
        private const String RATE_HOLIDAY_REGULAR = "RATE_HOLIDAY_REGULAR";
        private const String RATE_OT_HOLIDAY = "RATE_OT_HOLIDAY";
        private const String PAYROLL_REGULAR_HOURS = "PAYROLL_REGULAR_HOURS";
        private const String PAYROLL_IS_SPHOLIDAY_WITH_PAY = "PAYROLL_IS_SPHOLIDAY_WITH_PAY";
        private const String RATE_HOLIDAY_SPECIAL_REST_DAY = "RATE_HOLIDAY_SPECIAL_REST_DAY";

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(databaseFactory);

            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            _employeePayrollItemRepository = new EmployeePayrollItemRepository(databaseFactory);
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
            _employeePayrollRepository = new EmployeePayrollRepository(databaseFactory);
            _employeePayrollDeductionRepository = new EmployeePayrollDeductionRepository(databaseFactory);

            _employeePayrollItemService = new EmployeePayrollItemService(_unitOfWork, _employeePayrollItemRepository, 
                    _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeInfoService, _employeeSalaryService, _employeePayrollRepository, _employeePayrollDeductionRepository, null, null);
        }

        public void DeleteData()
        {
            _employeeInfoRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE work_schedule");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_payroll_item");
            _employeeInfoRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            _employeeInfoRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeRegular()
        {
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
            var payrollStart = DateTime.Parse("05/02/2016");
            var payrollEnd = DateTime.Parse("05/03/2016");
            var payrollDate = DateTime.Parse("05/04/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEnd);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.Regular, results[0].RateType);
            Assert.AreEqual(16.1, results[0].TotalHours);
            Assert.AreEqual((decimal)2012.50, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].Multiplier);
            Assert.AreEqual(200, results[1].RatePerHour);
            Assert.AreEqual(RateType.Regular, results[1].RateType);
            Assert.AreEqual(11.5, results[1].TotalHours);
            Assert.AreEqual((decimal)2300, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeOT()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("05/02/2016");
            var payrollEndDate = DateTime.Parse("05/03/2016");
            var payrollDate = DateTime.Parse("05/04/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(OTRate, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.OverTime, results[0].RateType);
            Assert.AreEqual(16.1, results[0].TotalHours);
            Assert.AreEqual((decimal)2515.625, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(OTRate, results[1].Multiplier);
            Assert.AreEqual(200, results[1].RatePerHour);
            Assert.AreEqual(RateType.OverTime, results[1].RateType);
            Assert.AreEqual(11.5, results[1].TotalHours);
            Assert.AreEqual((decimal)2875, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeNightDiff()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("05/02/2016");
            var payrollEndDate = DateTime.Parse("05/03/2016");
            var payrollDate = DateTime.Parse("05/04/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            Decimal nightDifRate = Decimal.Parse(_settingService.GetByKey(RATE_NIGHTDIF));

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].Multiplier);
            Assert.AreEqual(nightDifRate, results[0].RatePerHour);
            Assert.AreEqual(RateType.NightDifferential, results[0].RateType);
            Assert.AreEqual(16.1, results[0].TotalHours);
            Assert.AreEqual((decimal)12.88, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].Multiplier);
            Assert.AreEqual(nightDifRate, results[1].RatePerHour);
            Assert.AreEqual(RateType.NightDifferential, results[1].RateType);
            Assert.AreEqual(11.5, results[1].TotalHours);
            Assert.AreEqual((decimal)9.2, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeRestDay()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("05/06/2016");
            var payrollEndDate = DateTime.Parse("05/08/2016");
            var payrollDate = DateTime.Parse("05/09/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            Decimal nightDifRate = Decimal.Parse(_settingService.GetByKey(RATE_NIGHTDIF));
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));
            Double restDayRate = Double.Parse(_settingService.GetByKey(RATE_REST_DAY));

            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(restDayRate, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.RestDay, results[0].RateType);
            Assert.AreEqual(16.1, results[0].TotalHours);
            Assert.AreEqual((decimal)2616.25, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(restDayRate * OTRate, results[1].Multiplier);
            Assert.AreEqual(125, results[1].RatePerHour);
            Assert.AreEqual(RateType.RestDayOT, results[1].RateType);
            Assert.AreEqual(4.1, results[1].TotalHours);
            Assert.AreEqual((decimal)832.8125, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(restDayRate, results[2].Multiplier);
            Assert.AreEqual(200, results[2].RatePerHour);
            Assert.AreEqual(RateType.RestDay, results[2].RateType);
            Assert.AreEqual(7.5, results[2].TotalHours);
            Assert.AreEqual((decimal)1950, results[2].TotalAmount);
            Assert.AreEqual(payrollDate, results[2].PayrollDate);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(1, results[3].Multiplier);
            Assert.AreEqual(nightDifRate, results[3].RatePerHour);
            Assert.AreEqual(RateType.NightDifferential, results[3].RateType);
            Assert.AreEqual(4, results[3].TotalHours);
            Assert.AreEqual((decimal)3.20, results[3].TotalAmount);
            Assert.AreEqual(payrollDate, results[3].PayrollDate);

            Assert.AreEqual(2, results[4].EmployeeId);
            Assert.AreEqual(restDayRate * OTRate, results[4].Multiplier);
            Assert.AreEqual(200, results[4].RatePerHour);
            Assert.AreEqual(RateType.RestDayOT, results[4].RateType);
            Assert.AreEqual(4, results[4].TotalHours);
            Assert.AreEqual((decimal)1300, results[4].TotalAmount);
            Assert.AreEqual(payrollDate, results[4].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeHoliday()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("01/01/2016");
            var payrollEndDate = DateTime.Parse("01/02/2016");
            var payrollDate = DateTime.Parse("01/03/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            Decimal nightDifRate = Decimal.Parse(_settingService.GetByKey(RATE_NIGHTDIF));
            Double OTRate = Double.Parse(_settingService.GetByKey(RATE_OT));
            Double OTRateHoliday = Double.Parse(_settingService.GetByKey(RATE_OT_HOLIDAY));
            Double restDayRate = Double.Parse(_settingService.GetByKey(RATE_REST_DAY));
            Double regularHolidayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_REGULAR));
            Double specialHolidayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL));
            Double specialHolidayRestDayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL_REST_DAY));

            Assert.IsNotNull(results);
            Assert.AreEqual(7, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(regularHolidayRate, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.RegularHoliday, results[0].RateType);
            Assert.AreEqual(8, results[0].TotalHours);
            Assert.AreEqual((decimal)2000, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);
    
            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(regularHolidayRate * OTRateHoliday, results[1].Multiplier);
            Assert.AreEqual(125, results[1].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayOT, results[1].RateType);
            Assert.AreEqual(4.5, results[1].TotalHours);
            Assert.AreEqual((decimal)1462.50, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);

            Assert.AreEqual(1, results[2].EmployeeId);
            Assert.AreEqual(1, results[2].Multiplier);
            Assert.AreEqual(nightDifRate, results[2].RatePerHour);
            Assert.AreEqual(RateType.NightDifferential, results[2].RateType);
            Assert.AreEqual(2.5, results[2].TotalHours);
            Assert.AreEqual((decimal)2, results[2].TotalAmount);
            Assert.AreEqual(payrollDate, results[2].PayrollDate);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(specialHolidayRestDayRate, results[3].Multiplier);
            Assert.AreEqual(200, results[3].RatePerHour);
            Assert.AreEqual(RateType.SpecialHolidayRestDay, results[3].RateType);
            Assert.AreEqual(4, results[3].TotalHours);
            Assert.AreEqual((decimal)1200, results[3].TotalAmount);
            Assert.AreEqual(payrollDate, results[3].PayrollDate);

            Assert.AreEqual(2, results[4].EmployeeId);
            Assert.AreEqual(specialHolidayRestDayRate * OTRateHoliday, results[4].Multiplier);
            Assert.AreEqual(200, results[4].RatePerHour);
            Assert.AreEqual(RateType.SpecialHolidayRestDayOT, results[4].RateType);
            Assert.AreEqual(3.25, results[4].TotalHours);
            Assert.AreEqual((decimal)1267.50, results[4].TotalAmount);
            Assert.AreEqual(payrollDate, results[4].PayrollDate);

            Assert.AreEqual(2, results[5].EmployeeId);
            Assert.AreEqual(1, results[5].Multiplier);
            Assert.AreEqual(nightDifRate, results[5].RatePerHour);
            Assert.AreEqual(RateType.NightDifferential, results[5].RateType);
            Assert.AreEqual(0.25, results[5].TotalHours);
            Assert.AreEqual((decimal)0.2, results[5].TotalAmount);
            Assert.AreEqual(payrollDate, results[5].PayrollDate);

            Assert.AreEqual(2, results[6].EmployeeId);
            Assert.AreEqual(1, results[6].Multiplier);
            Assert.AreEqual(200, results[6].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[6].RateType);
            Assert.AreEqual(8, results[6].TotalHours);
            Assert.AreEqual((decimal)1600, results[6].TotalAmount);
            Assert.AreEqual(payrollDate, results[6].PayrollDate);

        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeHolidayNoWork()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("01/01/2016");
            var payrollEndDate = DateTime.Parse("01/02/2016");
            var payrollDate = DateTime.Parse("01/03/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[0].RateType);
            Assert.AreEqual(8, results[0].TotalHours);
            Assert.AreEqual((decimal)1000, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].Multiplier);
            Assert.AreEqual(200, results[1].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[1].RateType);
            Assert.AreEqual(8, results[1].TotalHours);
            Assert.AreEqual((decimal)1600, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeHolidayNoWorkNoPay()
        { //Arrange 
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
            var payrollStart = DateTime.Parse("01/01/2016");
            var payrollEndDate = DateTime.Parse("01/02/2016");
            var payrollDate = DateTime.Parse("01/03/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[0].RateType);
            Assert.AreEqual(8, results[0].TotalHours);
            Assert.AreEqual((decimal)1000, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(2, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].Multiplier);
            Assert.AreEqual(200, results[1].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[1].RateType);
            Assert.AreEqual(8, results[1].TotalHours);
            Assert.AreEqual((decimal)1600, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);
        }

        [TestMethod]
        public void GenerateEmployeePayrollItemByDateRangeHolidayPartialWork()
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
            var payrollStart = DateTime.Parse("01/01/2016");
            var payrollEndDate = DateTime.Parse("01/02/2016");
            var payrollDate = DateTime.Parse("01/03/2016");

            _employeePayrollItemService
                .GenerateEmployeePayrollItemByDateRange(payrollDate, payrollStart, payrollEndDate);

            var results = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            Double regularHolidayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_REGULAR));
            Double specialHolidayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL));
            Double specialHolidayRestDayRate = Double.Parse(_settingService.GetByKey(RATE_HOLIDAY_SPECIAL_REST_DAY));

            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(regularHolidayRate, results[0].Multiplier);
            Assert.AreEqual(125, results[0].RatePerHour);
            Assert.AreEqual(RateType.RegularHoliday, results[0].RateType);
            Assert.AreEqual(4, results[0].TotalHours);
            Assert.AreEqual((decimal)1000, results[0].TotalAmount);
            Assert.AreEqual(payrollDate, results[0].PayrollDate);

            Assert.AreEqual(1, results[1].EmployeeId);
            Assert.AreEqual(1, results[1].Multiplier);
            Assert.AreEqual(125, results[1].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[1].RateType);
            Assert.AreEqual(4, results[1].TotalHours);
            Assert.AreEqual((decimal)500, results[1].TotalAmount);
            Assert.AreEqual(payrollDate, results[1].PayrollDate);

            Assert.AreEqual(2, results[2].EmployeeId);
            Assert.AreEqual(specialHolidayRestDayRate, results[2].Multiplier);
            Assert.AreEqual(200, results[2].RatePerHour);
            Assert.AreEqual(RateType.SpecialHolidayRestDay, results[2].RateType);
            Assert.AreEqual(2, results[2].TotalHours);
            Assert.AreEqual((decimal)600, results[2].TotalAmount);
            Assert.AreEqual(payrollDate, results[2].PayrollDate);

            Assert.AreEqual(2, results[3].EmployeeId);
            Assert.AreEqual(1, results[3].Multiplier);
            Assert.AreEqual(200, results[3].RatePerHour);
            Assert.AreEqual(RateType.RegularHolidayNotWorked, results[3].RateType);
            Assert.AreEqual(8, results[3].TotalHours);
            Assert.AreEqual((decimal)1600, results[3].TotalAmount);
            Assert.AreEqual(payrollDate, results[3].PayrollDate);

        }
    }
}
