using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace WebApp.TagHelpers
{

    public class TimeTagHelperComponent : TagHelperComponent
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string timestamp = DateTime.Now.ToLongTimeString();
            if (output.TagName == "body")
            {
                TagBuilder divElement = new TagBuilder("div");
                divElement.Attributes.Add("class", "bg-info text-white m-2 p-2");
                divElement.InnerHtml.Append($"Time: {timestamp}");
                output.PreContent.AppendHtml(divElement);
            }
        }
    }
}
