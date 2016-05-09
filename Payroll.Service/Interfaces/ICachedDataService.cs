using System.Collections.Generic;
using Payroll.Entities;

namespace Payroll.Service.Interfaces
{
    public interface ICachedDataService
    {
        IEnumerable<Setting> GetAllSettings();
        string GetSettingValue(string key, string defaultValue = "");
    }
}
