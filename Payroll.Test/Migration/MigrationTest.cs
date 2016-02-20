using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities.Contexts;
using Payroll.Entities.Migrations;

namespace Payroll.Test.Migration
{
    [TestClass]
    public class MigrationTest
    {
        [TestMethod]
        public void RunSeedersTest()
        {
            var context = new PayrollContext();
            var configuration = new Configuration();
            configuration.RunSeeds(context);
        }
    }
}
