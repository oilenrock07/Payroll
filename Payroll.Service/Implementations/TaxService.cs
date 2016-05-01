using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
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

        public TaxService(ITaxRepository taxRepository) : base(taxRepository)
        {
            _taxRepository = taxRepository;
        }
        public decimal ComputeTax(decimal taxableAmount)
        {
            throw new NotImplementedException();
        }
    }
}
