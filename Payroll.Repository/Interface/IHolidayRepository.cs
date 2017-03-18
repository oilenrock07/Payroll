using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;

namespace Payroll.Repository.Interface
{
    public interface IHolidayRepository : IRepository<Holiday>
    {
        IEnumerable<Holiday> GetHolidaysByCurrentYear();

        Holiday GetHoliday(DateTime date);

        bool IsHolidayExists(DateTime date);

        bool IsHolidayExists(DateTime date, int holidayId);
    }
}
