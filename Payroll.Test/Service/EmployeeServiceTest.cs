using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeServiceTest
    {
        [TestMethod]
        public void TestSearchEmployee()
        {
            var databaseFactory = new DatabaseFactory();

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var employeeService = new EmployeeService(employeeRepository);

            var result = employeeService.SearchEmployee("corne");
        }
    }
}
