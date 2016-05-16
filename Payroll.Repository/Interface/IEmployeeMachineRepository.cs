using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeMachineRepository : IRepository<EmployeeMachine>
    {
        EmployeeMachine GetByEmployeeId(int employeeId, int machineId);
    }
}
