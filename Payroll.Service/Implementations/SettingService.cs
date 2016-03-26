using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Service.Interfaces;
using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Service.Implementations
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
      
        public SettingService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public String GetByKey(String Key)
        {
            return _settingRepository.GetSettingValue(Key);
        }
    }
}
