using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Payroll;
using Payroll.Repository.Interface;

namespace Payroll.Service.Implementations
{
    public class DeductionService : IDeductionService
    {
        private IDeductionRepository _deductionRepository;

        public DeductionService(IDeductionRepository deductionRepository)
        {
            _deductionRepository = deductionRepository;
        }

        public IList<Deduction> GetAllActive()
        {
            return _deductionRepository.GetAllActive().ToList();
        }

        public Deduction GetByName(string name)
        {
            return _deductionRepository.GetByName(name);
        }
    }
}
