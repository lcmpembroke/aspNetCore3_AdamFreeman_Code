using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Platform.Services
{
    public class GuidService : IResponseFormatter
    {
        private Guid guid = Guid.NewGuid();

        public async Task Format(HttpContext context, string content)
        {
            await context.Response.WriteAsync($"\nGuid: {guid}\n{content}\n\n");
        }

        //public async Task FormatGuid(HttpContext context, string content)
        //{
        //    await context.Response.WriteAsync($"\nGuid: {guid}\n{content}\n\n");
        //}

    }
}
