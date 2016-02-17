using System;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Caching
{
    public class CachedSettingService : SettingRepository
    {
        private readonly ISettingRepository _settingRepository; 
        private readonly ICacheService _cacheService;

        public CachedSettingService(IDatabaseFactory databaseFactory, ICacheService cacheService, ISettingRepository settingRepository)
            : base(databaseFactory)
        {
            _cacheService = cacheService;
            _settingRepository = settingRepository;
        }

        public override IEnumerable<Setting> GetAll()
        {
            var cacheKey = "GetAll";
            return _cacheService.Get(cacheKey, new TimeSpan(0, 10, 0), null, () => _settingRepository.GetAll());
        }

        public override string GetSettingValue(string key, string defaultValue = "")
        {
            var cacheKey = String.Format("GetSettingValue-{0}", key);

            return _cacheService.Get(cacheKey, new TimeSpan(0, 10, 0), null, () => _settingRepository.GetSettingValue(key, defaultValue));
        }
    }
}
