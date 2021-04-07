using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LanguageFeatures.Models
{
    public class MyAsyncMethods
    {

        // BOOK p104
        public static async IAsyncEnumerable<long?> GetPageLengths(List<string> output, params string[] urls)
        {
            HttpClient httpClient = new HttpClient();
            foreach (string url in urls)
            {
                output.Add($"Started request for {url}");
                var httpMessage = await httpClient.GetAsync($"http://{url}");
                output.Add($"Completed request for {url}");
                yield return httpMessage.Content.Headers.ContentLength;
            }
        }


        // BOOK p100/101 - asynchronous methods - using async / await to simplify code
        public async static Task<long?> GetPageLength()
        {
            HttpClient client = new HttpClient();

            var httpMesssage = await client.GetAsync("http://apress.com");
            return httpMesssage.Content.Headers.ContentLength;
        }

        // BOOK p99 - asynchronous methods - working with tasks directly
        public static Task<long?> GetPageLength_firstOption()
        {
            HttpClient client = new HttpClient();
            
            var httpTask = client.GetAsync("http://apress.com");

            // Below is the continuation code - the mechanism by which what happens when the task is ocmplete.A ContinueWith method processes the HttpResponseMessage object
            // frist return keyword specifies I a returning a Task<HttpResponseMessage> object, which, when the task is complete, will return the result of the GetPageLength method
            return httpTask.ContinueWith((Task<HttpResponseMessage> antecedent) =>
            {
                return antecedent.Result.Content.Headers.ContentLength;
            });
        }

    }
}
