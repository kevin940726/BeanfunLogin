using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeanfunLogin
{
    partial class BeanfunClient
    {
        public void GetAccounts(string service_code, string service_region, bool fatal = true)
        {
            if (this.webtoken == null)
            { return; }

            LoginMethod loginMethod = this.cardid==null ? LoginMethod.Regular : LoginMethod.PlaySafe;

            Regex regex;
            string response;
            if (loginMethod == LoginMethod.PlaySafe)
                response = this.DownloadString("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D" + service_code + "_" + service_region + "&web_token=" + webtoken + "&cardid=" + this.cardid, Encoding.UTF8);
            else
                response = this.DownloadString("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D" + service_code + "_" + service_region + "&web_token=" + webtoken, Encoding.UTF8);

            if (loginMethod == LoginMethod.PlaySafe)
            {
                regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstate"; return; }
                string viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoEventvalidation"; return; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                NameValueCollection payload = new NameValueCollection();
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("btnCheckPLASYSAFE", "Hidden+Button");
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D" + service_code + "_" + service_region + "&web_token=" + webtoken + "&cardid=" + cardid, payload));
            }

            // Add account list to ListView.
            regex = new Regex("<div id=\"(\\w+)\" sn=\"(\\d+)\" name=\"([^\"]+)\"");
            this.accountList.Clear();
            foreach (Match match in regex.Matches(response))
            {
                if (match.Groups[1].Value == "" || match.Groups[2].Value == "" || match.Groups[3].Value == "")
                { continue; }
                accountList.Add(new AccountList(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
            }
            if (fatal && accountList.Count == 0)
            { this.errmsg = "LoginNoAccount"; return; }

            this.errmsg = null;
        }
    }
}
