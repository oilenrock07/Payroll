using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingRepository _settingRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IWebService _webService;

        public MaintenanceController(IUnitOfWork unitOfWork, ISettingRepository settingRepository, IPositionRepository positionRepository, IWebService webService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
            _positionRepository = positionRepository;
            _webService = webService;
        }

        public virtual ActionResult Position()
        {
            var positions = _positionRepository.Find(x => x.IsActive);
            return View(positions);
        }

        public virtual ActionResult CreatePosition()
        {
            return View(new Position());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreatePosition(Position model)
        {
            model.IsActive = true;
            _positionRepository.Add(model);
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }
        
        public virtual ActionResult EditPosition(int id)
        {
            var position = _positionRepository.GetById(id);
            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditPosition(Position model)
        {
            var position = new Position { PositionId = model.PositionId };
            _positionRepository.Update(position);
            position.InjectFrom(model);
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }

        public virtual ActionResult DeletePosition(int id)
        {
            var position = _positionRepository.GetById(id);
            _positionRepository.Update(position);
            position.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Position");
        }
    }
}