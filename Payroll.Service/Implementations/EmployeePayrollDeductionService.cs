using Payroll.Entities;
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
        private IEmployeePayrollService _employeePayrollService;

        private IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;

        private readonly String IS_DEDUCTION_SEMIMONTHLY = "DEDUCTION_IS_SEMIMONTHLY";

        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_1 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_1";
        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_2 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_2";
        private readonly String DEDUCTION_MONTHLY_SCHEDULE = "DEDUCTION_MONTHLY_SCHEDULE";

        private readonly String SEMIMONTHLY_TOTAL_HOURS = "DEDUCTION_SEMIMONTHLY_TOTAL_HOURS";
        private readonly String MONTHLY_TOTAL_HOURS = "DEDUCTION_MONTHLY_TOTAL_HOURS";

        public EmployeePayrollDeductionService(IUnitOfWork unitOfWork, ISettingService settingService,
            IEmployeeSalaryService employeeSalaryService, IEmployeeInfoService employeeInfoService,
            IEmployeeDeductionService employeeDeductionService, IDeductionService deductionService,
            IEmployeePayrollDeductionRepository employeePayrollDeductionRepository,
            IEmployeePayrollService employeePayrollService)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _employeeSalaryService = employeeSalaryService;
            _employeeInfoService = employeeInfoService;
            _employeeDeductionService = employeeDeductionService;
            _deductionService = deductionService;
            _employeePayrollDeductionRepository = employeePayrollDeductionRepository;
            _employeePayrollService = employeePayrollService;
        }

        public int GenerateDeductionsByPayroll(DateTime payrollDate, DateTime payrollStartDate, 
            DateTime payrollEndDate, IList<EmployeePayroll> employeePayrolls)
        {
            //If proceed is false return
            if (!proceedDeduction(payrollStartDate, payrollEndDate))
            {
                return 1;
            }

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

            foreach (EmployeePayroll payroll in employeePayrolls)
            {
                //Compute basic pay
                //decimal basicPay = (_employeeSalaryService.GetEmployeeHourlyRate(employee) * (decimal)totalHours);
                //Compute HDMF Deduction
                //Compute SSS contribution

                //No computations from employee deductions info
                //Get all deductions
                var employee = _employeeInfoService.GetByEmployeeId(payroll.EmployeeId);
                var deductionList = _deductionService.GetAllActive();

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
                                DeductionId = deduction.DeductionId,
                                Amount = employeeDeduction.Amount,
                                PayrollDate = payrollDate
                            };

                        _employeePayrollDeductionRepository.Add(employeePayrollDeduction);
                        totalDeductions += employeeDeduction.Amount;
                    }
                }

                //Update employeePayroll total deductions and taxable income
                _employeePayrollService.Update(payroll);
                payroll.TotalDeduction = totalDeductions;
                payroll.TaxableIncome = payroll.TotalNet - payroll.TotalDeduction;
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

        private bool proceedDeduction(DateTime payrollStartDate, DateTime payrollEndDate)
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

                if (payrollStartDate.Day <= deductionSchedule &&
                        payrollEndDate.Day >= deductionSchedule)
                {
                    proceed = true;
                }
            }

            return proceed;
        }
    }
}
