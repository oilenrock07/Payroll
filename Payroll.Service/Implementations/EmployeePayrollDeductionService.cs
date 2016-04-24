using Payroll.Entities.Payroll;
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

        public void GenerateDeductionsByPayroll(EmployeePayroll employeePayroll)
        {
            //Compute SSS contribution
            throw new NotImplementedException();
        }

    }
}
