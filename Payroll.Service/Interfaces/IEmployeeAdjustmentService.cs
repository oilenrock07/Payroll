using System;
using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Repository.Models.Payroll;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeAdjustmentService
    {
        IEnumerable<EmployeeAdjustmentDao> GetEmployeeAdjustmentByDate(DateTime startDate, DateTime endDate);
        IEnumerable<EmployeeAdjustment> GetEmployeeAdjustments(int employeeId, DateTime startDate, DateTime endDate);
        void Update(EmployeeAdjustment employeeAdjustment);
    }
}
