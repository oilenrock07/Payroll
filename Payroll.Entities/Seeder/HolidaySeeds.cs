using System;
using System.Collections.Generic;

namespace Payroll.Entities.Seeder
{
    public class HolidaySeeds : ISeeders<Holiday>
    {
        public IEnumerable<Holiday> GetDefaultSeeds()
        {
            return new List<Holiday>
            {
                new Holiday { Date = new DateTime(2016, 1,1), IsActive = true, Description = "Regular Holiday", HolidayName = "New Year's Day", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 1,2), IsActive = true, Description = "Special Non-working Holiday", HolidayName = "Special non-working day after New Year", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 2,8), IsActive = true, Description = "Special Non-working Holiday", HolidayName = "Chinese Lunar New Year's Day", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 2,25), IsActive = true, Description = "Special Non-working Holiday", HolidayName = "People Power Anniversary", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 3,24), IsActive = true, Description = "Regular Holiday", HolidayName = "Maundy Thursday", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 3,25), IsActive = true, Description = "Regular Holiday", HolidayName = "Good Friday", IsRegularHoliday = true, Year = 2016},

                new Holiday { Date = new DateTime(2016, 4,9), IsActive = true, Description = "Regular Holiday", HolidayName = "The Day of Valor", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 5,1), IsActive = true, Description = "Regular Holiday", HolidayName = "Labor Day", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 7,8), IsActive = true, Description = "The Feast of Ramadan. Date to be confirmed.", HolidayName = "Eid'l Fitr", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 8,21), IsActive = true, Description = "Special Non-working day. Commemorates the assassination of Benigno Ninoy Aquino Jr. in 1983", HolidayName = "Ninoy Aquino Day", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 8,29), IsActive = true, Description = "Regular holiday. Last Monday of August", HolidayName = "National Heroes Day", IsRegularHoliday = true, Year = 2016},

                new Holiday { Date = new DateTime(2016, 11,1), IsActive = true, Description = "Special Non-working Holiday", HolidayName = "All Saints' Day", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 11,30), IsActive = true, Description = "Regular holiday. Last Monday of August", HolidayName = "Bonifacio Day", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 12,24), IsActive = true, Description = "Christmas Eve", HolidayName = "Christmas Eve", IsRegularHoliday = false, Year = 2016},
                new Holiday { Date = new DateTime(2016, 12,25), IsActive = true, Description = "Christmas Day", HolidayName = "Christmas Day", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 12,30), IsActive = true, Description = "Christmas Day", HolidayName = "	Rizal Day", IsRegularHoliday = true, Year = 2016},
                new Holiday { Date = new DateTime(2016, 12,31), IsActive = true, Description = "New Year's Eve", HolidayName = "New Year's Eve", IsRegularHoliday = false, Year = 2016},
            };
        }
    }
}
