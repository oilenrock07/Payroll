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
    public class TaxService : BaseEntityService<Tax>, ITaxService
    {
        private IUnitOfWork _unitOfWork;
        private ITaxRepository _taxRepository;
        private ISettingService _settingService;
        private IEmployeePayrollService _employeePayrollService;
        private IEmployeePayrollDeductionService _employeePayrollDeductionService;

        private readonly int MAX_DEPENDENT = 4;
        private readonly string TAX_FREQUENCY = "TAX_FREQUENCY";

        public TaxService(ITaxRepository taxRepository, ISettingService settingService,
            IEmployeePayrollService employeePayrollService, 
            IEmployeePayrollDeductionService employeePayrollDeductionService,
            IUnitOfWork unitOfWork)
            : base(taxRepository)
        {
            _taxRepository = taxRepository;
            _settingService = settingService;
            _employeePayrollService = employeePayrollService;
            _employeePayrollDeductionService = employeePayrollDeductionService;
            _unitOfWork = unitOfWork;
        }

        public int ComputeTax(Deduction taxDeduction, EmployeeInfo employeeInfo)
        {
            var frequency = _settingService.GetByKey(TAX_FREQUENCY);
            var noOfDependents =
                employeeInfo.Dependents > MAX_DEPENDENT ? 4 : employeeInfo.Dependents;

            FrequencyType taxFrequency = (FrequencyType)Convert.ToInt32(frequency);

            //Get all employee payroll for tax computation
            var payrolls = _employeePayrollService.GetForTaxProcessingByEmployee(employeeInfo.EmployeeId);

            //Get total taxable income
            decimal totalTaxableIncome = 0;

            //Get latest payroll, since payroll is orderdbydesc payroll date
               //Will get the first entry
            var currentPayroll = payrolls[0];

            foreach (EmployeePayroll payroll in payrolls)
            {
                totalTaxableIncome += payroll.TaxableIncome;

                //Update Payroll
                _employeePayrollService.Update(payroll);
                payroll.IsTaxed = true;

            }

            var taxAmount = ComputeTax(taxFrequency, noOfDependents, totalTaxableIncome);

            //Create deductions
            if (taxAmount > 0)
            {
                var employeePayrollDeduction = new EmployeePayrollDeduction
                {
                    EmployeeId = employeeInfo.EmployeeId,
                    DeductionId = taxDeduction.DeductionId,
                    Amount = taxAmount,
                    PayrollDate = currentPayroll.PayrollDate
                };

                _employeePayrollDeductionService.Add(employeePayrollDeduction);

                //Update employee payroll deduction and gross salary
                _employeePayrollService.Update(currentPayroll);

                currentPayroll.TotalDeduction += taxAmount;
                currentPayroll.TotalGross = currentPayroll.TotalNet - taxAmount;
            }

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }

        private decimal ComputeTax(FrequencyType frequency, int dependentCount, decimal totalTaxableIncome)
        {
            var tax =
               _taxRepository.GetByTaxableAmount(frequency, dependentCount, totalTaxableIncome);

            decimal totalTaxAmount = 0;

            if (tax != null)
            {
                //Compute tax
                totalTaxAmount = tax.BaseTaxAmount;
            
                //For excess of base amount
                if (totalTaxableIncome > tax.BaseTaxAmount)
                {
                    totalTaxAmount += (totalTaxableIncome - tax.BaseTaxAmount) * (tax.OverPercentage / 100);
                }
            }

            return totalTaxAmount;
        }
    }
}
