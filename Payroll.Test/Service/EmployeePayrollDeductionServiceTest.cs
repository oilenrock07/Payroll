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

            _taxService = new TaxService(_taxRepository);

            _employeePayrollService = new EmployeePayrollService(_unitOfWork, null, _employeePayrollRepository, _settingService, null, null);
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
                Amount = 100.10M
            };

            var employeeDeduction2 = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = SSS_DEDUCTION_ID,
                Amount = 50.25M
            };

            var employeeDeduction3 = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = PHILHEALTH_DEDUCTION_ID,
                Amount = 50.25M
            };

            var employeeDeduction4 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = HDMF_DEDUCTION_ID,
                Amount = 100.20M
            };

            var employeeDeduction5 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = SSS_DEDUCTION_ID,
                Amount = 45.5M
            };

            var employeeDeduction6 = new EmployeeDeduction
            {
                EmployeeId = 2,
                DeductionId = PHILHEALTH_DEDUCTION_ID,
                Amount = 55.90M
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
                TaxableIncome = 1500,
                TotalNet = 1500,
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                TotalGross = 1500
            };

            var employeePayroll2 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 1300.25M,
                TotalNet = 1300.25M,
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                TotalGross = 1300.25M
            };

            var employeePayroll3 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 1500.25M,
                TotalNet = 1500.25M,
                CutOffStartDate = DateTime.Parse("05/18/2016"),
                CutOffEndDate = DateTime.Parse("05/24/2016"),
                PayrollDate = DateTime.Parse("05/25/2016"),
                TotalGross = 1500.25M
            };

            var employeePayroll4 = new EmployeePayroll
            {
                EmployeeId = 1,
                IsTaxed = false,
                TaxableIncome = 950.50M,
                TotalNet = 950.50M,
                CutOffStartDate = DateTime.Parse("05/25/2016"),
                CutOffEndDate = DateTime.Parse("05/31/2016"),
                PayrollDate = DateTime.Parse("06/01/2016"),
                TotalGross = 950.50M
            };

            var employeePayroll5 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 2000.05M,
                TotalNet = 2000.05M,
                CutOffStartDate = DateTime.Parse("05/04/2016"),
                CutOffEndDate = DateTime.Parse("05/10/2016"),
                PayrollDate = DateTime.Parse("05/11/2016"),
                TotalGross = 2000.05M
            };

            var employeePayroll6 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 2100,
                TotalNet = 2100,
                CutOffStartDate = DateTime.Parse("05/11/2016"),
                CutOffEndDate = DateTime.Parse("05/17/2016"),
                PayrollDate = DateTime.Parse("05/18/2016"),
                TotalGross = 2100
            };

            var employeePayroll7 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 2200.50M,
                TotalNet = 2200.50M,
                CutOffStartDate = DateTime.Parse("05/18/2016"),
                CutOffEndDate = DateTime.Parse("05/24/2016"),
                PayrollDate = DateTime.Parse("05/25/2016"),
                TotalGross = 2200.50M
            };

            var employeePayroll8 = new EmployeePayroll
            {
                EmployeeId = 2,
                IsTaxed = false,
                TaxableIncome = 750.50M,
                TotalNet = 750.50M,
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
            payrollList.Add(employeePayroll8);

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
            Assert.AreEqual(749.9M, finalizedPayroll[0].TaxableIncome);
            Assert.AreEqual(950.50M, finalizedPayroll[0].TotalGross);
            Assert.AreEqual(247.31M, finalizedPayroll[0].TotalDeduction);
            Assert.AreEqual(703.190M, finalizedPayroll[0].TotalNet);

            //Get all deductions
            IList<EmployeePayrollDeduction> employeePayrollDeduction 
                = _employeePayrollDeductionService.GetByPayroll(4);

            Assert.AreEqual(4, employeePayrollDeduction.Count());

            Assert.AreEqual(employeePayrollDeduction[0].EmployeeId, 1);
            Assert.AreEqual(employeePayrollDeduction[0].EmployeePayrollId, 4);
            Assert.AreEqual(employeePayrollDeduction[0].Amount, 100.10M);
            Assert.AreEqual(employeePayrollDeduction[0].DeductionId, HDMF_DEDUCTION_ID);

            Assert.AreEqual(employeePayrollDeduction[1].EmployeeId, 1);
            Assert.AreEqual(employeePayrollDeduction[1].EmployeePayrollId, 4);
            Assert.AreEqual(employeePayrollDeduction[1].Amount, 50.25M);
            Assert.AreEqual(employeePayrollDeduction[1].DeductionId, SSS_DEDUCTION_ID);

            Assert.AreEqual(employeePayrollDeduction[2].EmployeeId, 1);
            Assert.AreEqual(employeePayrollDeduction[2].EmployeePayrollId, 4);
            Assert.AreEqual(employeePayrollDeduction[2].Amount, 50.25M);
            Assert.AreEqual(employeePayrollDeduction[2].DeductionId, PHILHEALTH_DEDUCTION_ID);

            //Check Tax
            Assert.AreEqual(employeePayrollDeduction[3].EmployeeId, 1);
            Assert.AreEqual(employeePayrollDeduction[3].EmployeePayrollId, 4);
            Assert.AreEqual(employeePayrollDeduction[3].Amount, 46.71M);
            Assert.AreEqual(employeePayrollDeduction[3].DeductionId, TAX_DEDUCTION_ID);
        
            Assert.AreEqual(2, finalizedPayroll[1].EmployeeId);
            Assert.AreEqual(true, finalizedPayroll[1].IsTaxed);
            Assert.AreEqual(DateTime.Parse("05/25/2016"), finalizedPayroll[1].CutOffStartDate);
            Assert.AreEqual(DateTime.Parse("05/31/2016"), finalizedPayroll[1].CutOffEndDate);
            Assert.AreEqual(DateTime.Parse("06/01/2016"), finalizedPayroll[1].PayrollDate);
            Assert.AreEqual(548.90M, finalizedPayroll[1].TaxableIncome);
            Assert.AreEqual(750.50M, finalizedPayroll[1].TotalGross);
            Assert.AreEqual(437.2975M, finalizedPayroll[1].TotalDeduction);
            Assert.AreEqual(313.2025M, finalizedPayroll[1].TotalNet);

            employeePayrollDeduction
               = _employeePayrollDeductionService.GetByPayroll(8);

            Assert.AreEqual(4, employeePayrollDeduction.Count());

            Assert.AreEqual(employeePayrollDeduction[0].EmployeeId, 2);
            Assert.AreEqual(employeePayrollDeduction[0].EmployeePayrollId, 8);
            Assert.AreEqual(employeePayrollDeduction[0].Amount, 100.20M);
            Assert.AreEqual(employeePayrollDeduction[0].DeductionId, HDMF_DEDUCTION_ID);

            Assert.AreEqual(employeePayrollDeduction[1].EmployeeId, 2);
            Assert.AreEqual(employeePayrollDeduction[1].EmployeePayrollId, 8);
            Assert.AreEqual(employeePayrollDeduction[1].Amount, 45.5M);
            Assert.AreEqual(employeePayrollDeduction[1].DeductionId, SSS_DEDUCTION_ID);

            Assert.AreEqual(employeePayrollDeduction[2].EmployeeId, 2);
            Assert.AreEqual(employeePayrollDeduction[2].EmployeePayrollId, 8);
            Assert.AreEqual(employeePayrollDeduction[2].Amount, 55.90M);
            Assert.AreEqual(employeePayrollDeduction[2].DeductionId, PHILHEALTH_DEDUCTION_ID);

            //Check Tax
            Assert.AreEqual(employeePayrollDeduction[3].EmployeeId, 2);
            Assert.AreEqual(employeePayrollDeduction[3].EmployeePayrollId, 8);
            Assert.AreEqual(employeePayrollDeduction[3].Amount, 235.6975M);
            Assert.AreEqual(employeePayrollDeduction[3].DeductionId, TAX_DEDUCTION_ID);
        }
    }
}
