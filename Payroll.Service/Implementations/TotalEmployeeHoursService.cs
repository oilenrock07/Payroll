using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class TotalEmployeeHoursService : ITotalEmployeeHoursService
    {
        private TotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private EmployeeHoursService _employeeHoursService;
        private UnitOfWork _unitOfWork;

        public TotalEmployeeHoursService(UnitOfWork unitOfWork, 
            TotalEmployeeHoursRepository totalEmployeeHoursRepository,
            EmployeeHoursService employeeHoursService)
        {
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursRepository = totalEmployeeHoursRepository;
            _employeeHoursService = employeeHoursService;
        }

        public void GenerateTotalByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            var employeeHoursList = _employeeHoursService.GetForProcessingByDateRange(dateFrom, dateTo);

            if (employeeHoursList != null && employeeHoursList.Count > 1)
            {
                int tempEmployeeId = 0;
                DateTime? tempDate = null;
                RateType? tempRate = null;

                TotalEmployeeHours totalEmployeeHours = null;

                EmployeeHours last = employeeHoursList.Last();

                foreach (EmployeeHours hours in employeeHoursList)
                {
                    // If not the same date, type and employee as the last entry should create new total employee hours
                    if (tempEmployeeId != hours.EmployeeId ||
                            tempDate != hours.Date || tempRate != hours.Type)
                    {
                        //Save previous entry if any
                        if (totalEmployeeHours != null)
                        {
                            //Check if there's an existing total entry for the employee, rate type and date
                            var existingTotalEmployeeHours = _totalEmployeeHoursRepository
                                .GetByEmployeeDateAndType(totalEmployeeHours.EmployeeId,
                                    totalEmployeeHours.Date, totalEmployeeHours.Type);

                            //If yes update the value
                            if (existingTotalEmployeeHours != null)
                            {
                                _totalEmployeeHoursRepository.Update(existingTotalEmployeeHours);

                                existingTotalEmployeeHours.Hours += totalEmployeeHours.Hours;

                            }
                            else //Create new entry
                            {
                                _totalEmployeeHoursRepository.Add(totalEmployeeHours);
                            }
                        }

                        //Create new total employee hours obj
                        totalEmployeeHours = new TotalEmployeeHours
                        {
                            Date = hours.Date,
                            EmployeeId = hours.EmployeeId,
                            Type = hours.Type,
                            Hours = hours.Hours
                        };

                    }
                    else //Same Employee, Date and Rate, Update the total Employee hours data
                    {
                        totalEmployeeHours.Hours = totalEmployeeHours.Hours + hours.Hours;
                    }

                    //If Last Iteration and New entry  
                    //Save
                    if (hours.Equals(last) && (totalEmployeeHours.TotalEmployeeHoursId == null
                        || totalEmployeeHours.TotalEmployeeHoursId <= 0))
                    {
                        _totalEmployeeHoursRepository.Add(totalEmployeeHours);
                    }

                    //Set Reference data
                    tempDate = hours.Date;
                    tempEmployeeId = hours.EmployeeId;
                    tempRate = hours.Type;

                    //Update employee hours IsIncludedInTotal value
                    _employeeHoursService.Update(hours);

                    hours.IsIncludedInTotal = true;
                }

                //Commit all change
                _unitOfWork.Commit();
            }
         
        }

        public TotalEmployeeHours GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type)
        {
            return _totalEmployeeHoursRepository.GetByEmployeeDateAndType(employeeId, date, type);
        }

        public IList<TotalEmployeeHours> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _totalEmployeeHoursRepository.GetByDateRange(dateFrom, dateTo);
        }
    }
}
