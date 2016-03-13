using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HolidayService(IHolidayRepository holidayRepository, ISettingRepository settingRepository, IUnitOfWork unitOfWork)
        {
            _holidayRepository = holidayRepository;
            _settingRepository = settingRepository;
            _unitOfWork = unitOfWork;
        }

        public void CreateNewHolidays()
        {
            var currentYear = 2017;//DateTime.Now.Year;
            var payrollCurrentYear = _settingRepository.Find(x => x.SettingKey == "HOLIDAY_CURRENT_YEAR").First();
            var staticHolidays = GetStaticHolidays(currentYear);
            if (currentYear > Convert.ToInt32(payrollCurrentYear.Value))
            {
                foreach (var holiday in staticHolidays)
                {
                    _holidayRepository.Add(holiday);
                }

                _settingRepository.Update(payrollCurrentYear);
                payrollCurrentYear.Value = currentYear.ToString();
                _unitOfWork.Commit();
            }

        }

        //http://www.timeanddate.com/holidays/philippines/good-friday
        public Dictionary<int, DateTime> GoodFridays
        {
            get
            {
                var goodFridays = new Dictionary<int, DateTime>();
                goodFridays.Add(2016, new DateTime(2016,3,25));
                goodFridays.Add(2017, new DateTime(2017, 4, 14));
                goodFridays.Add(2018, new DateTime(2018, 3, 30));
                goodFridays.Add(2019, new DateTime(2019, 4, 19));
                goodFridays.Add(2020, new DateTime(2020, 4, 10));

                goodFridays.Add(2021, new DateTime(2021, 4, 2));
                goodFridays.Add(2022, new DateTime(2022, 4, 15));
                goodFridays.Add(2023, new DateTime(2023, 4, 7));
                goodFridays.Add(2024, new DateTime(2024, 3, 29));
                goodFridays.Add(2025, new DateTime(2025, 4, 18));
                goodFridays.Add(2026, new DateTime(2026, 4, 3));

                goodFridays.Add(2027, new DateTime(2027, 3, 26));
                goodFridays.Add(2028, new DateTime(2028, 4, 14));
                goodFridays.Add(2029, new DateTime(2029, 3, 30));
                goodFridays.Add(2030, new DateTime(2030, 4, 19));
                goodFridays.Add(2031, new DateTime(2031, 4, 11));
                goodFridays.Add(2032, new DateTime(2032, 3, 26));
                return goodFridays;
            }
        }

        private IEnumerable<Holiday> GetStaticHolidays(int currentYear)
        {
            var goodFriday = GoodFridays.First(x => x.Value.Year == currentYear);

            var staticHolidays = new List<Holiday>()
            {
                new Holiday
                {
                    Date = new DateTime(currentYear, 1, 1),
                    IsActive = true,
                    Description = "Regular Holiday",
                    HolidayName = "New Year's Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 1, 2),
                    IsActive = true,
                    Description = "Special Non-working Holiday",
                    HolidayName = "Special non-working day after New Year",
                    IsRegularHoliday = false,
                    Year = currentYear
                },

                //todo: find a way to know the first lunar day
                //new Holiday
                //{
                //    Date = new DateTime(currentYear, 2, 8),
                //    IsActive = true,
                //    Description = "Special Non-working Holiday",
                //    HolidayName = "Chinese Lunar New Year's Day",
                //    IsRegularHoliday = false,
                //    Year = currentYear
                //},
                new Holiday
                {
                    Date =  new DateTime(currentYear, 2, 25),
                    IsActive = true,
                    Description = "Special Non-working Holiday",
                    HolidayName = "People Power Anniversary",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = goodFriday.Value.AddDays(-1),
                    IsActive = true,
                    Description = "Regular Holiday",
                    HolidayName = "Maundy Thursday",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = goodFriday.Value,
                    IsActive = true,
                    Description = "Regular Holiday",
                    HolidayName = "Good Friday",
                    IsRegularHoliday = true,
                    Year = currentYear
                },

                new Holiday
                {
                    Date = new DateTime(currentYear, 4, 9),
                    IsActive = true,
                    Description = "Regular Holiday",
                    HolidayName = "The Day of Valor",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 5, 1),
                    IsActive = true,
                    Description = "Regular Holiday",
                    HolidayName = "Labor Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 7, 8),
                    IsActive = true,
                    Description = "The Feast of Ramadan. Date to be confirmed.",
                    HolidayName = "Eid'l Fitr",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 8, 21),
                    IsActive = true,
                    Description =
                        "Special Non-working day. Commemorates the assassination of Benigno Ninoy Aquino Jr. in 1983",
                    HolidayName = "Ninoy Aquino Day",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 8, 29),
                    IsActive = true,
                    Description = "Regular holiday. Last Monday of August",
                    HolidayName = "National Heroes Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 11, 1),
                    IsActive = true,
                    Description = "Special Non-working Holiday",
                    HolidayName = "All Saints' Day",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 11, 30),
                    IsActive = true,
                    Description = "Regular holiday. Last Monday of August",
                    HolidayName = "Bonifacio Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 12, 24),
                    IsActive = true,
                    Description = "Christmas Eve",
                    HolidayName = "Christmas Eve",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 12, 25),
                    IsActive = true,
                    Description = "Christmas Day",
                    HolidayName = "Christmas Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 12, 30),
                    IsActive = true,
                    Description = "Christmas Day",
                    HolidayName = "	Rizal Day",
                    IsRegularHoliday = true,
                    Year = currentYear
                },
                new Holiday
                {
                    Date = new DateTime(currentYear, 12, 31),
                    IsActive = true,
                    Description = "New Year's Eve",
                    HolidayName = "New Year's Eve",
                    IsRegularHoliday = false,
                    Year = currentYear
                },
            };

            return staticHolidays;
        }
    }
}
