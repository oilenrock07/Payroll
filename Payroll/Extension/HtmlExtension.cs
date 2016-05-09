using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Payroll.Extension
{
    public static class HtmlExtension
    {
        public static MvcHtmlString DatePickerFor(this HtmlHelper htmlHelper, string name, DateTime? value, object htmlAttributes)
        {
            var textbox = new TagBuilder("input");
            textbox.Attributes.Add("type", "text");
            textbox.Attributes.Add("name", name);
            textbox.Attributes.Add("id", name);

            if (value != null && value != DateTime.MinValue)
            {
                textbox.Attributes.Add("value", Convert.ToDateTime(value).ToString("MM/dd/yyyy"));
            }

            textbox.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            string html = String.Format("<div class=\"input-group\">{0}<label class=\"input-group-addon\" for=\"{1}\"><i class=\"fa fa-calendar\"></i></label></div>", textbox.ToString(TagRenderMode.Normal), name);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DatePickerFor(this HtmlHelper htmlHelper, string name, string value, object htmlAttributes)
        {
            var textbox = new TagBuilder("input");
            textbox.Attributes.Add("type", "text");
            textbox.Attributes.Add("name", name);
            textbox.Attributes.Add("id", name);

            if (!string.IsNullOrEmpty(value) && Convert.ToDateTime(value) != DateTime.MinValue)
            {
                textbox.Attributes.Add("value", value);
            }

            textbox.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            string html = String.Format("<div class=\"input-group\">{0}<label class=\"input-group-addon\" for=\"{1}\"><i class=\"fa fa-calendar\"></i></label></div>", textbox.ToString(TagRenderMode.Normal), name);
            return MvcHtmlString.Create(html);
        }
    }
}