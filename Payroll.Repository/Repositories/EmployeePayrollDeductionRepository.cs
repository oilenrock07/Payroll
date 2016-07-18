using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Repositories
{
    public class EmployeePayrollDeductionRepository : Repository<EmployeePayrollDeduction>, IEmployeePayrollDeductionRepository
    {
        public EmployeePayrollDeductionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeePayrollDeductions;
        }

        public IList<EmployeePayrollDeduction> GetByPayroll(int payrollId)
        {
            return Find(d => d.IsActive && d.EmployeePayrollId == payrollId).ToList();
        }

        public virtual IEnumerable<EmployeePayrollDeduction> GetByPayroll(IEnumerable<int> payrollIds)
        {
            var deductions = from deduction in GetAllActive()
                             join payrollId in payrollIds on deduction.EmployeePayrollId equals payrollId
                             select deduction;

            return deductions;
        }
    }
}
