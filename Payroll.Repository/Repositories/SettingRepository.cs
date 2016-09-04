using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using CacheManager.Core;
using Payroll.Repository.Constants;

namespace Payroll.Repository.Repositories
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        private readonly ICacheManager<object> _cacheManager;

        public SettingRepository(IDatabaseFactory databaseFactory, ICacheManager<object> cacheManager = null)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Settings;
            _cacheManager = cacheManager;
        }

        public override void Update(Setting entity)
        {
            base.Update(entity);

            var cachedEmployee = _cacheManager != null ? _cacheManager.Get(entity.SettingKey, CacheRegion.Settings) : null;
            if (cachedEmployee != null)
                _cacheManager.Remove(entity.SettingKey, CacheRegion.Settings);
        }

        public new virtual IEnumerable<Setting> GetAll()
        {
            return base.GetAll().ToList();
        }

        public virtual string GetSettingValue(string key, string defaultValue = "")
        {
            var cachedSetting = GetSettingByKey(key);
            return cachedSetting != null ? cachedSetting.Value : defaultValue;
        }

        public Setting GetSettingByKey(string key)
        {
            var cachedSetting = _cacheManager != null ? _cacheManager.Get(key, CacheRegion.Settings) : null;
            if (cachedSetting == null)
            {
                var setting = Find(x => x.SettingKey == key).FirstOrDefault();
                if (_cacheManager != null)
                    _cacheManager.Add(key, setting, CacheRegion.Settings);
               
                return setting;
            }

            return cachedSetting as Setting;
        }
    }
}
