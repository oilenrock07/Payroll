using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service;
using Payroll.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeDailyPayrollServiceTest
    {
        public void DeleteInfo(EmployeeRepository repository, UnitOfWork unitOfwork)
        {
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 0");
            repository.ExecuteSqlCommand("TRUNCATE TABLE frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE payment_frequency");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance");
            repository.ExecuteSqlCommand("TRUNCATE TABLE attendance_log");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_info");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_workschedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_hours_total"); 
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_daily_payroll");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee_salary");
            repository.ExecuteSqlCommand("TRUNCATE TABLE work_schedule");
            repository.ExecuteSqlCommand("TRUNCATE TABLE employee");
            repository.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS = 1");
        }

        //Test Regular Rate 
        [TestMethod]
        public void GenerateEmployeeDailySalaryByDateRange()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            TotalEmployeeHoursRepository _totalEmployeeHoursRepository = 
                new TotalEmployeeHoursRepository(databaseFactory);
            EmployeeHoursRepository _employeeHoursRepository = 
                new EmployeeHoursRepository(databaseFactory);
            EmployeeWorkScheduleRepository _employeeWorkScheduleRepository = 
                new EmployeeWorkScheduleRepository(databaseFactory);
            HolidayRepository _holidayRepository = 
                new HolidayRepository(databaseFactory);
            SettingRepository _settingsRepository = 
                new SettingRepository(databaseFactory);
            EmployeeDepartmentRepository _employeeDepartmentRepository = 
                new EmployeeDepartmentRepository(databaseFactory);
            EmployeeRepository _employeeRepository = 
                new EmployeeRepository(databaseFactory, _employeeDepartmentRepository);
            AttendanceLogRepository _attendanceLogRepository = 
                new AttendanceLogRepository(databaseFactory, _employeeRepository);
            AttendanceRepository _attendanceRepository = 
                new AttendanceRepository(databaseFactory);
            EmployeeInfoRepository _employeeInfoRepository = 
                new EmployeeInfoRepository(databaseFactory);
            EmployeeDailyPayrollRepository _employeeDailyPayrollRepository = 
                new EmployeeDailyPayrollRepository(databaseFactory);
            EmployeeSalaryRepository _employeeSalaryRepository = 
                new EmployeeSalaryRepository(databaseFactory);

            EmployeeWorkScheduleService _employeeWorkScheduleService = 
                new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            SettingService _settingService = 
                new SettingService(_settingsRepository);
            HolidayService _holidayService = 
                new HolidayService(_holidayRepository, _settingsRepository, unitOfWork);
            AttendanceLogService _attendanceLogService = 
                new AttendanceLogService(_attendanceLogRepository);
            AttendanceService _attendanceService = 
                new AttendanceService(unitOfWork, _attendanceRepository, _attendanceLogService);
            EmployeeInfoService _employeeInfoService = 
                new EmployeeInfoService(_employeeInfoRepository);
            EmployeeHoursService _employeeHoursService = 
                new EmployeeHoursService(unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            TotalEmployeeHoursService _totalEmployeeHoursService =
                new TotalEmployeeHoursService(unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService);
            EmployeeSalaryService _employeeSalaryService = new EmployeeSalaryService();
            EmployeeDailyPayrollService employeeDailyPayrollService = 
                new EmployeeDailyPayrollService(unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService, _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);

            DeleteInfo(_employeeRepository, unitOfWork);

            //Data
            var employeeId1 = 1;
            var employeeId2 = 2;

            var employee1 = new Employee
            {
                EmployeeCode = "11001",
                FirstName = "Jona",
                LastName = "Pereira",
                MiddleName = "Aprecio",
                BirthDate = DateTime.Parse("02/02/1991"),
                Gender = 1,
                IsActive = true
            };

            var employee2 = new Employee
            {
                EmployeeCode = "11002",
                FirstName = "Cornelio",
                LastName = "Cawicaan",
                MiddleName = "Bue",
                BirthDate = DateTime.Parse("04/22/1989"),
                Gender = 1,
                IsActive = true
            };

            var EmployeeSalary1 = new EmployeeSalary
            {
                EmployeeInfoId = employeeId1,
                Salary = 100,
                SalaryFrequency = Entities.Enums.SalaryFrequency.Hourly
            };

            var EmployeeSalary2 = new EmployeeSalary
            {
                EmployeeInfoId = employeeId1,
                Salary = 3000,
                SalaryFrequency = Entities.Enums.SalaryFrequency.Weekly
            };

            _employeeSalaryRepository.Add(EmployeeSalary1);
            _employeeSalaryRepository.Add(EmployeeSalary2);

            var workSchedule = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule,
                Employee = employee1
            };

            var workSchedule2 = new WorkSchedule
            {
                TimeStart = new TimeSpan(0, 7, 0, 0),
                TimeEnd = new TimeSpan(0, 16, 0, 0),
                WeekStart = 1,
                WeekEnd = 5
            };

            var employeeWorkSchedule2 = new EmployeeWorkSchedule
            {
                WorkSchedule = workSchedule2,
                Employee = employee2
            };

            _employeeWorkScheduleRepository.Add(employeeWorkSchedule);
            _employeeWorkScheduleRepository.Add(employeeWorkSchedule2);

            var employeeInfo1 = new EmployeeInfo
            {
                Employee = employee1,
                EmployeeSalary = EmployeeSalary1
            };

            var employeeInfo2 = new EmployeeInfo
            {
                Employee = employee2,
                EmployeeSalary = EmployeeSalary2
            };

            _employeeInfoRepository.Add(employeeInfo1);
            _employeeInfoRepository.Add(employeeInfo2);

            var totalEmployeeHoursRegular1 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 8,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular5 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/21/2016")
            };

            var totalEmployeeHoursRegular2 = new TotalEmployeeHours
            {
                EmployeeId = employeeId1,
                Hours = 10.5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/22/2016")
            };

            var totalEmployeeHoursRegular3 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 0.5,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/23/2016")
            };

            var totalEmployeeHoursRegular4 = new TotalEmployeeHours
            {
                EmployeeId = employeeId2,
                Hours = 4.1,
                Type = Entities.Enums.RateType.Regular,
                Date = DateTime.Parse("04/24/2016")
            };

            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular1);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular2);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular3);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular4);
            _totalEmployeeHoursRepository.Add(totalEmployeeHoursRegular5);

            unitOfWork.Commit();

            //Test
            DateTime dateFrom = DateTime.Parse("04/21/2016");
            DateTime dateTo = DateTime.Parse("04/23/2016");

            employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(dateFrom, dateTo);

            var employeeDailyPayroll = employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);

            Assert.IsNotNull(employeeDailyPayroll);
            Assert.AreEqual(employeeDailyPayroll.Count, 4);

            var employeeDailyPayroll1 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/23/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 3,
                TotalPay = (decimal)48.75
            };

            var employeeDailyPayroll2 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/22/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 2,
                TotalPay = (decimal)1050
            };

            var employeeDailyPayroll3 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 1,
                TotalEmployeeHoursId = 1,
                TotalPay = (decimal)800
            };

            var employeeDailyPayroll4 = new EmployeeDailyPayroll
            {
                Date = DateTime.Parse("04/21/2016"),
                EmployeeId = 2,
                TotalEmployeeHoursId = 5,
                TotalPay = (decimal)375
            };

            Assert.AreEqual(employeeDailyPayroll1.Date, employeeDailyPayroll[0].Date);
            Assert.AreEqual(employeeDailyPayroll1.EmployeeId, employeeDailyPayroll[0].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll1.TotalEmployeeHoursId, employeeDailyPayroll[0].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll1.TotalPay, employeeDailyPayroll[0].TotalPay);

            Assert.AreEqual(employeeDailyPayroll2.Date, employeeDailyPayroll[1].Date);
            Assert.AreEqual(employeeDailyPayroll2.EmployeeId, employeeDailyPayroll[1].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll2.TotalEmployeeHoursId, employeeDailyPayroll[1].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll2.TotalPay, employeeDailyPayroll[1].TotalPay);

            Assert.AreEqual(employeeDailyPayroll3.Date, employeeDailyPayroll[2].Date);
            Assert.AreEqual(employeeDailyPayroll3.EmployeeId, employeeDailyPayroll[2].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll3.TotalEmployeeHoursId, employeeDailyPayroll[2].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll3.TotalPay, employeeDailyPayroll[2].TotalPay);

            Assert.AreEqual(employeeDailyPayroll4.Date, employeeDailyPayroll[3].Date);
            Assert.AreEqual(employeeDailyPayroll4.EmployeeId, employeeDailyPayroll[3].EmployeeId);
            Assert.AreEqual(employeeDailyPayroll4.TotalEmployeeHoursId, employeeDailyPayroll[3].TotalEmployeeHoursId);
            Assert.AreEqual(employeeDailyPayroll4.TotalPay, employeeDailyPayroll[3].TotalPay);

        }
    }
}
