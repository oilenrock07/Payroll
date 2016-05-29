using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Scheduler.Schedules
{
    public class HolidaySchedule : ISchedule
    {
        public readonly PayrollContext _payrollContext;
        public readonly IDatabaseFactory _databaseFactory;
        public readonly IUnitOfWork _unitOfWork;

        public readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        public readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;
        public readonly ISettingRepository _settingRepository;
        public readonly IHolidayRepository _holidayRepository;
        public readonly IHolidayService _holidayService;

        public HolidaySchedule()
        {
            _payrollContext = new PayrollContext();
            _databaseFactory = new DatabaseFactory(_payrollContext);
            _unitOfWork = new UnitOfWork(_databaseFactory);

            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(_databaseFactory);
            _employeeWorkScheduleService = new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            _settingRepository = new SettingRepository(_databaseFactory);
            _holidayRepository = new HolidayRepository(_databaseFactory);
            _holidayService = new HolidayService(_holidayRepository, _settingRepository, _unitOfWork);
        }

        public void Execute()
        {
            Console.WriteLine("Checking holidays");
            _holidayService.CreateNewHolidays();
        }
    }
}
