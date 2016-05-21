using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;
        private IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;
        private ISettingRepository _settingRepository;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(databaseFactory);

            _employeeDailyPayrollRepository = new EmployeeDailyPayrollRepository(databaseFactory);
            _employeePayrollRepository = new EmployeePayrollRepository(databaseFactory);
            _settingRepository = new SettingRepository(databaseFactory);
            _employeePayrollDeductionRepository = new EmployeePayrollDeductionRepository(databaseFactory);

            _settingService = new SettingService(_settingRepository);
            _employeeDailyPayrollService = new EmployeeDailyPayrollService(_unitOfWork, 
                null, null, null, null, _employeeDailyPayrollRepository, null, null);
            _employeePayrollDeductionService = new EmployeePayrollDeductionService(_unitOfWork, _settingService, null, null, null, null,null, null, null);
            _employeePayrollService = new EmployeePayrollService(_unitOfWork, _employeeDailyPayrollService, _employeePayrollRepository, _settingService, _employeePayrollDeductionService);
        }

        private void DeleteInfo()
        {
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_daily_payroll");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE payroll");
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
    }
}
