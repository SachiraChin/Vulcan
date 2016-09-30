using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vulcan.Core.Utilities
{
    public static class WebAccess
    {
        public class HttpResult<T> where T : class
        {
            public T Result { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string ResultString { get; set; }
        }

        public static async Task<HttpResult<T>> ApiRequest<T>(string baseAddress, string endpoint, HttpMethod method, object data = null, Func<MultipartFormDataContent, MultipartFormDataContent> mutipartContentProvider = null, AuthenticationHeaderValue authHeaderValue = null) where T : class
        {
            var c = new HttpClient
            {
                BaseAddress = new Uri(baseAddress),
                Timeout = TimeSpan.FromSeconds(60)
            };
            var req = new HttpRequestMessage(method, endpoint);
            if (method != HttpMethod.Get)
            {
                if (mutipartContentProvider == null)
                {
                    if (data != null)
                    {
                        var s = data as string;
                        if (s != null)
                        {
                            req.Content = new StringContent(s, Encoding.UTF8, "application/json");
                        }
                        else
                        {
                            var dataStr = JsonConvert.SerializeObject(data);
                            req.Content = new StringContent(dataStr, Encoding.UTF8, "application/json");
                        }
                    }
                }
                else
                {
                    var mutipartContent = new MultipartFormDataContent("----------------------------" + DateTime.Now.Ticks.ToString("x"));
                    mutipartContent = mutipartContentProvider(mutipartContent);
                    req.Content = mutipartContent;
                }
            }
            if (authHeaderValue != null)
                req.Headers.Authorization = authHeaderValue;
            var msg = await c.SendAsync(req);
            var httpresult = new HttpResult<T> { StatusCode = msg.StatusCode };

            string resStr;
            try
            {
                resStr = await msg.Content.ReadAsStringAsync();
                httpresult.ResultString = resStr;
            }
            catch (Exception ex)
            {
                return httpresult;
            }

            if (typeof(T) == typeof(object))
            {
                return httpresult;
            }

            httpresult.Result = (T)JsonConvert.DeserializeObject(resStr, typeof(T));

            return httpresult;
        }
    }
}
