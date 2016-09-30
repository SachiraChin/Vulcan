using System;
using System.Text;

namespace Vulcan.Core.Utilities
{
    public static class StringExtensions
    {
        public static string EncodeToBase64(this string toEncode)
        {
            var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            var returnValue  = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFromBase64(this string encodedData)
        {
            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            var returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static double ToDouble(this string str)
        {
            double val;
            double.TryParse(str, out val);

            return val;
        }
    }
}
