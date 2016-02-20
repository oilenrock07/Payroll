using System.Collections.Generic;
using System.Linq;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Service.Models
{
    public class PaginationModel<T> : IPaginationModel<T>
    {
        protected const int DisplayPageRange = 2;

        public virtual int CurrentPage { get; set; }
        public virtual int TotalPages { get; set; }
        public virtual int TotalItems { get; set; }
        public virtual string PageName { get; set; }
        public virtual int ItemsPerPage { get; set; }
        public virtual int DefaultItemsPerPage { get; set; }
        public virtual string PagingText { get; set; }
        public virtual IEnumerable<T> Items { get; set; }


        /// <summary>
        /// Whether or not to display the first page indicator
        /// </summary>
        public virtual bool DisplayFirstPageIndicator
        {
            get
            {
                return StartDisplayPage > 1;
            }
        }

        /// <summary>
        /// Whether or not to display the last page indicator
        /// </summary>
        public virtual bool DisplayLastPageIndicator
        {
            get
            {
                return EndDisplayPage < TotalPages;
            }
        }


        public virtual int StartDisplayPage
        {
            get
            {
                var start = CurrentPage - DisplayPageRange;
                if (start < 1) start = 1;

                return start;
            }
        }

        public virtual int EndDisplayPage
        {
            get
            {
                var end = StartDisplayPage + ( DisplayPageRange * 2 );
                if (end > TotalPages) end = TotalPages;

                return end;
            }
        }

        /// <summary>
        /// The valid range of pages to be displayed. Ensures we dont display too many at once
        /// </summary>
        public virtual IEnumerable<int> DisplayPages
        {
            get
            {
                // Only display two pages either side of current
                return Enumerable.Range(StartDisplayPage, EndDisplayPage - StartDisplayPage + 1);
            }
        }
    }
}