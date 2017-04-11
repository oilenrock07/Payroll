using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;

namespace Payroll.Test.Repository
{
    [TestClass]
    public class LogRepositoryTest
    {
        [TestMethod]
        public void TestCount()
        {
            var logRepository = new LogRepository(new DatabaseFactory());
            var all = logRepository.GetAll();

            var count = logRepository.Count(all);
        }
    }
}
