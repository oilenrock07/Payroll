using Payroll.Service.Interfaces;
using System;

namespace Payroll.Service.Implementations
{
    public class ValidationService : IValidationService
    {
        public bool ValidateBirthDate(DateTime birthdate)
        {
            return !(birthdate > DateTime.Now) || birthdate < new DateTime(1900, 1, 1);
        }
    }
}
