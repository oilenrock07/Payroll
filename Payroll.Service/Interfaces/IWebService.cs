using System.Collections.Generic;
using System.Web;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Service.Interfaces
{
    public interface IWebService
    {
        IPaginationModel<T> GetPaginationModel<T>(HttpRequestBase request, IEnumerable<T> items, int itemsPerPage = 0, string pageName = "");
    }
}
