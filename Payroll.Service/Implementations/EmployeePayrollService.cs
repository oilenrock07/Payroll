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

            foreach (EmployeeDailyPayroll dailyPayroll in employeeDailyPayroll)
            {
                EmployeePayroll employeePayroll = null;
              
                //If should create new entry
                if (tempEmployeePayroll != null && 
                    (tempEmployeePayroll.EmployeeId != tempEmployeePayroll.EmployeeId))
                {
                    //Save last entry if for different employee
                    _employeePayrollRepository.Add(tempEmployeePayroll);

                    employeePayroll = new EmployeePayroll
                    {
                        EmployeeId = dailyPayroll.EmployeeId,
                        CutOffStartDate = dateFrom,
                        CutOffEndDate = dateTo,
                        PayrollGeneratedDate = today,
                        PayrollDate = payrollDate
                    };

                }
                else
                {
                    //Update last entry

                }
            }
        }
    }
}
