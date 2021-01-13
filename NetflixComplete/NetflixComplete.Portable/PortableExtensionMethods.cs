using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetflixComplete.Portable
{
    public static class PortableExtensionMethods
    {
        public static void CopyTo(this CookieCollection cookies, Cookie[] destinationCookies, int destinationIndex)
        {
            foreach (Cookie sourceCookie in cookies)
            {
                destinationCookies[destinationIndex++] = sourceCookie;
            }
        }

        public static void ForEach<T>(this List<T> cookieList, Action<T> a)
        {
            foreach (var cookie in cookieList)
            {
                a(cookie);
            }
        }

        public static string ToTitleCase(this string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
                return originalString;

            string firstLetter = originalString[0].ToString().ToUpper();
            string remainingWord = originalString.Substring(1);

            return $"{firstLetter}{remainingWord}";
        }

        
    }
}
