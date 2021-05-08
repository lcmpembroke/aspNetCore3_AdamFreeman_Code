using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Platform.Services
{
    public class GuidService : IGuidResponseService
    {
        private Guid guid = Guid.NewGuid();

        public async Task FormatGuid(HttpContext context, string content)
        {
            await context.Response.WriteAsync($"\nGuid: {guid}\n{content}\n\n");
        }

    }
}
