using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            
        }

        public Employee GetByCode(string code)
        {
            return Find(e => e.EmployeeCode == code).FirstOrDefault();
        }
    }
}
