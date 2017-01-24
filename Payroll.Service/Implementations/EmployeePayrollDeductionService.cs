using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollDeductionService : IEmployeePayrollDeductionService
    {
        private IUnitOfWork _unitOfWork;
        private ISettingService _settingService;
        private IEmployeeSalaryService _employeeSalaryService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeDeductionService _employeeDeductionService;
        private IDeductionService _deductionService;

        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;
        private ITaxService _taxService;

        private readonly String IS_DEDUCTION_SEMIMONTHLY = "DEDUCTION_IS_SEMIMONTHLY";

        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_1 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_1";
        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_2 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_2";
        private readonly String DEDUCTION_MONTHLY_SCHEDULE = "DEDUCTION_MONTHLY_SCHEDULE";

        private readonly String SEMIMONTHLY_TOTAL_HOURS = "DEDUCTION_SEMIMONTHLY_TOTAL_HOURS";
        private readonly String MONTHLY_TOTAL_HOURS = "DEDUCTION_MONTHLY_TOTAL_HOURS";

        private readonly int MAX_DEPENDENT = 4;
        private readonly String TAX_DEDUCTION_NAME = "Tax";

        public EmployeePayrollDeductionService(IUnitOfWork unitOfWork, ISettingService settingService,
            IEmployeeSalaryService employeeSalaryService, IEmployeeInfoService employeeInfoService,
            IEmployeeDeductionService employeeDeductionService, IDeductionService deductionService,
            IEmployeePayrollDeductionRepository employeePayrollDeductionRepository, 
            ITaxService taxService)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _employeeSalaryService = employeeSalaryService;
            _employeeInfoService = employeeInfoService;
            _employeeDeductionService = employeeDeductionService;
            _deductionService = deductionService;
            _employeePayrollDeductionRepository = employeePayrollDeductionRepository;
            _taxService = taxService;
        }

        public Decimal GenerateDeductionsByPayroll(EmployeePayroll employeePayroll)
        {

            double totalHours = 0;
            //Get total number of hours
            /* if (isSemiMonthly)
             {
                 totalHours = Double.Parse(_settingService.GetByKey(SEMIMONTHLY_TOTAL_HOURS));
             }
             else
             {
                 totalHours = Double.Parse(_settingService.GetByKey(MONTHLY_TOTAL_HOURS));
             }*/

            //Get employees
            //var employeeList = _employeeInfoService.GetAllActive();

            var employee = _employeeInfoService.GetByEmployeeId(employeePayroll.EmployeeId);
            var deductionList = _deductionService.GetAllCustomizable();

            decimal totalDeductions = 0;
            //Every deductions check for available deduction for employee
            foreach (Deduction deduction in deductionList)
            {
                var employeeDeduction = _employeeDeductionService
                    .GetByDeductionAndEmployee(deduction.DeductionId, employee.EmployeeId);

                if (employeeDeduction != null)
                {
                    //Create a deduction entry
                    EmployeePayrollDeduction employeePayrollDeduction =
                        new EmployeePayrollDeduction
                        {
                            EmployeeId = employee.EmployeeId,
                            DeductionId = deduction.DeductionId,
                            Amount = employeeDeduction.Amount,
                            DeductionDate = new DateTime(),
                            EmployeePayrollId = employeePayroll.PayrollId
                        };

                    _employeePayrollDeductionRepository.Add(employeePayrollDeduction);
                    totalDeductions += employeeDeduction.Amount;
                }
            }

            return totalDeductions;
        }

        public decimal ComputeTax(int payrollId, EmployeeInfo employeeInfo, decimal totalTaxableIncome, FrequencyType taxFrequency)
        {
            var taxDeduction = _deductionService.GetByName(TAX_DEDUCTION_NAME);
            var noOfDependents =
                employeeInfo.Dependents > MAX_DEPENDENT ? MAX_DEPENDENT : employeeInfo.Dependents;

            var taxAmount = _taxService.ComputeTax(taxFrequency, noOfDependents, totalTaxableIncome);

            //Create deductions
            if (taxAmount > 0)
            {
                var employeePayrollDeduction = new EmployeePayrollDeduction
                {
                    EmployeeId = employeeInfo.EmployeeId,
                    DeductionId = taxDeduction.DeductionId,
                    Amount = taxAmount,
                    DeductionDate = new DateTime(),
                    EmployeePayrollId = payrollId
                };

                _employeePayrollDeductionRepository.Add(employeePayrollDeduction);
            }

            return taxAmount;
        }

        public bool proceedDeduction(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Get settings if monthly or semimonthly
            bool isSemiMonthly = _settingService.GetByKey(IS_DEDUCTION_SEMIMONTHLY).Equals("1");
            bool proceed = false;

            //Check if payroll should have deduction
            if (isSemiMonthly)
            {
                int firstDeductionSchedule = Convert
                    .ToInt32(_settingService.GetByKey(DEDUCTION_SEMIMONTHLY_SCHEDULE_1));

                int secondDeductionSchedule = Convert
                   .ToInt32(_settingService.GetByKey(DEDUCTION_SEMIMONTHLY_SCHEDULE_2));

                if ((payrollStartDate.Day <= firstDeductionSchedule &&
                        payrollEndDate.Day >= firstDeductionSchedule) ||
                    (payrollStartDate.Day <= secondDeductionSchedule &&
                        payrollEndDate.Day >= secondDeductionSchedule))
                {
                    proceed = true;
                }
            }
            else
            {
                int deductionSchedule = Convert
                   .ToInt32(_settingService.GetByKey(DEDUCTION_MONTHLY_SCHEDULE));

                //Use month 
                DateTime deductionDate =
                    new DateTime(payrollStartDate.Year, payrollStartDate.Month, deductionSchedule);

                //Use month 
                DateTime deductionDate2 =
                    new DateTime(payrollEndDate.Year, payrollEndDate.Month, deductionSchedule);

                if ((payrollStartDate <= deductionDate && payrollEndDate >= deductionDate) 
                        || (payrollStartDate <= deductionDate2 && payrollEndDate >= deductionDate2))
                {
                    proceed = true;
                }
            }

            return proceed;
        }
    
        public EmployeePayrollDeduction Add(EmployeePayrollDeduction employeePayrollDeduction)
        {
            return _employeePayrollDeductionRepository.Add(employeePayrollDeduction);
        }

        public IList<EmployeePayrollDeduction> GetByPayroll(int payrollId)
        {
            return _employeePayrollDeductionRepository.GetByPayroll(payrollId);
        }
    }
}
