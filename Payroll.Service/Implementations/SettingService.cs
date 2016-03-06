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
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(ISettingRepository settingRepository, IUnitOfWork unitOfWork)
        {
            _settingRepository = settingRepository;
            _unitOfWork = unitOfWork;
        }

        public String GetByKey(String Key)
        {
            return _settingRepository.GetSettingValue(Key);
        }
    }
}
