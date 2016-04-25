using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities;
using Payroll.Repository.Interface;

namespace Payroll.Service.Implementations
{
    public class DeductionAmountService : IDeductionAmountService
    {
        private IDeductionAmountRepository _deductionAmountRepository;

        public DeductionAmountService(IDeductionAmountRepository deductionAmountRepository)
        {
            _deductionAmountRepository = deductionAmountRepository;
        }

        public IList<DeductionAmount> GetByDeduction(int deductionId)
        {
            return _deductionAmountRepository.GetByDeduction(deductionId);
        }

        public DeductionAmount GetByDeductionAndAmount(int deductionId, decimal amount)
        {
            return _deductionAmountRepository.GetByDeductionAndAmount(deductionId, amount);
        }
    }
}
