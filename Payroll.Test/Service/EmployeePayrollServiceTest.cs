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
    public class EmployeePayrollServiceTest
    {
        private IUnitOfWork _unitOfWork;

        private IEmployeeDailyPayrollService _employeeDailyPayrollService;
        private IEmployeePayrollDeductionService _employeePayrollDeductionService;
        private ISettingService _settingService;
        private IEmployeePayrollService _employeePayrollService;
        private IEmployeeInfoService _employeeInfoService;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IDeductionService _deductionService;
        private IEmployeeDeductionRepository _employeeDeductionRepository;
        private ITaxService _taxService;
        private IEmployeeService _employeeService;
        private IEmployeePayrollItemService _employeePayrollItemService;
        private IEmployeeAdjustmentService _employeeAdjusmentService;

        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;
        private IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;
        private ISettingRepository _settingRepository;
        private IEmployeeInfoRepository _employeeInfoRepository;
        private ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private IDeductionRepository _deductionRepository;
        private IEmployeeDeductionService _employeeDeductionService;
        private ITaxRepository _taxRepository;
        private IEmployeeRepository _employeeRepository;
        private IEmployeePayrollItemRepository _employeePayrollItemRepository;
        private IEmployeeAdjustmentRepository _employeeAdjustmentRepository;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(databaseFactory);

            _employeeDailyPayrollRepository = new EmployeeDailyPayrollRepository(databaseFactory);
            _employeePayrollRepository = new EmployeePayrollRepository(databaseFactory);
            _settingRepository = new SettingRepository(databaseFactory);
            _employeePayrollDeductionRepository = new EmployeePayrollDeductionRepository(databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);
            _totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(databaseFactory);
            _deductionRepository = new DeductionRepository(databaseFactory);
            _employeeDeductionRepository = new EmployeeDeductionRepository(databaseFactory);
            _taxRepository = new TaxRepository(databaseFactory);
            _employeeRepository = new EmployeeRepository(databaseFactory, null);
            _employeePayrollItemRepository = new EmployeePayrollItemRepository(databaseFactory);
            _employeeAdjustmentRepository = new EmployeeAdjustmentRepository(databaseFactory);

            _settingService = new SettingService(_settingRepository);
            _employeeDailyPayrollService = new EmployeeDailyPayrollService(_unitOfWork, 
                null, null, null, null, _employeeDailyPayrollRepository, null, null);
            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _deductionService = new DeductionService(_deductionRepository);
            _employeeDeductionService = new EmployeeDeductionService(_employeeDeductionRepository);
            _taxService = new TaxService(_taxRepository);
            _employeePayrollDeductionService = new EmployeePayrollDeductionService(_unitOfWork, _settingService, null, _employeeInfoService, _employeeDeductionService, _deductionService, _employeePayrollDeductionRepository, _taxService);
            _employeeService = new EmployeeService(_employeeRepository);
            _totalEmployeeHoursService = new TotalEmployeeHoursService(_unitOfWork, _totalEmployeeHoursRepository, null, _settingService);
            _employeePayrollItemService = new EmployeePayrollItemService(_unitOfWork, _employeePayrollItemRepository, null, null, null, null,null, null);
            _employeeAdjusmentService = new EmployeeAdjustmentService(_employeeAdjustmentRepository, _employeeRepository);
            _employeePayrollService = new EmployeePayrollService(_unitOfWork, _employeePayrollRepository, _settingService, _employeePayrollDeductionService, _employeeInfoService, _totalEmployeeHoursService, _employeeService, _totalEmployeeHoursService, _employeePayrollItemService, _employeeAdjusmentService);

            //Update settings
            var settingsPayrollStartDate = _settingRepository.GetSettingByKey("PAYROLL_WEEK_START");

            _settingRepository.Update(settingsPayrollStartDate);
            settingsPayrollStartDate.Value = "3";

            var settingsPayrollEndDate = _settingRepository.GetSettingByKey("PAYROLL_WEEK_END");

            _settingRepository.Update(settingsPayrollEndDate);
            settingsPayrollEndDate.Value = "2";

            var settingsPayrollReleaseDate = _settingRepository.GetSettingByKey("PAYROLL_WEEK_RELEASE");

            _settingRepository.Update(settingsPayrollReleaseDate);
            settingsPayrollReleaseDate.Value = "3";

            _unitOfWork.Commit();

        }

        private void DeleteInfo()
        {
            _employeePayrollDeductionRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_payroll_item");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE payroll");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        [TestMethod]
        public void GetPayrollNextDate()
        {
            Initialize();
            DeleteInfo();

            DateTime date = DateTime.Parse("04/27/2016");

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(date);

            Assert.AreEqual(DateTime.Parse("04/20/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("04/26/2016"), payrollEndDate);

            DateTime payrollReleaseDate = _employeePayrollService
                .GetNextPayrollReleaseDate(payrollEndDate);

            Assert.AreEqual(DateTime.Parse("04/27/2016"), payrollReleaseDate);
        }

        [TestMethod]
        public void GetPayrollNextStartDate2()
        {
            Initialize();
            DeleteInfo();

            DateTime date = DateTime.Parse("05/17/2016");

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(date);

            Assert.AreEqual(DateTime.Parse("05/11/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
              .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("05/17/2016"), payrollEndDate);

            DateTime payrollReleaseDate = _employeePayrollService
              .GetNextPayrollReleaseDate(payrollEndDate);

            Assert.AreEqual(DateTime.Parse("05/18/2016"), payrollReleaseDate);
        }

        [TestMethod]
        public void GetPayrollNextStartDate3()
        {
            Initialize();
            DeleteInfo();

            //Test Data
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

            var employeePayroll = new EmployeePayroll()
            {
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                PayrollGeneratedDate = DateTime.Parse("05/18/2016"),
                Employee = employee
            };

            var employeePayroll2 = new EmployeePayroll()
            {
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                PayrollGeneratedDate = DateTime.Parse("05/11/2016"),
                Employee = employee
            };

            _employeePayrollRepository.Add(employeePayroll);
            _employeePayrollRepository.Add(employeePayroll2);

            _unitOfWork.Commit();

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate();

            Assert.AreEqual(DateTime.Parse("05/18/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("05/24/2016"), payrollEndDate);

            DateTime payrollReleaseDate = _employeePayrollService
                .GetNextPayrollReleaseDate(payrollEndDate);

            Assert.AreEqual(DateTime.Parse("05/25/2016"), payrollReleaseDate);
        }

        [TestMethod]
        public void GeneratePayrollNetPayByDateRange(){
            Initialize();
            DeleteInfo();

            //Test Data
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
                FirstName = "Cornelio Jr.",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 120,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 350
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 150,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 400
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

            var payrollDate = DateTime.Parse("05/21/2016");

            var employeePayrollItem = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 220.55M,
                TotalHours = 8,
                PayrollDate = payrollDate,
                RateType  = RateType.Regular,
                Multiplier = 1,
                RatePerHour = 100
            };

            var employeePayrollItem2 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalHours = 10,
                TotalAmount = 100,
                PayrollDate = payrollDate,
                RateType = RateType.OverTime,
                Multiplier = 1,
                RatePerHour = 200
            };

            var employeePayrollItem3 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalHours = 10,
                TotalAmount = 15,
                PayrollDate = payrollDate,
                RateType = RateType.OverTime,
                Multiplier = 1,
                RatePerHour = 12.23M
            };

            var employeePayrollItem4 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalHours = 15.2,
                TotalAmount = 300.10M,
                PayrollDate = payrollDate,
                RateType = RateType.NightDifferential,
                Multiplier = 1,
                RatePerHour = 122.2M
            };

            var employeePayrollItem5 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalHours = 1.25,
                TotalAmount = 50,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHoliday,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem6 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 150.12M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayNotWorked,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem7 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 40,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayOT,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem8 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 540.02M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            //Not included
            var employeePayrollItem9 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 540.02M,
                PayrollDate = DateTime.Parse("06/20/2016"),
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            _employeePayrollItemRepository.Add(employeePayrollItem);
            _employeePayrollItemRepository.Add(employeePayrollItem2);
            _employeePayrollItemRepository.Add(employeePayrollItem3);
            _employeePayrollItemRepository.Add(employeePayrollItem4);
            _employeePayrollItemRepository.Add(employeePayrollItem5);
            _employeePayrollItemRepository.Add(employeePayrollItem6);
            _employeePayrollItemRepository.Add(employeePayrollItem7);
            _employeePayrollItemRepository.Add(employeePayrollItem8);
            _employeePayrollItemRepository.Add(employeePayrollItem9);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/19/2016");
            var dateEnd = DateTime.Parse("05/20/2016");
            
            var payrollList = _employeePayrollService.GeneratePayrollGrossPayByDateRange(payrollDate, dateStart, dateEnd);

            Assert.IsNotNull(payrollList);
            Assert.AreEqual(2, payrollList.Count());

            Assert.AreEqual(1, payrollList[0].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[0].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[0].CutOffEndDate);
            Assert.AreEqual(false, payrollList[0].IsTaxed);
            Assert.AreEqual(0, payrollList[0].TotalAdjustment);
            Assert.AreEqual(0, payrollList[0].TotalDeduction);
            Assert.AreEqual(525.67M, payrollList[0].TotalGross);
            Assert.AreEqual(525.67M, payrollList[0].TotalNet);
            Assert.AreEqual(525.67M, payrollList[0].TaxableIncome);

            Assert.AreEqual(2, payrollList[1].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[1].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[1].CutOffEndDate);
            Assert.AreEqual(false, payrollList[1].IsTaxed);
            Assert.AreEqual(0, payrollList[1].TotalAdjustment);
            Assert.AreEqual(0, payrollList[1].TotalDeduction);
            Assert.AreEqual(890.12M, payrollList[1].TotalGross);
            Assert.AreEqual(890.12M, payrollList[1].TotalNet);
            Assert.AreEqual(890.12M, payrollList[1].TaxableIncome);

        }


        [TestMethod]
        public void ComputeAllowance()
        {
            Initialize();
            DeleteInfo();

            //Test Data
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
                FirstName = "Cornelio Jr.",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 120,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 350
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 150,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 400
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHours = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/06/2016"),
                Hours = 8,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/06/2016"),
                Hours = 3,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/06/2016"),
                Hours = 5,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/06/2016"),
                Hours = 2,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours5 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/07/2016"),
                Hours = 10,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours6 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/07/2016"),
                Hours = 3,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours7 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/07/2016"),
                Hours = 1,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours8 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/07/2016"),
                Hours = 2,
                Type = RateType.NightDifferential,
                TotalEmployeeHoursId = 2
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours5);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours6);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours7);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours8);

            var payrollDate = DateTime.Parse("05/11/2016");

            var employeePayrollItem = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 220.55M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem2 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 100,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem3 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 15,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem4 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 300.10M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem5 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 50,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem6 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 150.12M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem7 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 40,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem8 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 540.02M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            _employeePayrollItemRepository.Add(employeePayrollItem);
            _employeePayrollItemRepository.Add(employeePayrollItem2);
            _employeePayrollItemRepository.Add(employeePayrollItem3);
            _employeePayrollItemRepository.Add(employeePayrollItem4);
            _employeePayrollItemRepository.Add(employeePayrollItem5);
            _employeePayrollItemRepository.Add(employeePayrollItem6);
            _employeePayrollItemRepository.Add(employeePayrollItem7);
            _employeePayrollItemRepository.Add(employeePayrollItem8);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/04/2016");
            var dateEnd = DateTime.Parse("05/10/2016");
            
            _employeePayrollService.GeneratePayroll(dateStart, dateEnd);
            _unitOfWork.Commit();

            var payrollList = _employeePayrollService.GetByDateRange(payrollDate, payrollDate);

            Assert.IsNotNull(payrollList);
            Assert.AreEqual(2, payrollList.Count());

            Assert.AreEqual(1, payrollList[0].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[0].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[0].CutOffEndDate);
            Assert.AreEqual(false, payrollList[0].IsTaxed);
            Assert.AreEqual(0, payrollList[0].TotalAdjustment);
            Assert.AreEqual(0, payrollList[0].TotalDeduction);
            Assert.AreEqual(552.59M, decimal.Round(payrollList[0].TotalGross, 2));
            Assert.AreEqual(552.59M, decimal.Round(payrollList[0].TotalNet, 2));
            Assert.AreEqual(552.59M, decimal.Round(payrollList[0].TaxableIncome, 2));
            Assert.AreEqual(26.92M, decimal.Round(payrollList[0].TotalAllowance, 2));

            Assert.AreEqual(2, payrollList[1].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[1].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[1].CutOffEndDate);
            Assert.AreEqual(false, payrollList[1].IsTaxed);
            Assert.AreEqual(0, payrollList[1].TotalAdjustment);
            Assert.AreEqual(0, payrollList[1].TotalDeduction);
            Assert.AreEqual(902.43M, decimal.Round(payrollList[1].TotalGross, 2));
            Assert.AreEqual(902.43M, decimal.Round(payrollList[1].TotalNet, 2));
            Assert.AreEqual(902.43M, decimal.Round(payrollList[1].TaxableIncome, 2));
            Assert.AreEqual(12.31M, decimal.Round(payrollList[1].TotalAllowance, 2));

        }

        [TestMethod]
        public void ComputeAllowanceNone()
        {
            Initialize();
            DeleteInfo();

            //Test Data
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
                FirstName = "Cornelio Jr.",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("10/30/1989"),
                Gender = 2,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                Salary = 120,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 350
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                Salary = 150,
                SalaryFrequency = FrequencyType.Hourly,
                Allowance = 400
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHours = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/12/2016"),
                Hours = 8,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours2 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/12/2016"),
                Hours = 3,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours3 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/12/2016"),
                Hours = 5,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours4 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/12/2016"),
                Hours = 2,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours5 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/13/2016"),
                Hours = 10,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours6 = new TotalEmployeeHours
            {
                EmployeeId = 1,
                Date = DateTime.Parse("05/13/2016"),
                Hours = 3,
                Type = RateType.OverTime,
                TotalEmployeeHoursId = 2
            };

            var totalEmployeeHours7 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/13/2016"),
                Hours = 1,
                Type = RateType.Regular,
                TotalEmployeeHoursId = 1
            };

            var totalEmployeeHours8 = new TotalEmployeeHours
            {
                EmployeeId = 2,
                Date = DateTime.Parse("05/13/2016"),
                Hours = 2,
                Type = RateType.NightDifferential,
                TotalEmployeeHoursId = 2
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHours);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours5);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours6);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours7);
            _totalEmployeeHoursRepository.Add(totalEmployeeHours8);

            var payrollDate = DateTime.Parse("05/18/2016");

            var employeePayrollItem = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 220.55M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem2 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 100,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem3 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 15,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem4 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 300.10M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem5 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 50,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem6 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 150.12M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem7 = new EmployeePayrollItem
            {
                EmployeeId = 1,
                TotalAmount = 40,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            var employeePayrollItem8 = new EmployeePayrollItem
            {
                EmployeeId = 2,
                TotalAmount = 540.02M,
                PayrollDate = payrollDate,
                RateType = RateType.RegularHolidayRestDay,
                Multiplier = 1,
                RatePerHour = 0.25M
            };

            _employeePayrollItemRepository.Add(employeePayrollItem);
            _employeePayrollItemRepository.Add(employeePayrollItem2);
            _employeePayrollItemRepository.Add(employeePayrollItem3);
            _employeePayrollItemRepository.Add(employeePayrollItem4);
            _employeePayrollItemRepository.Add(employeePayrollItem5);
            _employeePayrollItemRepository.Add(employeePayrollItem6);
            _employeePayrollItemRepository.Add(employeePayrollItem7);
            _employeePayrollItemRepository.Add(employeePayrollItem8);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/11/2016");
            var dateEnd = DateTime.Parse("05/17/2016");

            _employeePayrollService.GeneratePayroll(dateStart, dateEnd);
            _unitOfWork.Commit();

            var payrollList = _employeePayrollService.GetByDateRange(payrollDate, payrollDate);

            Assert.IsNotNull(payrollList);
            Assert.AreEqual(2, payrollList.Count());

            Assert.AreEqual(1, payrollList[0].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[0].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[0].CutOffEndDate);
            Assert.AreEqual(false, payrollList[0].IsTaxed);
            Assert.AreEqual(0, payrollList[0].TotalAdjustment);
            Assert.AreEqual(0, payrollList[0].TotalDeduction);
            Assert.AreEqual(525.67M, payrollList[0].TotalGross);
            Assert.AreEqual(525.67M, payrollList[0].TotalNet);
            Assert.AreEqual(525.67M, payrollList[0].TaxableIncome);
            Assert.AreEqual(0, decimal.Round(payrollList[0].TotalAllowance, 2));

            Assert.AreEqual(2, payrollList[1].EmployeeId);
            Assert.AreEqual(dateStart, payrollList[1].CutOffStartDate);
            Assert.AreEqual(dateEnd, payrollList[1].CutOffEndDate);
            Assert.AreEqual(false, payrollList[1].IsTaxed);
            Assert.AreEqual(0, payrollList[1].TotalAdjustment);
            Assert.AreEqual(0, payrollList[1].TotalDeduction);
            Assert.AreEqual(890.12M, payrollList[1].TotalGross);
            Assert.AreEqual(890.12M, payrollList[1].TotalNet);
            Assert.AreEqual(890.12M, payrollList[1].TaxableIncome);
            Assert.AreEqual(0, decimal.Round(payrollList[1].TotalAllowance, 2));
        }

        [TestMethod]
        public void GetPayrollDatesTest()
        {
            Initialize();
            //get dates for 6 months
            var dates = _employeePayrollService.GetPayrollDates(6);
            Assert.IsTrue(dates.Any());
        }
    }
}
