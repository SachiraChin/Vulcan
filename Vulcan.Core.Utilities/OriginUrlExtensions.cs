using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.Owin;

namespace Vulcan.Core.Utilities
{
    public static class OriginUrlExtensions
    {
        public static string GetOriginUrl(this IOwinRequest request)
        {
            if (request == null || request.Headers == null) return null;

            var origin = request.Headers["Origin"];
            var referer = request.Headers["Referer"];

            if (origin == null && referer == null) return null;
            var requestUri = new Uri(origin ?? referer);
            var requestOrigin = requestUri.Scheme + "://" + requestUri.Host + (requestUri.Port > 0 && requestUri.Port != 80 && requestUri.Port != 443 ? ":" + requestUri.Port : "");
            return requestOrigin;
        }


        public static string GetOriginUrl(this HttpRequestMessage request)
        {
            if (request == null || request.Headers == null) return null;

            var origin = request.Headers.Contains("Origin") ? request.Headers.FirstOrDefault(h => h.Key == "Origin").Value.FirstOrDefault() : null;
            var referer = request.Headers.Referrer;

            if (origin == null && referer == null) return null;
            var requestUri = origin != null ? new Uri(origin) : referer;
            var requestOrigin = requestUri.GetFullHost();
            return requestOrigin;
        }
        public static string GetOriginUrl(this HttpRequestBase request)
        {
            if (request == null || request.Headers == null) return null;

            var origin = request.Headers["Origin"];
            var referer = request.Headers["Referer"];

            if (origin == null && referer == null) return null;
            var requestUri = new Uri(origin ?? referer);
            var requestOrigin = requestUri.GetFullHost();
            return requestOrigin;
        }

        public static string GetFormattedOriginUrl(this string url)
        {
            if (url == null) return null;
            try
            {
                var requestUri = new Uri(url);

                var requestOrigin = requestUri.GetFullHost();
                return requestOrigin;

            }
            catch (UriFormatException)
            {
                return null;
            }
        }
    }
}
