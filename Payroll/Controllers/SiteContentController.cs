using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Payroll.Repository.Interface;

namespace Payroll.Controllers
{
    public class SiteContentController : Controller
    {

        private readonly ISettingRepository _settingRepository;

        public SiteContentController(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }


    }
}