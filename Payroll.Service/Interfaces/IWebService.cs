using System.Collections.Generic;
using System.Web;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Service.Interfaces
{
    public interface IWebService
    {
        IPaginationModel GetPaginationModel(HttpRequestBase request, int itemCount, int itemsPerPage = 0, string pageName = "");
        IEnumerable<T> TakePaginationModel<T>(IEnumerable<T> list, IPaginationModel pagination) where T : class;
    }
}
