using System.Collections.Generic;

namespace Payroll.Service.Interfaces.Model
{
    public interface IPaginationModel<T>
    {
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        int TotalItems { get; set; }
        string PageName { get; set; }
        int ItemsPerPage { get; set; }
        int DefaultItemsPerPage { get; set; }
        string PagingText { get; set; }
        bool DisplayFirstPageIndicator { get; }
        bool DisplayLastPageIndicator { get; }
        int StartDisplayPage { get; }
        int EndDisplayPage { get; }
        IEnumerable<int> DisplayPages { get; }
        IEnumerable<T> Items { get; set; }
    }
}
