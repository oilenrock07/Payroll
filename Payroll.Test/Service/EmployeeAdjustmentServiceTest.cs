using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeAdjustmentServiceTest
    {
        private readonly IEmployeeAdjustmentService _employeeAdjustmentService;

        public EmployeeAdjustmentServiceTest()
        {
            var databaseFactory = new DatabaseFactory();
    
            var employeeAdjustmentRepository = new EmployeeAdjustmentRepository(databaseFactory);
            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepostory = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            _employeeAdjustmentService = new EmployeeAdjustmentService(employeeAdjustmentRepository, employeeRepostory);
        }

        [TestMethod]
        public void GetAllEmployeesAndAdjustmentsTest()
        {
            var result = _employeeAdjustmentService.GetEmployeeAdjustmentByDate(new DateTime(2016, 7, 1), new DateTime(2016, 7,5)).ToList();
        }
    }
}
