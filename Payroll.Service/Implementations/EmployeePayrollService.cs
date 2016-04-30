using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollService : IEmployeePayrollService
    {
        private UnitOfWork _unitOfWork;
        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeeDailyPayrollService _employeeDailyPayrollService;

        public EmployeePayrollService(UnitOfWork unitOfWork, IEmployeeDailyPayrollService employeeDailyPayrollService, 
            IEmployeePayrollRepository employeeePayrollRepository)
        {
            _unitOfWork = unitOfWork;
            _employeeDailyPayrollService = employeeDailyPayrollService;
            _employeePayrollRepository = employeeePayrollRepository;
        }

        public void GeneratePayrollByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo)
        {
            var employeeDailyPayroll = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            //Hold last payroll processed
            EmployeePayroll tempEmployeePayroll = null;

            DateTime today = new DateTime();
            EmployeeDailyPayroll last = employeeDailyPayroll.Last();

            foreach (EmployeeDailyPayroll dailyPayroll in employeeDailyPayroll)
            {
                //If should create new entry
                if (tempEmployeePayroll != null && 
                    (tempEmployeePayroll.EmployeeId != tempEmployeePayroll.EmployeeId))
                {
                    //Save last entry if for different employee
                    _employeePayrollRepository.Add(tempEmployeePayroll);

                    EmployeePayroll employeePayroll = new EmployeePayroll
                    {
                        EmployeeId = dailyPayroll.EmployeeId,
                        CutOffStartDate = dateFrom,
                        CutOffEndDate = dateTo,
                        PayrollGeneratedDate = today,
                        PayrollDate = payrollDate,
                        TotalNet = dailyPayroll.TotalPay
                    };

                    tempEmployeePayroll = employeePayroll;

                }
                else
                {
                    //Update last entry
                    tempEmployeePayroll.TotalNet += dailyPayroll.TotalPay;
                }

                //If last iteration save
                if (dailyPayroll.Equals(last))
                {
                    _employeePayrollRepository.Add(tempEmployeePayroll);
                }
            }

            //Commit
            _unitOfWork.Commit();
        }
    }
}
