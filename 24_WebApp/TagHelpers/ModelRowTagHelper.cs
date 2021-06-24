using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace WebApp.TagHelpers
{
    // Example uses of this tagHelper:              <tr for="Name">   or   <tr for="Price" format="c">
    [HtmlTargetElement("tr", Attributes = "for")]   
    public class ModelRowTagHelper : TagHelper
    {
        public string Format { get; set; }          // used for innerHtml of <td>
        public ModelExpression For { get; set; }    // used to receive the value of the "for" attribute - remember properties (PascalCase) relate to attributes (kebab-case)
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            TagBuilder th = new TagBuilder("th");
            //th.InnerHtml.Append(For.Name);
            th.InnerHtml.Append(For.Name.Split(".").Last());
            th.Attributes.Add("highlight", "true");
            output.Content.AppendHtml(th);

            TagBuilder td = new TagBuilder("td");
            if (Format != null && For.Metadata.ModelType == typeof(decimal))
            {
                td.InnerHtml.Append(((decimal)For.Model).ToString(Format));
            }
            else
            {
                td.InnerHtml.Append(For.Model.ToString());
            }
            output.Content.AppendHtml(td);
        }
    }
}
