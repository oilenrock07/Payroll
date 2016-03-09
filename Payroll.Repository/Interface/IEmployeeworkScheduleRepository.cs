using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeworkScheduleRepository : IRepository<EmployeeWorkSchedule>
    {
        EmployeeWorkSchedule GetActiveByEmployeeId(int employeeId);
    }
}
