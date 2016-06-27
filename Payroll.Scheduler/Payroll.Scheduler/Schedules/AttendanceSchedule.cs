using Payroll.Entities.Enums;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Scheduler.Schedules;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;

namespace Payroll.Schedules.Scheduler
{
    public class AttendanceSchedule : BaseSchedule, ISchedule
    {
        public readonly IAttendanceRepository _attendanceRepository;
        public readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;
        public readonly IEmployeeRepository _employeeRepository;
        public readonly IAttendanceLogRepository _attendanceLogRepository;
        public readonly IAttendanceLogService _attendanceLogService;
        public readonly IAttendanceService _attendanceService;

        public AttendanceSchedule()
        {
            _attendanceRepository = new AttendanceRepository(_databaseFactory);
            _employeeDepartmentRepository = new EmployeeDepartmentRepository(_databaseFactory);
            _employeeRepository = new EmployeeRepository(_databaseFactory, _employeeDepartmentRepository);
            _attendanceLogRepository = new AttendanceLogRepository(_databaseFactory, _employeeRepository);

            _attendanceLogService = new AttendanceLogService(_attendanceLogRepository);
            _attendanceService = new AttendanceService(_unitOfWork, _attendanceRepository, _attendanceLogService);
            
        }

        public void Execute()
        {
            //Generate Attendance 
            try
            {
                Console.WriteLine("Generating Attendance...");
                _attendanceService.CreateWorkSchedules();

                LogSchedule(SchedulerLogType.Success);
            }
            catch (Exception ex)
            {
                LogSchedule(SchedulerLogType.Exception, ex.InnerException.Message);
            }
        }


    }
}
