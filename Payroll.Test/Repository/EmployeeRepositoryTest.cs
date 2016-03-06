using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Entities;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using Assert = NUnit.Framework.Assert;

namespace Payroll.Test.Repository
{
    /// <summary>
    /// Summary description for EmployeeRepositoryTest
    /// </summary>
    [TestClass]
    public class EmployeeRepositoryTest
    {
        public EmployeeRepositoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void AddToDepartmentTest()
        {
            //Arrange
            var data = new List<Employee>
            {
                new Employee() { EmployeeId = 1, FirstName = "Cawi", BirthDate = new DateTime(1989, 10, 30)},
            }.AsQueryable();

            var departments = new List<Department>
            {
                new Department() { DepartmentId = 1, DepartmentName = "Test Department 1"},
                new Department() { DepartmentId = 2, DepartmentName = "Test Department 2"},
                new Department() { DepartmentId = 3, DepartmentName = "Test Department 3"},
                new Department() { DepartmentId = 4, DepartmentName = "Test Department 4"},

            }.AsQueryable();

            var employeeDepartments = new List<EmployeeDepartment>
            {
                new EmployeeDepartment() { IsActive = true, DepartmentId = 1, EmployeeId = 1, EmployeeDepartmentId = 1},
            }.AsQueryable();



            var dbSetEmployeesMock = new Mock<IDbSet<Employee>>();
            dbSetEmployeesMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetEmployeesMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetEmployeesMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetEmployeesMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var dbSetDepartmentMock = new Mock<IDbSet<Department>>();
            dbSetDepartmentMock.Setup(m => m.Provider).Returns(departments.Provider);
            dbSetDepartmentMock.Setup(m => m.Expression).Returns(departments.Expression);
            dbSetDepartmentMock.Setup(m => m.ElementType).Returns(departments.ElementType);
            dbSetDepartmentMock.Setup(m => m.GetEnumerator()).Returns(departments.GetEnumerator());

            var dbSetEmployeeDepartmentMock = new Mock<IDbSet<EmployeeDepartment>>();
            dbSetEmployeeDepartmentMock.Setup(m => m.Provider).Returns(employeeDepartments.Provider);
            dbSetEmployeeDepartmentMock.Setup(m => m.Expression).Returns(employeeDepartments.Expression);
            dbSetEmployeeDepartmentMock.Setup(m => m.ElementType).Returns(employeeDepartments.ElementType);
            dbSetEmployeeDepartmentMock.Setup(m => m.GetEnumerator()).Returns(employeeDepartments.GetEnumerator());


            var context = new Mock<PayrollContext>();

            context.Setup(x => x.Employees).Returns(dbSetEmployeesMock.Object);
            context.Setup(x => x.Departments).Returns(dbSetDepartmentMock.Object);
            context.Setup(x => x.EmployeeDepartments).Returns(dbSetEmployeeDepartmentMock.Object);

            
            var databaseFactory = new DatabaseFactory(context.Object);
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);

            //Act
            var newDepartments = new[] {2, 3}; 
            employeeRepository.UpdateDepartment(newDepartments, 1);
            unitOfWork.Commit();

            //Asset
            Assert.AreEqual(employeeRepository.GetDepartments(1).Count(), 2);
        }

        [TestMethod]
        public void UnitOfWorkCommitTest()
        {
            var data = new List<Employee>
            {
                new Employee() { EmployeeId = 1, FirstName = "Cawi", BirthDate = new DateTime(1989, 10, 30)},
            }.AsQueryable();

            var dbSetEmployeesMock = new Mock<IDbSet<Employee>>();
            dbSetEmployeesMock.Setup(m => m.Provider).Returns(data.Provider);
            dbSetEmployeesMock.Setup(m => m.Expression).Returns(data.Expression);
            dbSetEmployeesMock.Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetEmployeesMock.Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new Mock<PayrollContext>();
            context.Setup(x => x.Employees).Returns(dbSetEmployeesMock.Object);

            var databaseFactory = new DatabaseFactory(context.Object);
            var unitOfWork = new UnitOfWork(databaseFactory);

            var employeeDepartmentRepository = new EmployeeDepartmentRepository(databaseFactory);
            var employeeRepository = new EmployeeRepository(databaseFactory, employeeDepartmentRepository);

            employeeRepository.Add(new Employee() {BirthDate = DateTime.Now, FirstName = "New"});
            unitOfWork.Commit();

            dbSetEmployeesMock.Verify(x => x.Add(It.IsAny<Employee>()));
            context.Verify(x => x.SaveChanges());

            var count = employeeRepository.GetAll().Count();

            Assert.AreEqual(count,2);
        }
    }
}
