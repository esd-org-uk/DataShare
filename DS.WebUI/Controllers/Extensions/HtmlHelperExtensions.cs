using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml.Linq;
using DS.Domain;

namespace DS.WebUI.Controllers.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DropDownListExtended(this HtmlHelper html, string Id, IEnumerable<SelectListItemExtended> selectList)
        {
            var selectDoc = XDocument.Parse(html.DropDownList(Id, selectList).ToString());
            
            var options = from XElement el in selectDoc.Element("select").Descendants()
                          select el;

            foreach (var item in options)
            {
                var itemValue = item.Attribute("value");
                item.SetAttributeValue("class", selectList.Where(x => x.Value == itemValue.Value).Single().CssClass);
            }

            // rebuild the control, resetting the options with the ones you modified
            selectDoc.Root.ReplaceAll(options.ToArray());
            selectDoc.Root.SetAttributeValue("Id", Id);
            selectDoc.Root.SetAttributeValue("Name", Id);
            return MvcHtmlString.Create(selectDoc.ToString());
        }
    }
}