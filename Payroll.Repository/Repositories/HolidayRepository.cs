using System;
using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using System.Linq;

namespace Payroll.Repository.Repositories
{
    public class HolidayRepository : Repository<Holiday>, IHolidayRepository
    {
        public HolidayRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Holidays;
        }

        public virtual IEnumerable<Holiday> GetHolidaysByCurrentYear()
        {
            var year = DateTime.Now.Year;
            return Find(x => x.Year == year && x.IsActive);
        }

        public Holiday GetHoliday(DateTime date)
        {
            return Find(h => h.Date == date && h.IsActive).FirstOrDefault();
        }
    }
}
