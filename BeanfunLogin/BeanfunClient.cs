﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Specialized;


namespace BeanfunLogin
{
    public partial class BeanfunClient : WebClient
    {
        private System.Net.CookieContainer CookieContainer;
        private Uri ResponseUri;
        public string errmsg;
        private string webtoken;
        private string cardid;
        public List<AccountList> accountList;
        bool redirect;
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

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

        static BeanfunClient()
        {
            // 原本預設的協定為 SSL3.0 + TLS1.0
            // 由於Beanfun現在(2019/09/19)要求需要使用TLS1.2以上才能進入
            // 在此將協定設定為 SSL3.0 + TLS1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
        }

        public BeanfunClient()
        {
            this.redirect = true;
            this.CookieContainer = new System.Net.CookieContainer();
            this.Headers.Set("User-Agent", userAgent);
            this.ResponseUri = null;
            this.errmsg = null;
            this.webtoken = null;
            this.accountList = new List<AccountList>();
        }

        public string DownloadString(string Uri, Encoding Encoding)
        {
            var ret = (Encoding.GetString(base.DownloadData(Uri)));
            return ret;
        }

        public string DownloadString(string Uri)
        {
            this.Headers.Set("User-Agent", userAgent);
            var ret = base.DownloadString(Uri);
            return ret;
        }

        public byte[] UploadValues(string skey, NameValueCollection payload)
        {
            this.Headers.Set("User-Agent", userAgent);
            return base.UploadValues(skey, payload);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            HttpWebRequest request2 = webRequest as HttpWebRequest;

            if (request2 != null)
            {
                request2.CookieContainer = this.CookieContainer;
                request2.AllowAutoRedirect = this.redirect;
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
            byte[] raw = null;

            raw = this.DownloadData("http://tw.beanfun.com/beanfun_block/generic_handlers/echo_token.ashx?webtoken=1");
            string ret = Encoding.GetString(raw);
            Debug.WriteLine(GetCurrentTime() + " @ " +ret);
        }

    }
}
