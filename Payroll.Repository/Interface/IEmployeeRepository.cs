using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Employee GetByCode(string code);
    }
}
