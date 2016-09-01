using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using Payroll.Service.Interfaces.Model;
using Payroll.Service.Models;
using Payroll.Repository.Constants;

namespace Payroll.Service.Implementations
{
    public class WebService : IWebService
    {
        private readonly ISettingRepository _settingRepository;

        public WebService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public IEnumerable<T> TakePaginationModel<T>(IEnumerable<T> list, IPaginationModel pagination) where T : class 
        {
            list = (pagination.CurrentPage > 0 ? list.Skip((pagination.CurrentPage - 1) * pagination.ItemsPerPage).Take(pagination.ItemsPerPage) : list);
            return list;
        }

        public IPaginationModel GetPaginationModel(HttpRequestBase request, int itemCount, int itemsPerPage = 0, string pageName = "")
        {
            var page = Convert.ToInt32(request.QueryString["Page"] ?? "1");

            if (itemsPerPage == 0)
            {
                itemsPerPage = Convert.ToInt16(_settingRepository.GetSettingValue(SettingValue.PAGINATION_ITEMS_PER_PAGE));
            }

            return new PaginationModel
            {
                PageName = pageName,
                CurrentPage = page == 0 ? 1 : page,
                TotalPages = Convert.ToInt32(Math.Ceiling((decimal)itemCount / itemsPerPage)),
                TotalItems = itemCount,
                DefaultItemsPerPage = itemsPerPage,
                ItemsPerPage = itemsPerPage,
                
            };
        }
    }
}
