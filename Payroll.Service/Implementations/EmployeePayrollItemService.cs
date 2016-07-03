using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollItemService : BaseEntityService<EmployeePayrollItem>, 
        IEmployeePayrollItemService
    {
        private IEmployeePayrollItemRepository _employeePayrollItemRepository;

        public EmployeePayrollItemService(IEmployeePayrollItemRepository employeePayrollItemRepository) 
            : base(employeePayrollItemRepository)
        {
            _employeePayrollItemRepository = employeePayrollItemRepository;
        }
    }
}
