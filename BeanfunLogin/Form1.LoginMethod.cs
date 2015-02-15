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
using FSSCUTILATLLib;
using FSCAPIATLLib;
using FSP11CRYPTATLLib;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        // Working progress, waiting for playsafe card to debug.
        /*public void GetReader()
        {
            FSFISCClass fs = new FSFISCClass();
            try
            {
                Debug.WriteLine(fs.FSFISC_GetErrorCode());
            }
            catch { Debug.WriteLine("what"); }

            Debug.WriteLine("done");

            char readername = '\0';
            string aaa;
            try
            {
                aaa = FSFISC_GetReaderNames(0);
                Debug.WriteLine(aaa);
            }
            catch
            {
                return 'X';
            }
            if (aaa == null)
                return 'Z';
            else
            {
                var readers = aaa.ToArray();
                foreach (var reader in readers)
                {
                    var cardflag = FSFISC_GetCardType2(reader);
                    Debug.WriteLine(cardflag);
                    if (FSFISC_GetErrorCode() != 0)
                        cardflag = "";
                    else if (cardflag == "0")
                    {
                        readername = reader;
                        cardType = "F";
                    }
                    else if (cardflag == "1")
                    {
                        readername = reader;
                        cardType = "G";
                    }

                }
                if (readername != '\0')
                {
                    CardReader = readername;
                }
                else
                    return 'F';

                return readername;
            }
        }

        private string GetPublicCN(char reader)
        {
            if (reader == '\0')
                return "Fail";
            string rtn = FSFISC_GetPublicCN(reader, 0);
            if (FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }

        private string GetOPInfo(char reader, string pin)
        {
            if (reader == '\0')
                return "Fail";
            string rtn = FSFISC_GetOPInfo(reader, pin, 0);
            if (FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }

        private string EncryptData(char reader, string pin, string data)
        {
            if (reader == '\0')
                return "Fail";
            string rtn = FSFISC_GetTAC(reader, pin, data, 0, 0);
            if (FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }*/


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
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
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
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
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
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
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
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
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
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
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

                /*var readername = GetReader();
                var original = cardType + "~" + sotp + "~" + userID + "~" + GetOPInfo(readername, pass);
                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", this.viewstate);
                payload.Add("__EVENTVALIDATION", this.eventvalidation);
                payload.Add("card_check_id", GetPublicCN(readername));
                payload.Add("original", original);
                payload.Add("signature", EncryptData(readername, pass, original));
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.newlogin.beanfun.com/login/playsafe_form.aspx?skey=" + skey, payload));
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    return "登入失敗。\nNo response for webtoken.";
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.";
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;*/

                return "OK";
            }
            catch
            {
                return "登入失敗，未知的錯誤。\nUnknown Error.";
            }
        }

    }
}
