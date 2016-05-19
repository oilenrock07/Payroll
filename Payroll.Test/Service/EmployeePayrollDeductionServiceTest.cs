using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
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
    public class EmployeePayrollDeductionServiceTest
    {
        private IUnitOfWork _unitOfWork;
        private ISettingService _settingService;
        private IEmployeeSalaryService _employeeSalaryService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeDeductionService _employeeDeductionService;
        private IDeductionService _deductionService;
        private IEmployeePayrollService _employeePayrollService;
        private ITaxService _taxService;

        private ISettingRepository _settingRepository;
        private IEmployeeInfoRepository _employeeInfoRepository;
        private IEmployeeDeductionRepository _employeeDeductionRepository;
        private IDeductionRepository _deductionRepository;
        private IEmployeePayrollRepository _employeePayrollRepository;
        private ITaxRepository _taxRepository;
        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;

        private IEmployeePayrollDeductionService _employeePayrollDeductionService;

        private readonly int TAX_DEDUCTION_ID = 1;
        private readonly int HDMF_DEDUCTION_ID = 2;
        private readonly int SSS_DEDUCTION_ID = 3;
        private readonly int PHILHEALTH_DEDUCTION_ID = 4;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(databaseFactory);

            _settingRepository = new SettingRepository(databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(databaseFactory);
            _employeeDeductionRepository = new EmployeeDeductionRepository(databaseFactory);
            _deductionRepository = new DeductionRepository(databaseFactory);
            _employeePayrollRepository = new EmployeePayrollRepository(databaseFactory);
            _employeePayrollDeductionRepository = new EmployeePayrollDeductionRepository(databaseFactory);
            _taxRepository = new TaxRepository(databaseFactory);

            _settingService = new SettingService(_settingRepository);
            _employeeSalaryService = new EmployeeSalaryService();
            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _employeeDeductionService = new EmployeeDeductionService(_employeeDeductionRepository);
            _deductionService = new DeductionService(_deductionRepository);
            _employeePayrollService = new EmployeePayrollService(_unitOfWork, null, _employeePayrollRepository, _settingService);
            _taxService = new TaxService(_taxRepository);

            _employeePayrollDeductionService = new EmployeePayrollDeductionService(_unitOfWork, _settingService, _employeeSalaryService, _employeeInfoService, _employeeDeductionService, _deductionService, _employeePayrollDeductionRepository, _employeePayrollService, _taxService);

        }

        private void DeleteInfo()
        {
            _employeePayrollDeductionRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_deduction");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee_payroll_deduction");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE payroll");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            _employeePayrollDeductionRepository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        [TestMethod]
        public void GenerateDeduction()
        {
            Initialize();
            DeleteInfo();

            //Initialize Data
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
                BirthDate = DateTime.Parse("10/30/1981"),
                Gender = 1,
                IsActive = true
            };

            var employeeInfo = new EmployeeInfo
            {
                Employee = employee,
                SalaryFrequency = FrequencyType.Weekly,
                Salary = 1000
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                SalaryFrequency = FrequencyType.Weekly,
                Salary = 900
            };

            _employeeInfoRepository.Add(employeeInfo);
            _employeeInfoRepository.Add(employeeInfo2);

            var employeeDeduction = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = HDMF_DEDUCTION_ID,
                Amount = 101.10M
            };

            var employeeDeduction2 = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = SSS_DEDUCTION_ID,
                Amount = 202.25M
            };

            var employeeDeduction3 = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = PHILHEALTH_DEDUCTION_ID,
                Amount = 350
            };

            var employeeDeduction4 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = HDMF_DEDUCTION_ID,
                Amount = 551.20M
            };

            var employeeDeduction5 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = SSS_DEDUCTION_ID,
                Amount = 100.5M
            };

            var employeeDeduction6 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = PHILHEALTH_DEDUCTION_ID,
                Amount = 999.90M
            };

            _employeeDeductionRepository.Add(employeeDeduction);
            _employeeDeductionRepository.Add(employeeDeduction2);
            _employeeDeductionRepository.Add(employeeDeduction3);
            _employeeDeductionRepository.Add(employeeDeduction4);
            _employeeDeductionRepository.Add(employeeDeduction5);
            _employeeDeductionRepository.Add(employeeDeduction6);

            var employeePayroll = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 1200,
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                TotalGross = 1200
            };

            var employeePayroll2 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 1100.25M,
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                TotalGross = 1100
            };

            var employeePayroll3 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 1000.25M,
                CutOffStartDate = DateTime.Parse("05/18/2016"),
                CutOffEndDate = DateTime.Parse("05/24/2016"),
                PayrollDate = DateTime.Parse("05/25/2016"),
                TotalGross = 1000.25M
            };

            var employeePayroll4 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 950.50M,
                CutOffStartDate = DateTime.Parse("05/25/2016"),
                CutOffEndDate = DateTime.Parse("05/31/2016"),
                PayrollDate = DateTime.Parse("06/01/2016"),
                TotalGross = 950.50M
            };

            var employeePayroll5 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 900.05M,
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                TotalGross = 900.05M
            };

            var employeePayroll6 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 800,
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                TotalGross = 800
            };

            var employeePayroll7 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 750.50M,
                CutOffStartDate = DateTime.Parse("05/18/2016"),
                CutOffEndDate = DateTime.Parse("05/24/2016"),
                PayrollDate = DateTime.Parse("05/25/2016"),
                TotalGross = 750.50M
            };

            var employeePayroll8 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 750.50M,
                CutOffStartDate = DateTime.Parse("05/25/2016"),
                CutOffEndDate = DateTime.Parse("05/31/2016"),
                PayrollDate = DateTime.Parse("06/01/2016"),
                TotalGross = 750.50M
            };


            _employeePayrollRepository.Add(employeePayroll);
            _employeePayrollRepository.Add(employeePayroll2);
            _employeePayrollRepository.Add(employeePayroll3);
            _employeePayrollRepository.Add(employeePayroll4);
            _employeePayrollRepository.Add(employeePayroll5);
            _employeePayrollRepository.Add(employeePayroll6);
            _employeePayrollRepository.Add(employeePayroll7);
            _employeePayrollRepository.Add(employeePayroll8);

            _unitOfWork.Commit();

            //Current payroll
            IList<EmployeePayroll> payrollList = new List<EmployeePayroll>();
            payrollList.Add(employeePayroll4);
            payrollList.Add(employeePayroll7);

            //Test
            _employeePayrollDeductionService.GenerateDeductionsByPayroll(
                DateTime.Parse("06/01/2016"), DateTime.Parse("05/25/2016"),
                DateTime.Parse("05/31/2016"), payrollList);

            //Verify
            var startDate = DateTime.Parse("06/01/2016");
            var endDate = DateTime.Parse("06/01/2016");

            IList<EmployeePayroll> finalizedPayroll = _employeePayrollService.GetByDateRange(startDate, endDate);

            Assert.IsNotNull(finalizedPayroll);

            Assert.AreEqual(2, finalizedPayroll.Count());

            Assert.AreEqual(1, finalizedPayroll[0].EmployeeId);
            Assert.AreEqual(true, finalizedPayroll[0].IsTaxed);
            Assert.AreEqual(DateTime.Parse("05/25/2016"), finalizedPayroll[0].CutOffStartDate);
            Assert.AreEqual(DateTime.Parse("05/31/2016"), finalizedPayroll[0].CutOffEndDate);
            Assert.AreEqual(DateTime.Parse("06/01/2016"), finalizedPayroll[0].PayrollDate);
            Assert.AreEqual(297.15M, finalizedPayroll[0].TaxableIncome);
            Assert.AreEqual(950.50M, finalizedPayroll[0].TotalGross);
            Assert.AreEqual(653.35M, finalizedPayroll[0].TotalDeduction);
            Assert.AreEqual(297.15M, finalizedPayroll[0].TotalNet);
        }

    }
}
