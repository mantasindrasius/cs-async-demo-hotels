using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public static class Utils
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = ErrorHandler
        };

        public static HttpClient MakeRestClient(string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }


        public static A ReadJson<A>(string source)
        {
            return JsonConvert.DeserializeObject<A>(source, jsonSettings);
            //var reader = new JsonReader();
            //return serializer.Deserialize<A>(source);
        }

        public static void ErrorHandler(object x, ErrorEventArgs error)
        {
            Console.WriteLine(error.ErrorContext.Error);
            error.ErrorContext.Handled = true;
        }

        public static A UnwrapResult<A>(Task<A> task)
        {
            task.Wait();

            return task.Result;
        }
    }
}
