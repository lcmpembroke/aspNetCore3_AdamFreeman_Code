using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace WebApp.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "[wrap=true]")]
    public class ContentWrapperTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.AllAttributes.ContainsName("wrap"))
            {
                //System.Console.WriteLine("context.AllAttributes *****");
                //foreach (var item in context.AllAttributes)
                //{
                //    System.Console.WriteLine(item.Name + "   " + item.Value);
                //}

                //System.Console.WriteLine();
                //System.Console.WriteLine("output.Attributes *****");
                //foreach (var item in output.Attributes)
                //{
                //    System.Console.WriteLine(item.Name + "   " + item.Value);
                //}

                // removes the wrap attribute
                //output.Attributes.RemoveAll("wrap");

            }

            TagBuilder divElement = new TagBuilder("div");
            divElement.Attributes["class"] = "bg-primary text-white p-2 m-2";
            divElement.InnerHtml.AppendHtml("Wrapper");

            // NOTE AppendHtml() used
            output.PreElement.AppendHtml(divElement);
            output.PostElement.AppendHtml(divElement);
        }

    }
}
