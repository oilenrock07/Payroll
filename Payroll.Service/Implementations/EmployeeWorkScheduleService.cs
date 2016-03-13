using Payroll.Service.Interfaces;
using Payroll.Entities;
using Payroll.Repository.Repositories;

namespace Payroll.Service.Implementations
{
    public class EmployeeWorkScheduleService : IEmployeeWorkScheduleService
    {
        private readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;

        public EmployeeWorkScheduleService(IEmployeeWorkScheduleRepository employeeWorkScheduleRepository)
        {
            _employeeWorkScheduleRepository = employeeWorkScheduleRepository;
        }

        public EmployeeWorkSchedule GetByEmployeeId(int employeeId)
        {
            return _employeeWorkScheduleRepository.GetByEmployeeId(employeeId);
        }
    }
}
