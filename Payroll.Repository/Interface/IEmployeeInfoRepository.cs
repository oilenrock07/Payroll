using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeInfoRepository : IRepository<EmployeeInfo>
    {
        EmployeeInfo GetByEmployeeId(int employeeId);
    }
}
