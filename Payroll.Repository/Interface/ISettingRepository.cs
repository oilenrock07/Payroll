using System.Collections.Generic;
using Payroll.Infrastructure.Interfaces;
using Payroll.Entities;

namespace Payroll.Repository.Interface
{
    public interface ISettingRepository : IRepository<Setting>
    {
        new IEnumerable<Setting> GetAll();
        string GetSettingValue(string key, string defaultValue = "");
    }
}
