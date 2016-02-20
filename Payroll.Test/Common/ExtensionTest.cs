using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Common.Extension;

namespace Payroll.Test.Common
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void DeserializeDate()
        {
            string datetime = "20160208112700";
            DateTime date = datetime.DeserializeDate();
        }

        [TestMethod]
        public void SerializeDate()
        {
            var serializedDate = DateTime.Now.Serialize();
        }
    }
}
