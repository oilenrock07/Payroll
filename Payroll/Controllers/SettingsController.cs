using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingRepository _settingRepository;

        public SettingsController(IUnitOfWork unitOfWork, ISettingRepository settingRepository)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
        }

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "key", VaryByCustom = "setting:key")]
        public string SettingValue(string key)
        {
            return _settingRepository.GetSettingValue(key);
        }

        public ActionResult Index()
        {
            var settings = _settingRepository.GetAll().ToList();
            return View(settings);
        }

        public ActionResult Edit(int id)
        {
            var setting = _settingRepository.GetById(id);
            return View(setting);
        }

        [HttpPost]
        public ActionResult Edit(Setting setting)
        {
            var updateSetting = new Setting {SettingId = setting.SettingId};
            _settingRepository.Update(updateSetting);
            updateSetting.InjectFrom(setting);
            _unitOfWork.Commit();

            return RedirectToAction("Index");
        }
    }
}