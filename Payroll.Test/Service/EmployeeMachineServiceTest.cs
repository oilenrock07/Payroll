using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;

namespace Payroll.Test.Service
{
    [TestClass]
    public class EmployeeMachineServiceTest
    {
        [TestMethod]
        public void GetAllEmployeesToTheMachine()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var employeeMachineRepository = new EmployeeMachineRepository(databaseFactory);
            var employeeMachineService = new EmployeeMachineService(employeeMachineRepository, employeeRepository);

            var employees = employeeMachineService.GetEmployees(4);
            Assert.IsNotNull(employees);
        }

        [TestMethod]
        public void GetAllNotRegisteredEmployeesToTheMachine()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);
            var employeeMachineRepository = new EmployeeMachineRepository(databaseFactory);
            var employeeMachineService = new EmployeeMachineService(employeeMachineRepository, employeeRepository);

            var employees = employeeMachineService.GetEmployeesNotRegistered(4);
            Assert.IsNotNull(employees);
        }
    }
}
