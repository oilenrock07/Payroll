using System;
using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Caching
{
    public class CachedDataService : ICachedDataService
    {
        private readonly ISettingRepository _settingRepository; 
        private readonly ICacheService _cacheService;

        public CachedDataService(ICacheService cacheService, ISettingRepository settingRepository)
        {
            _cacheService = cacheService;
            _settingRepository = settingRepository;
        }

        public virtual IEnumerable<Setting> GetAllSettings()
        {
            var cacheKey = "GetAllSettings";
            return _cacheService.Get(cacheKey, new TimeSpan(0, 10, 0), null, () => _settingRepository.GetAll());
        }

        public virtual string GetSettingValue(string key, string defaultValue = "")
        {
            var cacheKey = String.Format("GetSettingValue-{0}", key);

            return _cacheService.Get(cacheKey, new TimeSpan(0, 10, 0), null, () => _settingRepository.GetSettingValue(key, defaultValue));
        }
    }
}
