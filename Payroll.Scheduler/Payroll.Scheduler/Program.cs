using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using System;
using System.Configuration;
using Payroll.Scheduler.Interfaces;
using Payroll.Schedules.Scheduler;
using Payroll.Scheduler.Schedules;

namespace Payroll.Scheduler
{
    public class Program
    {
        static void Main(string[] args)
        {
            var scheduleType = ConfigurationManager.AppSettings["ScheduleType"].ToString();

            Console.WriteLine("Initializing");
           
            ISchedule scheduler;
            switch(scheduleType)
            {
                case ScheduleTypes.ATTENDANCE:
                    scheduler = new AttendanceSchedule();
                    break;
                case ScheduleTypes.HOLIDAY:
                    scheduler = new HolidaySchedule();
                    break;
                case ScheduleTypes.EMPLOYEE_HOURS:
                    scheduler = new EmployeeHoursSchedule();
                    break;
                default:
                    scheduler = null;
                    break;
            }

            scheduler.Execute();
        }
    }
}
