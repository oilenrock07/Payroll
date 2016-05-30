using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using Payroll.Common.Extension;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Schedules.Scheduler
{
    public class AttendanceSchedule : ISchedule
    {
        public readonly PayrollContext _payrollContext;
        public readonly IDatabaseFactory _databaseFactory;
        public readonly IUnitOfWork _unitOfWork;

        public readonly IAttendanceRepository _attendanceRepository;
        public readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;
        public readonly IEmployeeRepository _employeeRepository;
        public readonly IAttendanceLogRepository _attendanceLogRepository;

        public readonly IAttendanceLogService _attendanceLogService;
        public readonly IAttendanceService _attendanceService;

        public AttendanceSchedule()
        {
            _payrollContext = new PayrollContext();
            _databaseFactory = new DatabaseFactory(_payrollContext);
            _unitOfWork = new UnitOfWork(_databaseFactory);

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
            Console.WriteLine("Generating Attendance...");
            _attendanceService.CreateWorkSchedules();
        }
    }
}
