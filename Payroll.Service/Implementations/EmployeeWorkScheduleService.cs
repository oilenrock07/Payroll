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

        public EmployeeWorkSchedule Add(int workScheduleId, int employeeId)
        {
            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                EmployeeId = employeeId,
                WorkScheduleId = workScheduleId
            };

            return _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
        }

        public void Update(int workScheduleId, int employeeId)
        {
            var workSchedule = _employeeWorkScheduleRepository.GetByEmployeeId(employeeId);
            if (workSchedule != null && workSchedule.WorkScheduleId != workScheduleId)
            {
                _employeeWorkScheduleRepository.Update(workSchedule);
                workSchedule.IsActive = false;

                Add(workScheduleId, employeeId);
            }

            if (workSchedule == null && workScheduleId > 0)
                Add(workScheduleId, employeeId);
        }
    }
}
