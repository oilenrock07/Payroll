using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using Payroll.Service.Interfaces.Model;
using Payroll.Service.Models;

namespace Payroll.Service.Implementations
{
    public class WebService : IWebService
    {
        private readonly ISettingRepository _settingRepository;

        public WebService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public IPaginationModel<T> GetPaginationModel<T>(HttpRequestBase request, IEnumerable<T> items, int itemsPerPage = 0, string pageName = "")
        {
            var page = Convert.ToInt32(request.QueryString["Page"] ?? "1");
            var totalCount = items.Count();

            if (itemsPerPage == 0)
            {
                itemsPerPage = Convert.ToInt16(_settingRepository.GetSettingValue("PAGINATION_ITEMS_PER_PAGE"));
            }

            return new PaginationModel<T>
            {
                PageName = pageName,
                CurrentPage = page == 0 ? 1 : page,
                TotalPages = Convert.ToInt32(Math.Ceiling((decimal)totalCount / itemsPerPage)),
                TotalItems = totalCount,
                DefaultItemsPerPage = itemsPerPage,
                ItemsPerPage = itemsPerPage,
                Items =  (page > 0 ? items.Skip((page - 1) * itemsPerPage).Take(itemsPerPage) : items)
            };
        }
    }
}
