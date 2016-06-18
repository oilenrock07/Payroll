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

        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;
        private IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;
        private ISettingRepository _settingRepository;
        private IEmployeeInfoRepository _employeeInfoRepository;
        private ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private IDeductionRepository _deductionRepository;
        private IEmployeeDeductionService _employeeDeductionService;
        private ITaxRepository _taxRepository;

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

            _settingService = new SettingService(_settingRepository);
            _employeeDailyPayrollService = new EmployeeDailyPayrollService(_unitOfWork, 
                null, null, null, null, _employeeDailyPayrollRepository, null, null);
            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _deductionService = new DeductionService(_deductionRepository);
            _employeeDeductionService = new EmployeeDeductionService(_employeeDeductionRepository);
            _taxService = new TaxService(_taxRepository);
            _employeePayrollDeductionService = new EmployeePayrollDeductionService(_unitOfWork, _settingService, null, _employeeInfoService, _employeeDeductionService, _deductionService, _employeePayrollDeductionRepository, _taxService);

            _totalEmployeeHoursService = new TotalEmployeeHoursService(_unitOfWork, _totalEmployeeHoursRepository, null, _settingService);

            _employeePayrollService = new EmployeePayrollService(_unitOfWork,
                _employeeDailyPayrollService, _employeePayrollRepository, _settingService, _employeePayrollDeductionService, _employeeInfoService, _totalEmployeeHoursService);
        }

        private void DeleteInfo()
        {
            _employeePayrollDeductionRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_daily_payroll");
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

            var frequency = FrequencyType.Weekly;
            DateTime date = DateTime.Parse("04/27/2016");

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(date);

            Assert.AreEqual(DateTime.Parse("04/20/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("04/26/2016"), payrollEndDate);
        }

        [TestMethod]
        public void GetPayrollNextStartDate2()
        {
            Initialize();
            DeleteInfo();

            var frequency = FrequencyType.Weekly;
            DateTime date = DateTime.Parse("05/17/2016");

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(date);

            Assert.AreEqual(DateTime.Parse("05/11/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
              .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("05/17/2016"), payrollEndDate);
        }

        [TestMethod]
        public void GetPayrollNextStartDate3()
        {
            Initialize();
            DeleteInfo();

            var employeePayroll = new EmployeePayroll()
            {
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                PayrollGeneratedDate = DateTime.Parse("05/18/2016"),
                EmployeeId = 1
            };

            var employeePayroll2 = new EmployeePayroll()
            {
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                PayrollGeneratedDate = DateTime.Parse("05/11/2016"),
                EmployeeId = 1
            };

            _employeePayrollRepository.Add(employeePayroll);
            _employeePayrollRepository.Add(employeePayroll2);

            _unitOfWork.Commit();

            var frequency = FrequencyType.Weekly;

            DateTime payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate();

            Assert.AreEqual(DateTime.Parse("05/18/2016"), payrollStartDate);

            DateTime payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(payrollStartDate);

            Assert.AreEqual(DateTime.Parse("05/24/2016"), payrollEndDate);
        }

        [TestMethod]
        public void GeneratePayrollNetPayByDateRange(){
            Initialize();
            DeleteInfo();

            //Test Data
            var dailyPayroll = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = 220.55M,
                Date = DateTime.Parse("05/19/2016")
            };

            var dailyPayroll2 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = 100,
                Date = DateTime.Parse("05/19/2016")
            };

            var dailyPayroll3 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 3,
                TotalPay = 15,
                Date = DateTime.Parse("05/19/2016")
            };

            var dailyPayroll4 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 4,
                TotalPay = 300.10M,
                Date = DateTime.Parse("05/19/2016")
            };

            var dailyPayroll5 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = 50,
                Date = DateTime.Parse("05/19/2016")
            };

            var dailyPayroll6 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 6,
                TotalPay = 150.12M,
                Date = DateTime.Parse("05/20/2016")
            };

            var dailyPayroll7 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 7,
                TotalPay = 40,
                Date = DateTime.Parse("05/20/2016")
            };

            var dailyPayroll8 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 8,
                TotalPay = 540.02M,
                Date = DateTime.Parse("05/20/2016")
            };

            _employeeDailyPayrollRepository.Add(dailyPayroll);
            _employeeDailyPayrollRepository.Add(dailyPayroll2);
            _employeeDailyPayrollRepository.Add(dailyPayroll3);
            _employeeDailyPayrollRepository.Add(dailyPayroll4);
            _employeeDailyPayrollRepository.Add(dailyPayroll5);
            _employeeDailyPayrollRepository.Add(dailyPayroll6);
            _employeeDailyPayrollRepository.Add(dailyPayroll7);
            _employeeDailyPayrollRepository.Add(dailyPayroll8);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/19/2016");
            var dateEnd = DateTime.Parse("05/20/2016");
            var payrollDate = DateTime.Parse("05/21/2016");

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

            var dailyPayroll = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = 220.55M,
                Date = DateTime.Parse("05/06/2016")
            };

            var dailyPayroll2 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = 100,
                Date = DateTime.Parse("05/06/2016")
            };

            var dailyPayroll3 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 3,
                TotalPay = 15,
                Date = DateTime.Parse("05/06/2016")
            };

            var dailyPayroll4 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 4,
                TotalPay = 300.10M,
                Date = DateTime.Parse("05/06/2016")
            };

            var dailyPayroll5 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = 50,
                Date = DateTime.Parse("05/07/2016")
            };

            var dailyPayroll6 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 6,
                TotalPay = 150.12M,
                Date = DateTime.Parse("05/07/2016")
            };

            var dailyPayroll7 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 7,
                TotalPay = 40,
                Date = DateTime.Parse("05/07/2016")
            };

            var dailyPayroll8 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 8,
                TotalPay = 540.02M,
                Date = DateTime.Parse("05/07/2016")
            };

            _employeeDailyPayrollRepository.Add(dailyPayroll);
            _employeeDailyPayrollRepository.Add(dailyPayroll2);
            _employeeDailyPayrollRepository.Add(dailyPayroll3);
            _employeeDailyPayrollRepository.Add(dailyPayroll4);
            _employeeDailyPayrollRepository.Add(dailyPayroll5);
            _employeeDailyPayrollRepository.Add(dailyPayroll6);
            _employeeDailyPayrollRepository.Add(dailyPayroll7);
            _employeeDailyPayrollRepository.Add(dailyPayroll8);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/06/2016");
            var dateEnd = DateTime.Parse("05/07/2016");
            var payrollDate = DateTime.Parse("05/08/2016");

            _employeePayrollService.GeneratePayroll(payrollDate, dateStart, dateEnd);
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

            var dailyPayroll = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = 220.55M,
                Date = DateTime.Parse("05/12/2016")
            };

            var dailyPayroll2 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = 100,
                Date = DateTime.Parse("05/12/2016")
            };

            var dailyPayroll3 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 3,
                TotalPay = 15,
                Date = DateTime.Parse("05/12/2016")
            };

            var dailyPayroll4 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 4,
                TotalPay = 300.10M,
                Date = DateTime.Parse("05/12/2016")
            };

            var dailyPayroll5 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = 50,
                Date = DateTime.Parse("05/13/2016")
            };

            var dailyPayroll6 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 6,
                TotalPay = 150.12M,
                Date = DateTime.Parse("05/13/2016")
            };

            var dailyPayroll7 = new EmployeeDailyPayroll
            {
                EmployeeId = 1,
                TotalEmployeeHoursId = 7,
                TotalPay = 40,
                Date = DateTime.Parse("05/13/2016")
            };

            var dailyPayroll8 = new EmployeeDailyPayroll
            {
                EmployeeId = 2,
                TotalEmployeeHoursId = 8,
                TotalPay = 540.02M,
                Date = DateTime.Parse("05/13/2016")
            };

            _employeeDailyPayrollRepository.Add(dailyPayroll);
            _employeeDailyPayrollRepository.Add(dailyPayroll2);
            _employeeDailyPayrollRepository.Add(dailyPayroll3);
            _employeeDailyPayrollRepository.Add(dailyPayroll4);
            _employeeDailyPayrollRepository.Add(dailyPayroll5);
            _employeeDailyPayrollRepository.Add(dailyPayroll6);
            _employeeDailyPayrollRepository.Add(dailyPayroll7);
            _employeeDailyPayrollRepository.Add(dailyPayroll8);

            _unitOfWork.Commit();

            //Test
            var dateStart = DateTime.Parse("05/12/2016");
            var dateEnd = DateTime.Parse("05/13/2016");
            var payrollDate = DateTime.Parse("05/14/2016");

            _employeePayrollService.GeneratePayroll(payrollDate, dateStart, dateEnd);
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
