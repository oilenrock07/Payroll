using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Scheduler
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing");
            var payrollContext = new PayrollContext();
            var databaseFactory = new DatabaseFactory(payrollContext);
            var unitOfWork = new UnitOfWork(databaseFactory);

            var empoyeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(databaseFactory);
            var employeeWorkSchedule = new EmployeeWorkScheduleService(empoyeeWorkScheduleRepository);

            var settingRepository = new SettingRepository(databaseFactory);
            var holidayRepository = new HolidayRepository(databaseFactory);
            var holidayService = new HolidayService(holidayRepository, settingRepository, unitOfWork);

            Console.WriteLine("Checking holidays");
            holidayService.CreateNewHolidays();
        }
    }
}
