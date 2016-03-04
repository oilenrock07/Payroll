using System;

namespace Payroll.Service.Interfaces
{
    public interface IValidationService
    {
        bool ValidateBirthDate(DateTime birthdate);
    }
}
