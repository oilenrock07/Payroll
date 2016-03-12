using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Models.Maintenance;
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
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IWebService _webService;

        public MaintenanceController(IUnitOfWork unitOfWork, ISettingRepository settingRepository, IPositionRepository positionRepository, IPaymentFrequencyRepository paymentFrequencyRepository, 
            IHolidayRepository holidayRepository, IDepartmentRepository departmentRepository, IWebService webService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
            _positionRepository = positionRepository;
            _paymentFrequencyRepository = paymentFrequencyRepository;
            _departmentRepository = departmentRepository;
            _holidayRepository = holidayRepository;
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

        public virtual ActionResult PaymentFrequency()
        {
            var paymentFrequencies = _paymentFrequencyRepository.Find(x => x.IsActive).ToList();
            return View(paymentFrequencies);
        }

        public virtual ActionResult DeletePaymentFrequency(int id)
        {
            var paymentFrequency = _paymentFrequencyRepository.GetById(id);
            _paymentFrequencyRepository.Update(paymentFrequency);
            paymentFrequency.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("PaymentFrequency");
        }

        public virtual ActionResult CreatePaymentFrequency()
        {
            
            var frequencies = new List<SelectListItem>();
            foreach (Common.Enums.Frequency val in Enum.GetValues(typeof(Common.Enums.Frequency)))
            {
                frequencies.Add(new SelectListItem
                {
                    Text = val.ToString(),
                    Value = ((int)val).ToString()
                });
            }

            var dayOfWeeks = new List<SelectListItem>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                dayOfWeeks.Add(new SelectListItem
                {
                    Text = dayOfWeek.ToString(),
                    Value = ((int)dayOfWeek).ToString()
                });
            }

            var viewModel = new PaymentFrequencyViewModel
            {
                Frequencies = frequencies,
                DayOfWeeks = dayOfWeeks,
                PaymentFrequency = new PaymentFrequency { FrequencyId = 1, MonthlyStartDay = 15, MonthlyEndDay = 30, WeeklyStartDayOfWeek = 3}
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreatePaymentFrequency(PaymentFrequencyViewModel viewModel)
        {
            var paymentFrequency = viewModel.PaymentFrequency.MapItem<PaymentFrequency>();
            paymentFrequency.IsActive = true;
            _paymentFrequencyRepository.Add(paymentFrequency);
            _unitOfWork.Commit();

            return RedirectToAction("PaymentFrequency");
        }


        public virtual ActionResult Department()
        {
            var departments = _departmentRepository.Find(x => x.IsActive);
            return View(departments);
        }

        public virtual ActionResult EditDepartment(int id)
        {
            var department = _departmentRepository.GetById(id);
            return View(department);
        }

        public virtual ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateDepartment(Department department)
        {
            _departmentRepository.Add(department);
            _unitOfWork.Commit();
            return RedirectToAction("Department");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditDepartment(Department model)
        {
            var department = new Department { DepartmentId = model.DepartmentId };
            _departmentRepository.Update(department);
            department.InjectFrom(model);
            _unitOfWork.Commit();
            return RedirectToAction("Department");
        }

        public virtual ActionResult DeleteDepartment(int id)
        {
            var department = _departmentRepository.GetById(id);
            _departmentRepository.Update(department);
            department.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Department");
        }

        public virtual ActionResult Holiday()
        {
            var holidays = _holidayRepository.GetHolidaysByCurrentYear();
            return View(holidays);
        }

        public virtual ActionResult CreateHoliday()
        {
            return View(new Holiday());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateHoliday(Holiday holiday)
        {
            holiday.Year = DateTime.Now.Year;
            holiday.IsActive = true;

            _holidayRepository.Add(holiday);
            _unitOfWork.Commit();

            return RedirectToAction("Holiday");
        }

        public virtual ActionResult EditHoliday(int id)
        {
            var holiday = _holidayRepository.GetById(id);
            return View(holiday);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditHoliday(Holiday model)
        {
            var holiday = _holidayRepository.GetById(model.HolidayId);
            _holidayRepository.Update(holiday);

            holiday.InjectFrom(model);
            holiday.IsActive = true;

            _unitOfWork.Commit();
            return RedirectToAction("Holiday");
        }

        public virtual ActionResult DeleteHoliday(int id)
        {
            var holiday = _holidayRepository.GetById(id);
            _holidayRepository.Update(holiday);
            holiday.IsActive = false;
            _unitOfWork.Commit();

            return RedirectToAction("Holiday");
        }
    }
}