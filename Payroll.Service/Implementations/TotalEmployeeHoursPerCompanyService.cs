using Payroll.Entities.Payroll;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Enums;

namespace Payroll.Service.Implementations
{
    public class TotalEmployeeHoursPerCompanyService : ITotalEmployeeHoursPerCompanyService
    {
        private ITotalEmployeeHoursPerCompanyRepository _totalEmployeeHoursPerCompanyRepository;

        public IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _totalEmployeeHoursPerCompanyRepository.GetByDateRange(dateFrom, dateTo);
        }

        public IList<TotalEmployeeHoursPerCompany> GetByEmployeeDate(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursPerCompanyRepository.GetByEmployeeDate(employeeId, date);
        }

        public IList<TotalEmployeeHoursPerCompany> GetByTypeAndDateRange(int employeeId, RateType? rateType, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            payrollEndDate = payrollEndDate.AddDays(1);
            if (rateType == null)
            {
                return _totalEmployeeHoursPerCompanyRepository.GetByDateRange(employeeId, payrollStartDate, payrollEndDate);
            }
            return _totalEmployeeHoursPerCompanyRepository.GetByTypeAndDateRange(employeeId, rateType.Value, payrollStartDate, payrollEndDate);
        }

        public double CountTotalHours(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursPerCompanyRepository.CountTotalHours(employeeId, date);
        }
    }
}
