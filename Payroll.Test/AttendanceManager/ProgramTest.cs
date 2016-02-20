using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTEvents;

namespace Payroll.Test.AttendanceManager
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void LoadingErrorCodeTest()
        {
            var codes = Program.GetErrorCodes();
            Assert.IsTrue(codes.Count > 0);
        }
    }
}
