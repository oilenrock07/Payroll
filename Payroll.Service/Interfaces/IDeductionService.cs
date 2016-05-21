using Payroll.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IDeductionService
    {
        IList<Deduction> GetAllActive();

        Deduction GetByName(String name);

        IList<Deduction> GetAllCustomizable();
    }
}
