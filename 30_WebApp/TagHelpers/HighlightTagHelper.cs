using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp.TagHelpers
{
    [HtmlTargetElement("*",Attributes = "[highlight=true]")]
    public class HighlightTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // NOTE SetHtmlContent() used
            output.PreContent.SetHtmlContent("<b><i>");
            output.PostContent.SetHtmlContent("</b></i>");
        }
    }
}
