using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Controllers
{
    public class LookupController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICompanyRepository _companyRepository;

        public LookupController(IEmployeeService employeeService, ICompanyRepository companyRepository)
        {
            _employeeService = employeeService;
            _companyRepository = companyRepository;
        }

        public JsonResult LookUpEmployee(string criteria)
        {
            var names = _employeeService.SearchEmployee(criteria);
            var result = names.Select(x => new
            {
                name = x.FullName,
                id= x.EmployeeId,
            });

            return Json(result);
        }

        public JsonResult LookUpCompany(string criteria)
        {
            var names = _companyRepository.SearchCompany(criteria);
            var result = names.Select(x => new
            {
                name = x.CompanyName,
                id = x.CompanyId
            });

            return Json(result);
        }
    }
}