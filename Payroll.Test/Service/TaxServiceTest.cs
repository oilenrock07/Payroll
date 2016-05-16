using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Test.Service
{
    [TestClass]
    public class TaxServiceTest
    {
        private ITaxService _taxService;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            ITaxRepository _taxRepository = new TaxRepository(databaseFactory);
            _taxService = new TaxService(_taxRepository);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel1() {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //No Tax
            decimal taxAmount1 = 4000;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual(0, tax1);

            //No Tax
            decimal taxAmount2 = 4166;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual(0, tax2);

            //No Tax
            decimal taxAmount3 = 4167;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual(0, tax3);

            //With Tax
            decimal taxAmount4 = 4300;
            var tax4 = _taxService.ComputeTax(frequency, 0, taxAmount4);
            Assert.AreEqual((decimal)6.6500, tax4);

            //With Tax
            decimal taxAmount5 = 4999.99M;
            var tax5 = _taxService.ComputeTax(frequency, 0, taxAmount5);
            Assert.AreEqual((decimal)41.6495, tax5);
        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel2()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 5000;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)41.67, tax1);

            //With Tax
            decimal taxAmount2 = 5001.50M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)41.82, tax2);

            //With Tax
            decimal taxAmount3 = 5500.25M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)91.695, tax3);

            //With Tax
            decimal taxAmount4 = 6500.75M;
            var tax4 = _taxService.ComputeTax(frequency, 0, taxAmount4);
            Assert.AreEqual((decimal)191.745, tax4);

            //With Tax
            decimal taxAmount5 = 6666.99M;
            var tax5 = _taxService.ComputeTax(frequency, 0, taxAmount5);
            Assert.AreEqual((decimal)208.369, tax5);
        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel3()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 6667;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)208.33, tax1);

            //With Tax
            decimal taxAmount2 = 6700.22M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)213.313, tax2);

            //With Tax
            decimal taxAmount3 = 8500.99M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)483.4285, tax3);

            //With Tax
            decimal taxAmount4 = 9225;
            var tax4 = _taxService.ComputeTax(frequency, 0, taxAmount4);
            Assert.AreEqual((decimal)592.03, tax4);

            //With Tax
            decimal taxAmount5 = 9999.99M;
            var tax5 = _taxService.ComputeTax(frequency, 0, taxAmount5);
            Assert.AreEqual((decimal)708.2785, tax5);
        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel4()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 10000;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)708.33, tax1);

            //With Tax
            decimal taxAmount2 = 12500.22M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)1208.374, tax2);

            //With Tax
            decimal taxAmount3 = 15832.99M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)1874.928, tax3);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel5()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 15833;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)1875, tax1);

            //With Tax
            decimal taxAmount2 = 23253.33M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)3730.0825, tax2);

            //With Tax
            decimal taxAmount3 = 24999.99M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)4166.7475, tax3);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel6()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 25000;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)4166.67, tax1);

            //With Tax
            decimal taxAmount2 = 37999.26M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)8066.448, tax2);

            //With Tax
            decimal taxAmount3 = 45832.99M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)10416.567, tax3);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel7()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 45833;
            var tax1 = _taxService.ComputeTax(frequency, 0, taxAmount1);
            Assert.AreEqual((decimal)10416.67, tax1);

            //With Tax
            decimal taxAmount2 = 50140.25M;
            var tax2 = _taxService.ComputeTax(frequency, 0, taxAmount2);
            Assert.AreEqual((decimal)11794.99, tax2);

            //With Tax
            decimal taxAmount3 = 100000.75M;
            var tax3 = _taxService.ComputeTax(frequency, 0, taxAmount3);
            Assert.AreEqual((decimal)27750.35, tax3);

        }


        [TestMethod]
        public void ComputeTaxMonthlyLevel1Dependent1()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //No Tax
            decimal taxAmount1 = 4000;
            var tax1 = _taxService.ComputeTax(frequency, 1, taxAmount1);
            Assert.AreEqual(0, tax1);

            //No Tax
            decimal taxAmount2 = 6249;
            var tax2 = _taxService.ComputeTax(frequency, 1, taxAmount2);
            Assert.AreEqual(0, tax2);

            //No Tax
            decimal taxAmount3 = 6250;
            var tax3 = _taxService.ComputeTax(frequency, 1, taxAmount3);
            Assert.AreEqual(0, tax3);

            //With Tax
            decimal taxAmount4 = 6500.99M;
            var tax4 = _taxService.ComputeTax(frequency, 1, taxAmount4);
            Assert.AreEqual((decimal)12.5495, tax4);

            //With Tax
            decimal taxAmount5 = 7082.99M;
            var tax5 = _taxService.ComputeTax(frequency, 1, taxAmount5);
            Assert.AreEqual((decimal)41.6495, tax5);
        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel2Dependent2()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 9167;
            var tax1 = _taxService.ComputeTax(frequency, 2, taxAmount1);
            Assert.AreEqual((decimal)41.67, tax1);

            //With Tax
            decimal taxAmount2 = 9504.50M;
            var tax2 = _taxService.ComputeTax(frequency, 2, taxAmount2);
            Assert.AreEqual((decimal)75.42, tax2);

            //With Tax
            decimal taxAmount3 = 10555;
            var tax3 = _taxService.ComputeTax(frequency, 2, taxAmount3);
            Assert.AreEqual((decimal)180.47, tax3);

            //With Tax
            decimal taxAmount4 = 10832.99M;
            var tax4 = _taxService.ComputeTax(frequency, 2, taxAmount4);
            Assert.AreEqual((decimal)208.269, tax4);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel3Dependent3()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 12917;
            var tax1 = _taxService.ComputeTax(frequency, 3, taxAmount1);
            Assert.AreEqual((decimal)208.33, tax1);

            //With Tax
            decimal taxAmount2 = 13536.50M;
            var tax2 = _taxService.ComputeTax(frequency, 3, taxAmount2);
            Assert.AreEqual((decimal)301.255, tax2);

            //With Tax
            decimal taxAmount3 = 15989;
            var tax3 = _taxService.ComputeTax(frequency, 3, taxAmount3);
            Assert.AreEqual((decimal)669.13, tax3);

            //With Tax
            decimal taxAmount4 = 16249.99M;
            var tax4 = _taxService.ComputeTax(frequency, 3, taxAmount4);
            Assert.AreEqual((decimal)708.2785, tax4);

        }

        [TestMethod]
        public void ComputeTaxMonthlyLevel4Dependent4()
        {
            Initialize();

            var frequency = FrequencyType.Monthly;

            //With Tax
            decimal taxAmount1 = 18333;
            var tax1 = _taxService.ComputeTax(frequency, 4, taxAmount1);
            Assert.AreEqual((decimal)708.33, tax1);

            //With Tax
            decimal taxAmount2 = 20999.50M;
            var tax2 = _taxService.ComputeTax(frequency, 4, taxAmount2);
            Assert.AreEqual((decimal)1241.63, tax2);

            //With Tax
            decimal taxAmount3 = 23158;
            var tax3 = _taxService.ComputeTax(frequency, 4, taxAmount3);
            Assert.AreEqual((decimal)1673.33, tax3);

            //With Tax
            decimal taxAmount4 = 24166.99M;
            var tax4 = _taxService.ComputeTax(frequency, 4, taxAmount4);
            Assert.AreEqual((decimal)1875.128, tax4);
        }
    }
}
