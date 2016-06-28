using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Settings;
        }

        public new virtual IEnumerable<Setting> GetAll()
        {
            return base.GetAll().ToList();
        }

        public virtual string GetSettingValue(string key, string defaultValue = "")
        {
            var setting = Find(x => x.SettingKey == key).FirstOrDefault();
            return setting != null ? setting.Value : defaultValue;
        }

        public Setting GetSettingByKey(string key)
        {
            return Find(x => x.IsActive && x.SettingKey == key).FirstOrDefault();
        }
    }
}
