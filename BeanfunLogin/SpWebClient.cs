using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Collections;

// By Inndy.
// Add and edit a few code.
namespace BeanfunLogin
{
    public class SpWebClient : WebClient
    {
        public SpWebClient()
        {
            this.CookieContainer = new System.Net.CookieContainer();
            this.ResponseUri = null;
        }

        public SpWebClient(System.Net.CookieContainer CookieContainer)
        {
            this.CookieContainer = CookieContainer;
            this.ResponseUri = null;
        }

        public string DownloadString(string Uri, Encoding Encoding)
        {
            return (Encoding.GetString(base.DownloadData(Uri)));
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            HttpWebRequest request2 = webRequest as HttpWebRequest;
            if (request2 != null)
            {
                request2.CookieContainer = this.CookieContainer;
            }
            return webRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse webResponse = base.GetWebResponse(request);
            this.ResponseUri = webResponse.ResponseUri;
            return webResponse;
        }

        public System.Net.CookieContainer CookieContainer { get; private set; }

        public string getCookie(string name)
        {
            Hashtable table = (Hashtable)this.CookieContainer.GetType().InvokeMember("m_domainTable",
                                                                         BindingFlags.NonPublic |
                                                                         BindingFlags.GetField |
                                                                         BindingFlags.Instance,
                                                                         null,
                                                                         this.CookieContainer,
                                                                         new object[] { });
            foreach (var key in table.Keys)
            {
                foreach (Cookie cookie in this.CookieContainer.GetCookies(new Uri(string.Format("http://{0}/", key))))
                {
                    if (cookie.Name == name) return cookie.Value;
                }
            }
            return "Fail";
        }

        public Uri ResponseUri { get; private set; }
    }
}
