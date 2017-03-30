using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Settings
{
    public class SystemLogViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public IEnumerable<Log> Logs { get; set; }
        public IPaginationModel PaginationModel { get; set; }
    }
}