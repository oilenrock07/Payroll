using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IHolidayRepository : IRepository<Holiday>
    {
        IEnumerable<Holiday> GetHolidaysByCurrentYear();
    }
}
