using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Payroll.Helper
{
    public static class Export
    {
        public static void ToExcel(HttpResponseBase response, object clientsList, string fileName)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = clientsList;
            grid.DataBind();
            response.ClearContent();
            fileName = String.Format("{0}.xls", fileName);
            response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            response.ContentType = "application/excel";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);
            response.Write(sw.ToString());
            response.End();
        }
    }
}