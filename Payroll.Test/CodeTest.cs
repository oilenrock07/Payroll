using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Payroll.Test
{
    [TestClass]
    public class CodeTest
    {
        [TestMethod]
        public void GetGUIDTest()
        {
            var guid = Guid.NewGuid().GetHashCode();
        }
    }
}
