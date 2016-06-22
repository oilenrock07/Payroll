using System;
using System.Configuration;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;

namespace Payroll.Scheduler.Schedules
{
    public abstract class BaseSchedule
    {
        public readonly PayrollContext _payrollContext;
        public readonly IDatabaseFactory _databaseFactory;
        public readonly IUnitOfWork _unitOfWork;

        protected IDatabaseFactory DatabaseFactory;
        private readonly ISchedulerLogRepository _schedulerLogRepository;

        protected BaseSchedule()
        {
            _payrollContext = new PayrollContext();
            _databaseFactory = new DatabaseFactory(_payrollContext);
            _unitOfWork = new UnitOfWork(_databaseFactory);
            _schedulerLogRepository = new SchedulerLogRepository(_databaseFactory);
        }


        public void LogSchedule(SchedulerLogType type, string exception = "")
        {
            var schedulerLog = new SchedulerLog
            {
                LogType = type,
                ScheduleType = ConfigurationManager.AppSettings["ScheduleType"],
                Exception = exception
            };

            _schedulerLogRepository.Add(schedulerLog);
            _unitOfWork.Commit();
        }
    }
}
