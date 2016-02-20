using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Repository
{
    [TestClass]
    public class AttendanceRepositoryTest
    {
        private TestContext testContextInstance;

        public AttendanceRepositoryTest()
        {

        }

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
         
        [TestMethod]
        public void GetAttendanceByDateRange()
        {
            var databaseFactory = new DatabaseFactory();
            var attendanceRepository = new AttendanceRepository(databaseFactory);

            //Test Data
            var attendance1 = new Attendance()
            {
                AttendanceId = 1,
                EmployeeCode = "001",
                ClockIn = DateTime.Parse("2016-02-01 23:59:59"),
                ClockOut = DateTime.Parse("201-02-02 06:50:00"),
            };

        }
    }
}
