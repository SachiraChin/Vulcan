using System;

namespace Vulcan.Core.Utilities
{
    public static class UriExtensions
    {
        public static string GetFullHost(this Uri uri)
        {
            return uri.Scheme + "://" + uri.Host + (uri.Port > 0 && uri.Port != 80 && uri.Port != 443 ? ":" + uri.Port : "");
        }
    }
}
