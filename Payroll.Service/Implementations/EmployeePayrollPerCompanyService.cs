using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using Payroll.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollPerCompanyService : IEmployeePayrollPerCompanyService
    {
        private IUnitOfWork _unitOfWork;
       
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private IEmployeeService _employeeService;
        private IEmployeePayrollPerCompanyRepository _employeePayrollRepository;
        private ITotalEmployeeHoursPerCompanyService _totalEmployeeHoursService;
        private IEmployeePayrollItemPerCompanyService _employeePayrollItemService;
        private IEmployeePayrollService _employeePayrollService;

        private FrequencyType _frequency;

        private readonly String PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";
        private readonly String PAYROLL_WEEK_START = "PAYROLL_WEEK_START";
        private readonly String PAYROLL_WEEK_END = "PAYROLL_WEEK_END";
        private readonly String PAYROLL_WEEK_RELEASE = "PAYROLL_WEEK_RELEASE";
        private readonly String ALLOWANCE_WEEK_SCHEDULE = "ALLOWANCE_WEEK_SCHEDULE";
        private readonly String ALLOWANCE_DAY_SCHEDULE = "ALLOWANCE_DAY_SCHEDULE";
        private readonly String ALLOWANCE_TOTAL_DAYS = "ALLOWANCE_TOTAL_DAYS";
        private readonly String PAYROLL_TOTAL_HOURS = "PAYROLL_TOTAL_HOURS";
        private readonly String TAX_FREQUENCY = "TAX_FREQUENCY";
        private readonly String TAX_ENABLED = "TAX";

        public EmployeePayrollPerCompanyService(IUnitOfWork unitOfWork, 
            IEmployeePayrollPerCompanyRepository employeeePayrollRepository, 
            ISettingService settingService, 
            IEmployeeInfoService employeeInfoService, 
            ITotalEmployeeHoursPerCompanyService totalEmployeeHourService, 
            IEmployeeService employeeService, 
            IEmployeePayrollItemPerCompanyService employeePayrollItemService,
            IEmployeePayrollService employeePayrollService)
        {
            _unitOfWork = unitOfWork;
            _employeePayrollRepository = employeeePayrollRepository;
            _settingService = settingService;
            _employeeInfoService = employeeInfoService;
            _employeeService = employeeService;
            _totalEmployeeHoursService = totalEmployeeHourService;
            _employeePayrollItemService = employeePayrollItemService;
            _employeePayrollService = employeePayrollService;

            _frequency = (FrequencyType)Convert
                .ToInt32(_settingService.GetByKey(PAYROLL_FREQUENCY));
        }

        public IList<EmployeePayrollPerCompany> GeneratePayrollGrossPayByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            var employeePayrollItems = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            var employeePayrollList = new List<EmployeePayrollPerCompany>();

            if (employeePayrollItems != null && employeePayrollItems.Count() > 0)
            {
                //Hold last payroll processed
                EmployeePayrollPerCompany tempEmployeePayroll = null;
                DateTime today = DateTime.Now;
                EmployeePayrollItemPerCompany lastPayrollItem = employeePayrollItems.Last();

                foreach (EmployeePayrollItemPerCompany item in employeePayrollItems)
                {
                    //If should create new entry
                    if (tempEmployeePayroll == null || //First iteration
                        tempEmployeePayroll.Employee.EmployeeId != item.EmployeeId || //Diff employee
                            tempEmployeePayroll.CompanyId != item.CompanyId) //Dif company
                    {
                        if (tempEmployeePayroll != null)
                        {
                            //Save last employeePayroll entry if for different employee
                            _employeePayrollRepository.Add(tempEmployeePayroll);
                            employeePayrollList.Add(tempEmployeePayroll);
                        }
                        Employee employee = _employeeService.GetById(item.EmployeeId);

                        EmployeePayrollPerCompany employeePayroll = new EmployeePayrollPerCompany
                        {
                            Employee = employee,
                            CutOffStartDate = payrollStartDate,
                            CutOffEndDate = payrollEndDate,
                            PayrollGeneratedDate = today,
                            PayrollDate = payrollDate,
                            TotalGross = item.TotalAmount,
                            TotalNet = item.TotalAmount,
                            TaxableIncome = item.TotalAmount,
                            CompanyId = item.CompanyId
                        };

                        tempEmployeePayroll = employeePayroll;
                    }
                    else
                    {
                        //Update last entry
                        tempEmployeePayroll.TotalGross += item.TotalAmount;
                        tempEmployeePayroll.TotalNet += item.TotalAmount;
                        tempEmployeePayroll.TaxableIncome += item.TotalAmount;
                    }

                    //If last iteration save
                    if (item.Equals(lastPayrollItem))
                    {
                        _employeePayrollRepository.Add(tempEmployeePayroll);
                        employeePayrollList.Add(tempEmployeePayroll);
                    }
                }

                //Commit
                _unitOfWork.Commit();
            }

            MapItemsToPayroll(employeePayrollItems, employeePayrollList);

            return employeePayrollList;
          
        }

        public void MapItemsToPayroll(IList<EmployeePayrollItemPerCompany> items, IList<EmployeePayrollPerCompany> payrolls)
        {
            //Add mapping to employee payroll lists
            foreach (EmployeePayrollItemPerCompany item in items)
            {
                var employeePayroll = payrolls.Where(p => p.EmployeeId == item.EmployeeId && p.CompanyId == item.CompanyId).First();

                _employeePayrollItemService.Update(item);
                item.PayrollPerCompanyId = employeePayroll.EmployeePayrollPerCompanyId;
            }
            //Commit
            _unitOfWork.Commit();
        }

        public void Update(EmployeePayrollPerCompany employeePayroll)
        {
            _employeePayrollRepository.Update(employeePayroll);
        }
      
        public void GeneratePayroll(DateTime? date)
        {
            DateTime payrollStartDate = _employeePayrollService.GetNextPayrollStartDate(date);
            DateTime payrollEndDate = _employeePayrollService.GetNextPayrollEndDate(payrollStartDate);
            
            GeneratePayroll(payrollStartDate, payrollEndDate);
        }

        private void DeleteByDateRange(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //If there's existing payroll within date range, make it inactive
            var existingPayrolls = this.GetByDateRange(payrollStartDate, payrollEndDate);
            _employeePayrollRepository.DeleteAll(existingPayrolls);

            _unitOfWork.Commit();
        }

        public void GeneratePayroll(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Delete existing payrolls
            DeleteByDateRange(payrollStartDate, payrollEndDate);

            var payrollDate = _employeePayrollService.GetNextPayrollReleaseDate(payrollEndDate);
           
            //Generate employee payroll and net pay
            var employeePayrollList = 
                GeneratePayrollGrossPayByDateRange(payrollDate, payrollStartDate, payrollEndDate);

        }

        public IList<EmployeePayrollPerCompany> GetByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            dateEnd = dateEnd.AddDays(1);
            return _employeePayrollRepository.GetByDateRange(dateStart, dateEnd);
        }

        public virtual EmployeePayrollPerCompany GetById(int id)
        {
            return _employeePayrollRepository.GetById(id);
        }

        public virtual bool IsPayrollComputed(DateTime startDate, DateTime endDate)
        {
            return _employeePayrollRepository.Find(x => x.IsActive && x.CutOffStartDate == startDate && x.CutOffEndDate == endDate).Any();
        }

    }
}
