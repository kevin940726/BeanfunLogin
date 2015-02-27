using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;


namespace BeanfunLogin
{
    public partial class BeanfunClient : WebClient
    {
        private System.Net.CookieContainer CookieContainer;
        private Uri ResponseUri;
        public string errmsg;
        private string webtoken;
        public List<AccountList> accountList;

        public class AccountList
        {
            public string sacc;
            public string sotp;
            public string sname;
            public string screatetime;

            public AccountList()
            { this.sacc = null; this.sotp = null; this.sname = null; this.screatetime = null; }
            public AccountList(string sacc, string sotp, string sname, string screatetime = null)
            { this.sacc = sacc; this.sotp = sotp; this.sname = sname; this.screatetime = screatetime; }
        }

        public BeanfunClient()
        {
            this.CookieContainer = new System.Net.CookieContainer();
            this.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36");
            this.ResponseUri = null;
            this.errmsg = null;
            this.webtoken = null;
            this.accountList = new List<AccountList>();
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

        private string GetCookie(string cookieName)
        {
            foreach (Cookie cookie in this.CookieContainer.GetCookies(new Uri("https://tw.beanfun.com/")))
            {
                if (cookie.Name == cookieName)
                {
                    return cookie.Value;
                }
            }
            return null;
        }

        private string GetCurrentTime(int method = 0)
        {
            DateTime date = DateTime.Now;
            switch (method)
            {
                case 1:
                    return (date.Year - 1900).ToString() + (date.Month - 1).ToString() + date.ToString("ddHHmmssfff");
                case 2:
                    return date.Year.ToString() + (date.Month - 1).ToString() + date.ToString("ddHHmmssfff");
                default:
                    return date.ToString("yyyyMMddHHmmss.fff");
            }
        }

        public void Ping()
        {
            this.DownloadString("https://tw.new.beanfun.com/beanfun_block/generic_handlers/record_service_start.ashx");
        }


    }
}
