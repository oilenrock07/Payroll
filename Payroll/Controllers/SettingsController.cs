using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Settings;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebService _webService;
        private readonly ISettingRepository _settingRepository;
        private readonly ISchedulerLogService _schedulerLogService;

        public SettingsController(IUnitOfWork unitOfWork, ISettingRepository settingRepository, ISchedulerLogService schedulerLogService, IWebService webService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
            _schedulerLogService = schedulerLogService;
            _webService = webService;
        }

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "key", VaryByCustom = "setting:key")]
        public string SettingValue(string key)
        {
            return _settingRepository.GetSettingValue(key);
        }

        public ActionResult Index()
        {
            var settings = _settingRepository.GetAll().OrderBy(x => x.Category);
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

        public ActionResult SchedulerLogs(string startDate = "", string endDate = "")
        {
            var viewModel = new SchedulerLogViewModel();
            
            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var logs = _schedulerLogService.GetSchedulerLogs(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate)).ToList();
                var pagination = _webService.GetPaginationModel(Request, logs.Count);
                viewModel.SchedulerLogs = _webService.TakePaginationModel(logs, pagination);
                viewModel.PaginationModel = pagination;

                if (!logs.Any()) ViewBag.NoResult = true;
            }

            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;

            return View(viewModel);
        }
    }
}