using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private readonly int TAX_DEDUCTION_HDMF = 2;
        private readonly int TAX_DEDUCTION_SSS = 3;
        private readonly int TAX_DEDUCTION_PHILHEALTH = 4;

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

        [TestMethod]
        public void GenerateDeduction()
        {
            Initialize();

            //Initialize Data
            var employeeDeduction = new EmployeeDeduction
            {
                EmployeeId = 1,
                DeductionId = TAX_DEDUCTION_HDMF,
                Amount = 100
            };

        }
       
    }
}
