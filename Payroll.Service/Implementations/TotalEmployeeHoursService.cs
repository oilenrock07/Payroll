using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Repository.Models;

namespace Payroll.Service.Implementations
{
    public class TotalEmployeeHoursService : ITotalEmployeeHoursService
    {
        private ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private IEmployeeHoursService _employeeHoursService;
        private IUnitOfWork _unitOfWork;
        private ISettingService _settingService;
       
        public TotalEmployeeHoursService(IUnitOfWork unitOfWork, 
            ITotalEmployeeHoursRepository totalEmployeeHoursRepository,
            IEmployeeHoursService employeeHoursService,
            ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _totalEmployeeHoursRepository = totalEmployeeHoursRepository;
            _employeeHoursService = employeeHoursService;
            _settingService = settingService;
        }

        public void GenerateTotalByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            //Delete existing data within date range
            DeleteByDateRange(dateFrom, dateTo);

            var employeeHoursList = _employeeHoursService
                .GetForProcessingByDateRange(true, dateFrom, dateTo);

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

                                existingTotalEmployeeHours.Hours = (existingTotalEmployeeHours.Hours + totalEmployeeHours.Hours);

                            }
                            else //Create new entry
                            {
                                totalEmployeeHours.Hours = (totalEmployeeHours.Hours);

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
                        totalEmployeeHours.Hours = (totalEmployeeHours.Hours);

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

        /*public double ComputeTotalAllowedHours(double TotalHours)
        {
            double total = TotalHours;
            //Total employee hours minimum butal is 5 mins
            //Get minimum OT minutes value
            double minimumOTInMinutes = (Convert.ToDouble
                (_settingService.GetByKey(SCHEDULE_MINIMUM_OT_MINUTES)) / (double)60);
            double totalMinutes = total - Math.Truncate(total);
            if (Math.Round(minimumOTInMinutes, 3) > Math.Round(totalMinutes, 3))
            {
                //Set total hours to floor
                total = Math.Floor(total);
            }

            return total;
        }*/

        public TotalEmployeeHours GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type)
        {
            return _totalEmployeeHoursRepository.GetByEmployeeDateAndType(employeeId, date, type);
        }

        public IList<TotalEmployeeHours> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = dateTo.AddDays(1);
            return _totalEmployeeHoursRepository.GetByDateRange(dateFrom, dateTo);
        }

        public IList<TotalEmployeeHours> GetByEmployeeDate(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursRepository.GetByEmployeeDate(employeeId, date);
        }

        public IList<TotalEmployeeHours> GetByTypeAndDateRange(int employeeId, RateType? rateType, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            payrollEndDate = payrollEndDate.AddDays(1);
            if (rateType == null)
            {
                return _totalEmployeeHoursRepository.GetByDateRange(employeeId, payrollStartDate, payrollEndDate);
            }
            return _totalEmployeeHoursRepository.GetByTypeAndDateRange(employeeId, rateType.Value, payrollStartDate, payrollEndDate);
        }

        //Delete existing total employee hours within date range
        private void DeleteByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            var existingTotalEmployeeHoursList = this.GetByDateRange(dateFrom, dateTo);
            _totalEmployeeHoursRepository.DeleteAll(existingTotalEmployeeHoursList);

            _unitOfWork.Commit();
        }

        public TotalEmployeeHours GetById(int id)
        {
            return _totalEmployeeHoursRepository.GetById(id);
        }

        public double CountTotalHours(int employeeId, DateTime date)
        {
            return _totalEmployeeHoursRepository.CountTotalHours(employeeId, date);
        }

        public virtual IEnumerable<HoursPerCompanyDao> GetEmployeeHoursTotal(DateTime startDate, DateTime endDate, int employeeId)
        {
            IEnumerable<TotalEmployeeHours> totalHours;
            if (employeeId > 0)
                totalHours = _totalEmployeeHoursRepository.GetByDateRange(startDate, endDate, employeeId);
            else
                totalHours = _totalEmployeeHoursRepository.GetByDateRange(startDate, endDate);

            var query = totalHours.GroupBy(x => new {
                    x.Date,
                    x.EmployeeId,
                    x.Employee.FirstName,
                    x.Employee.LastName,
                    x.Employee.MiddleName
                }).ToList();

            var hourPerCompany = query.Select(item => new HoursPerCompanyDao
            {
                Date = item.Key.Date,
                EmployeeId = item.Key.EmployeeId,
                FirstName = item.Key.FirstName,
                LastName = item.Key.LastName,
                MiddleName = item.Key.MiddleName,
                TotalEmployeeHours = item
            });

            return hourPerCompany;
        }
    }
}
