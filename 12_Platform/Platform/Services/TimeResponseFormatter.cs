using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Platform.Services
{
    public class TimeResponseFormatter : IResponseFormatter
    {
        private ITimeStamper _timeStamper;

        public TimeResponseFormatter(ITimeStamper timeStamper)
        {
            _timeStamper = timeStamper;
        }

        public async Task Format(HttpContext context, string content)
        {
            await context.Response.WriteAsync($"{_timeStamper.TimeStamp}: {content}");
        }
    }
}
