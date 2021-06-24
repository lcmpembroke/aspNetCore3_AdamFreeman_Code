using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace WebApp.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "[route-data=true]")]
    public class RouteDataTagHelper : TagHelper
    {
        [ViewContext]               // set the [ViewContext] attribute so that the ViewContext property gets set to the current ViewContext
        [HtmlAttributeNotBound]     // prevents a value being assigned to this property if there's a matching attribute on the div element
        public ViewContext Context { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class","bg-primary m-2 p-2");

            TagBuilder list = new TagBuilder("ul");
            list.Attributes["class"] = "list-group";
            RouteValueDictionary rvDictionary = Context.RouteData.Values;
            if (rvDictionary.Count > 0)
            {
                foreach (var kvp in rvDictionary)
                {
                    TagBuilder item = new TagBuilder("li");
                    item.Attributes["class"] = "list-group-item";
                    item.InnerHtml.Append($"{kvp.Key}: {kvp.Value}");
                    
                    list.InnerHtml.AppendHtml(item);
                }
                output.Content.AppendHtml(list);
            }
            else
            {
                output.Content.Append("No route data");
            }
        }

    }
}
