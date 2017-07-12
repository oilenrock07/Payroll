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

        public TotalEmployeeHoursPerCompanyService(ITotalEmployeeHoursPerCompanyRepository totalEmployeeHoursPerCompanyRepository)
        {
            _totalEmployeeHoursPerCompanyRepository = totalEmployeeHoursPerCompanyRepository;
        }

        public IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _totalEmployeeHoursPerCompanyRepository.GetByDateRange(dateFrom, dateTo);
        }

        public IList<TotalEmployeeHoursPerCompany> GetByEmployeeDate(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursPerCompanyRepository.GetByEmployeeDate(employeeId, date);
        }

        public double CountTotalHours(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursPerCompanyRepository.CountTotalHours(employeeId, date);
        }

    }
}
