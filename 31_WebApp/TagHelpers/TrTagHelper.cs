using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp.TagHelpers
{
    // Narrowing scope
    //[HtmlTargetElement("tr", Attributes = "bg-color,text-color", ParentTag ="thead" )]

    // Widening scope
    [HtmlTargetElement("tr", Attributes = "bg-color,text-color")]
    [HtmlTargetElement("td", Attributes = "bg-color")]
    public class TrTagHelper : TagHelper
    {
        public string BgColor { get; set; } = "dark";
        public string TextColor { get; set; } = "white";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", $"bg-{BgColor} text-center text-{TextColor}");
        }
    }
}