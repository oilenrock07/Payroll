using Payroll.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeInfoService
    {
        EmployeeInfo GetByEmployeeId(int employeeId);

        IList<EmployeeInfo> GetActiveByPaymentFrequency(int PaymentFrequencyId);
    }
}
