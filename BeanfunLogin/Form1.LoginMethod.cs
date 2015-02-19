using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using FSFISCATLLib;

namespace BeanfunLogin
{
    public partial class main : Form
    {

        private string getServerTime()
        {
            DateTime date = DateTime.Now;
            return (date.Year - 1900).ToString() + (date.Month - 1).ToString() + date.ToString("ddHHmmssfff");
        }

        private bool vaktenAuthenticate(string lblSID)
        {
            try
            {
                string[] ports = { "14057", "16057", "17057" };
                foreach (string port in ports)
                {
                    string response = this.web.DownloadString("https://localhost:" + port + "/api/1/status.jsonp?api=YXBpLmtleXBhc2NvaWQuY29tOjQ0My9SZXN0L0FwaVNlcnZpY2Uv&callback=_jqjsp&alt=json-in-script");
                    response = this.web.DownloadString("https://localhost:" + port + "/api/1/aut.jsonp?sid=GAMANIA" + lblSID + "&api=YXBpLmtleXBhc2NvaWQuY29tOjQ0My9SZXN0L0FwaVNlcnZpY2Uv&callback=_jqjsp&alt=json-in-script");
                    if (response == "_jqjsp( {\"statusCode\":200} );") return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string regularLogin(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("CodeTextBox", "");
                payload.Add("btn_login.x", "46");
                payload.Add("btn_login.y", "31");
                payload.Add("LBD_VCID_c_login_idpass_form_samplecaptcha", "");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }      

        private string keypascoLogin(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/keypasco_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;
                regex = new Regex("samplecaptcha\" value=\"(\\w+)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"samplecaptcha\".";
                this.captchaId = regex.Match(response).Groups[1].Value;
                regex = new Regex("lblSID\"><font color=\"White\">(\\w+)</font></span>");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"lblSID\".";
                string lblSID = regex.Match(response).Groups[1].Value;
                if (!vaktenAuthenticate(lblSID))
                    return "登入失敗，與伺服器驗證失敗，請檢查是否安裝vakten程式。\nNo response with keypasco api.";

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("CodeTextBox", "");
                payload.Add("btn_login.x", "46");
                payload.Add("btn_login.y", "31");
                payload.Add("LBD_VCID_c_login_keypasco_form_samplecaptcha", captchaId);
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/keypasco_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

        private string gamaotpLogin(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/gamaotp_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;
                regex = new Regex("motp_challenge_code\" value=\"(\\d+)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"motp\".";
                string motp = regex.Match(response).Groups[1].Value;
                response = this.web.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + getServerTime());
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"sOtp\".";
                string sotp = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("original", "M~" + sotp + "~" + userID + "~" + pass + "|" + motp);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("motp_challenge_code", motp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/gamaotp_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

        private string otpLogin(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/otp_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.web.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + getServerTime());
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"sOtp\".";
                string sotp = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("original", "O~" + sotp + "~" + userID + "~" + pass);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/otp_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

        private string otpELogin(string userID, string pass, string securePass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/otp_form.aspx?type=E&skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.web.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + getServerTime());
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"sOtp\".";
                string sotp = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("original", "E~" + sotp + "~" + userID + "~" + pass);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_MainAccountPassword", pass);
                payload.Add("t_Password", securePass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/otp_form.aspx?type=E&skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

        private string playsafeLogin(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/playsafe_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__VIEWSTATE\".";
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"__EVENTVALIDATION\".";
                this.eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.web.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + getServerTime());
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                    return "登入失敗。\nCannot find \"sOtp\".";
                string sotp = regex.Match(response).Groups[1].Value;

                this.fs = new FSFISCClass();
                var readername = GetReader();
                if (readername == "Fail")
                    return "登入失敗，找不到讀卡機。\nFailed to get reader.";
                if (cardType == "")
                    return "登入失敗，晶片卡讀取失敗。\nFailed to get card type.";
                this.cardid = GetPublicCN(readername);
                if (cardid == "Fail")
                    return "登入失敗，找不到讀卡機。\nFailed to get reader's public CN.";
                var opinfo = GetOPInfo(readername, pass);
                if (opinfo == "Fail")
                    return "登入失敗，讀卡機讀取失敗。\nFailed to get OP Info.";                
                var original = cardType + "~" + sotp + "~" + userID + "~" + opinfo;
                var encryptedData = EncryptData(readername, pass, original);
                if (encryptedData == "Fail")
                    return "登入失敗，晶片卡讀取失敗。\nFailed to encrypt data.";
                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("card_check_id", cardid);
                payload.Add("original", original);
                payload.Add("signature", encryptedData);
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/playsafe_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

    }
}
