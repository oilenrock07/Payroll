using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Interface
{
    public interface IEmployeePayrollDeductionRepository : IRepository<EmployeePayrollDeduction>
    {
    }
}
