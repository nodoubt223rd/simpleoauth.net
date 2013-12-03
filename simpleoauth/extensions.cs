using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SimpleOAuth
{
    /// <summary>
    /// Provider of core extension methods.
    /// </summary>
    static public class CoreExtensionMethods
    {
        #region Dictionary<string, object>
        /// <summary>
        /// Creates a query string from this dictionary.
        /// </summary>
        /// <param name="d">This dictionary.</param>
        /// <returns>The query string, including the leading '<c>?</c>' or if the dictionary is empty, <see cref="string.Empty"/>.</returns>
        /// <remarks>Null values are ignored.</remarks>
        static public string ToQueryString(this Dictionary<string, object> d)
        {
            if (0 == d.Count)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (string k in d.Keys)
            {
                if (null == d[k])
                    continue;

                if (sb.Length > 0)
                    sb.Append('&');

                sb.AppendFormat("{0}={1}", k, HttpUtility.UrlEncode(d[k].ToString()));
            }

            if (0 == sb.Length)
                return string.Empty;

            sb.Insert(0, '?');

            return sb.ToString();
        }
        #endregion Dictionary<string, object>
    }
}
