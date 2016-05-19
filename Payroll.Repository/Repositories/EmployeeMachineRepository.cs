using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class EmployeeMachineRepository : Repository<EmployeeMachine>, IEmployeeMachineRepository
    {
        public EmployeeMachineRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeMachines;
        }

        public virtual EmployeeMachine GetByEmployeeId(int employeeId, int machineId)
        {
            return Find(x => x.EmployeeId == employeeId && x.MachineId == machineId).FirstOrDefault();
        }
    }
}
