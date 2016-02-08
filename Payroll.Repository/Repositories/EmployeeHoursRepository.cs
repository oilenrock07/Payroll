using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Payroll;
using System.Linq.Expressions;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Repositories
{
    public class EmployeeHoursRepository : Repository<EmployeeHours>, IEmployeeHoursRepository
    {
        public EmployeeHoursRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {

        }

    }
}
