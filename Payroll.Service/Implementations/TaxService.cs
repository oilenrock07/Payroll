using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class TaxService : BaseEntityService<Tax>, ITaxService
    {
        private ITaxRepository _taxRepository;

        public TaxService(ITaxRepository taxRepository)
            : base(taxRepository)
        {
            _taxRepository = taxRepository;            
        }

        public decimal ComputeTax(FrequencyType frequency, int dependentCount, decimal totalTaxableIncome)
        {
            var tax =
               _taxRepository.GetByTaxableAmount(frequency, dependentCount, totalTaxableIncome);

            decimal totalTaxAmount = 0;

            if (tax != null)
            {
                //Compute tax
                totalTaxAmount = tax.BaseTaxAmount;
            
                //For excess of base amount
                if (totalTaxableIncome > tax.BaseAmount)
                {
                    var test = (decimal)(totalTaxableIncome - tax.BaseAmount);
                    var test2 = (decimal)tax.OverPercentage / (decimal)100;

                    totalTaxAmount += (decimal)(totalTaxableIncome - tax.BaseAmount) * ((decimal)tax.OverPercentage / (decimal)100);
                }
            }

            return totalTaxAmount;
        }
    }
}
