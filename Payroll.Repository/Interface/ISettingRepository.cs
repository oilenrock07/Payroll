using Payroll.Infrastructure.Interfaces;
using Payroll.Entities;

namespace Payroll.Repository.Interface
{
    public interface ISettingRepository : IRepository<Setting>
    {
        string GetSettingValue(string key, string defaultValue = "");
    }
}
