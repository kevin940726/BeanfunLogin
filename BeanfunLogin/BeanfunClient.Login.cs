using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web;

namespace BeanfunLogin
{
    public partial class BeanfunClient : WebClient
    {

        private string RegularLogin(string id, string pass, string skey)
        {
            try
            {
                string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    {this.errmsg = "LoginNoViewstate"; return null;}
                string viewstate = regex.Match(response).Groups[1].Value;

                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    { this.errmsg = "LoginNoEventvalidation"; return null; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__VIEWSTATEGENERATOR\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstateGenerator"; return null; }
                string viewstateGenerator = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"LBD_VCID_c_login_idpass_form_samplecaptcha\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoSamplecaptcha"; return null; }
                string samplecaptcha = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__VIEWSTATEGENERATOR", viewstateGenerator);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("t_AccountID", id);
                payload.Add("t_Password", pass);
                payload.Add("CodeTextBox", "");
                payload.Add("btn_login.x", "0");
                payload.Add("btn_login.y", "0");
                payload.Add("LBD_VCID_c_login_idpass_form_samplecaptcha", samplecaptcha);

                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey, payload));
                //Debug.WriteLine(response);
                //regex = new Regex("akey%3d(.*)\"");
                //if (!regex.IsMatch(response.ToString()))
                //{ this.errmsg = "LoginNoAkey"; return null; }
                //string akey = regex.Match(response).Groups[1].Value;
                //string s = this.DownloadString("https://tw.newlogin.beanfun.com/login/final_step.aspx?akey=" + akey);
                //Debug.WriteLine(s);

                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = "LoginNoAkey"; return null; }
                string akey = regex.Match(this.ResponseUri.ToString()).Groups[1].Value;

                return akey;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        private bool vaktenAuthenticate(string lblSID)
        {
            try
            {
                string[] ports = { "14057", "16057", "17057" };
                foreach (string port in ports)
                {
                    string response = this.DownloadString("https://vaktenlocal.com:" + port + "/api/2/authenticate.jsonp?customerId=GAMANIA&sessionId=" + lblSID + "&api=https://api.keypascoid.com/Rest/ApiService/3&callback=_jqjsp&alt=json-in-script");
                    if (response == "_jqjsp( {\"statusCode\":200} );")
                    {
                        //response = this.DownloadString("https://localhost:" + port + "/api/1/aut.jsonp?sid=GAMANIA" + lblSID + "&api=YXBpLmtleXBhc2NvaWQuY29tOjQ0My9SZXN0L0FwaVNlcnZpY2Uv&callback=_jqjsp&alt=json-in-script");
                        //if (response == "_jqjsp( {\"statusCode\":200} );") return true;
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string KeypascoLogin(string id, string pass, string skey)
        {
            try
            {
                string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/keypasco_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstate"; return null; }
                string viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoEventvalidation"; return null; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                regex = new Regex("lblSID\"><font color=\"White\">(\\w+)</font></span>");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoLblSID"; return null; }
                string lblSID = regex.Match(response).Groups[1].Value;
                if (!vaktenAuthenticate(lblSID))
                { this.errmsg = "LoginNoResponseVakten"; return null; }

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("t_AccountID", id);
                payload.Add("t_Password", pass);
                payload.Add("CodeTextBox", "");
                payload.Add("btn_login.x", "46");
                payload.Add("btn_login.y", "31");
                payload.Add("LBD_VCID_c_login_keypasco_form_samplecaptcha", "");
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/keypasco_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = "LoginNoAkey"; return null; }
                return regex.Match(this.ResponseUri.ToString()).Groups[1].Value;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        public class GamaotpClass
        {
            public string skey;
            public string sotp;
            public string motp;
            public string viewstate;
            public string eventvalidation;

            public GamaotpClass(string skey, string sotp, string motp, string viewstate, string eventvalidation)
            { this.skey = skey; this.sotp = sotp; this.motp = motp; this.viewstate = viewstate; this.eventvalidation = eventvalidation; }
        }

        [Obsolete("Method1 is deprecated, unsupport login method", true)]
        public GamaotpClass GetGamaotpPassCode(string skey)
        {
            string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/gamaotp_form.aspx?skey=" + skey);
            Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoViewstate"; return null; }
            string viewstate = regex.Match(response).Groups[1].Value;
            regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoEventvalidation"; return null; }
            string eventvalidation = regex.Match(response).Groups[1].Value;
            regex = new Regex("motp_challenge_code\" value=\"(\\d+)\" />");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoMotp"; return null; }
            string motp = regex.Match(response).Groups[1].Value;
            response = this.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + GetCurrentTime(1));
            regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoSotp"; return null; }
            string sotp = regex.Match(response).Groups[1].Value;

            return new GamaotpClass(skey, sotp, motp, viewstate, eventvalidation);
        }

        [Obsolete("Method1 is deprecated, unsupport login method", true)]
        private string GamaotpLogin(string id, string pass, GamaotpClass gamaotpClass)
        {
            try
            {
                string skey = gamaotpClass.skey;
                string sotp = gamaotpClass.sotp;
                string motp = gamaotpClass.motp;
                string viewstate = gamaotpClass.viewstate;
                string eventvalidation = gamaotpClass.eventvalidation;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("original", "M~" + sotp + "~" + id + "~" + pass + "|" + motp);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("motp_challenge_code", motp);
                payload.Add("t_AccountID", id);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                string response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/gamaotp_form.aspx?skey=" + skey, payload));
                Regex regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = "LoginNoAkey"; return null; }
                string Akey = regex.Match(this.ResponseUri.ToString()).Groups[1].Value;

                // Thanks 小艾 for the testing and code.
                //要求寫入 Cookies 資料
                regex = new Regex("<script type=\"text/javascript\">var strWriteUrl = \"(.*)\";</script>");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoCookies"; return null; }
                response = DownloadString(regex.Match(response).Groups[1].Value);

                return Akey;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        [Obsolete("Method1 is deprecated, unsupport login method", true)]
        private string OtpLogin(string userID, string pass, string skey)
        {
            try
            {
                string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/otp_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstate"; return null; }
                string viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoEventvalidation"; return null; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + GetCurrentTime(1));
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoSotp"; return null; }
                string sotp = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("original", "O~" + sotp + "~" + userID + "~" + pass);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", userID);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/otp_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = "LoginNoAkey"; return null; }
                return regex.Match(this.ResponseUri.ToString()).Groups[1].Value;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        [Obsolete("Method1 is deprecated, unsupport login method", true)]
        private string OtpELogin(string id, string pass, string securePass, string skey)
        {
            try
            {
                string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/otp_form.aspx?type=E&skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstate"; return null; }
                string viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoEventvalidation"; return null; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + GetCurrentTime(1));
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoSotp"; return null; }
                string sotp = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("original", "E~" + sotp + "~" + id + "~" + securePass);
                payload.Add("signature", "");
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", id);
                payload.Add("t_MainAccountPassword", pass);
                payload.Add("t_Password", securePass);
                payload.Add("btn_login", "Login");
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/otp_form.aspx?type=E&skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = "LoginNoAkey"; return null; }
                return regex.Match(this.ResponseUri.ToString()).Groups[1].Value;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace; 
                return null;
            }
        }

        private string playsafeLogin(string id, string pass, string skey)
        {
            try
            {
                string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/playsafe_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoViewstate"; return null; }
                string viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoEventvalidation"; return null; }
                string eventvalidation = regex.Match(response).Groups[1].Value;
                response = this.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_security_otp.ashx?d=" + GetCurrentTime(1));
                regex = new Regex("<playsafe_otp>(\\w+)</playsafe_otp>");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginNoSotp"; return null; }
                string sotp = regex.Match(response).Groups[1].Value;

                PlaySafe ps = new PlaySafe();
                var readername = ps.GetReader();
                if (readername == null)
                { this.errmsg = "LoginNoReaderName"; return null; }
                if (ps.cardType == null)
                { this.errmsg = "LoginNoCardType"; return null; }

                string original = null;
                string signature = null;
                if (ps.cardType == "F")
                {
                    ps.cardid = ps.GetPublicCN(readername);
                    if (ps.cardid == null)
                    { this.errmsg = "LoginNoCardId"; return null; }
                    var opinfo = ps.GetOPInfo(readername, pass);
                    if (opinfo == null)
                    { this.errmsg = "LoginNoOpInfo"; return null; }
                    original = ps.cardType + "~" + sotp + "~" + id + "~" + opinfo;
                    signature = ps.EncryptData(readername, pass, original);
                    if (signature == null)
                    { this.errmsg = "LoginNoEncryptedData"; return null; }
                }
                else if (ps.cardType == "G")
                {
                    original = ps.cardType + "~" + sotp + "~" + id + "~";
                    signature = ps.FSCAPISign(pass, original);
                }
                NameValueCollection payload = new NameValueCollection();
                payload.Add("__EVENTTARGET", "");
                payload.Add("__EVENTARGUMENT", "");
                payload.Add("__VIEWSTATE", viewstate);
                payload.Add("__EVENTVALIDATION", eventvalidation);
                payload.Add("card_check_id", ps.cardid);
                payload.Add("original", original);
                payload.Add("signature", signature);
                payload.Add("serverotp", sotp);
                payload.Add("t_AccountID", id);
                payload.Add("t_Password", pass);
                payload.Add("btn_login", "Login");
                
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.newlogin.beanfun.com/login/playsafe_form.aspx?skey=" + skey, payload));
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.ResponseUri.ToString()))
                { this.errmsg = signature; return null; }
                return ps.cardid + " " + regex.Match(this.ResponseUri.ToString()).Groups[1].Value;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        public class QRCodeClass
        {
            public string skey;
            public string value;
            public string viewstate;
            public string eventvalidation;
            public Bitmap bitmap;
        }

        public QRCodeClass GetQRCodeValue(string skey)
        {
            string resp = this.DownloadString("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey);

            string response = this.DownloadString("https://tw.newlogin.beanfun.com/login/qr_form.aspx?skey=" + skey );
            Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoViewstate"; return null; }
            string viewstate = regex.Match(response).Groups[1].Value;

            regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoEventvalidation"; return null; }
            string eventvalidation = regex.Match(response).Groups[1].Value;

            //Thread.Sleep(3000);

            regex = new Regex("u=(.*)\" style");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoHash"; return null; }
            string value = regex.Match(response).Groups[1].Value;

            Stream stream = this.OpenRead("http://tw.newlogin.beanfun.com/qrhandler.ashx?u="  + value);

            QRCodeClass res = new QRCodeClass();
            res.skey = skey;
            res.viewstate = viewstate;
            res.eventvalidation = eventvalidation;
            res.value = Uri.UnescapeDataString(value);
            res.bitmap = new Bitmap(stream);

            return res;
        }

        private string QRCodeLogin(QRCodeClass qrcodeclass)
        {
            try
            {
                string skey = qrcodeclass.skey;
                
                this.Headers.Set("Referer", @"https://tw.newlogin.beanfun.com/login/qr_form.aspx?skey=" + skey);
                this.redirect = false;
                byte[] tmp2 = this.DownloadData("https://tw.newlogin.beanfun.com/login/qr_step2.aspx?skey=" + skey);
                this.redirect = true;
                string response2 = Encoding.UTF8.GetString(tmp2);
                Debug.Write(response2);
                Regex regex2 = new Regex("akey%3d(.*)%26authkey");
                if (!regex2.IsMatch(response2))
                { this.errmsg = "AKeyParseFailed"; return null; }
                string akey = regex2.Match(response2).Groups[1].Value;
                string test = this.DownloadString("https://tw.newlogin.beanfun.com/login/final_step.aspx?akey="+akey+"&authkey=N&bfapp=1");
                return akey;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        public int QRCodeCheckLoginStatus(QRCodeClass qrcodeclass)
        {
            try
            {
                string skey = qrcodeclass.skey;
                int errorCount = 0;
                string result;
                this.Headers.Set("Referer", @"https://tw.newlogin.beanfun.com/login/qr_form.aspx?skey=" + skey);

                NameValueCollection payload = new NameValueCollection();
                payload.Add("data", qrcodeclass.value);
                //Debug.WriteLine(qrcodeclass.value);

                string response = Encoding.UTF8.GetString(this.UploadValues("https://tw.bfapp.beanfun.com/api/Check/CheckLoginStatus", payload));
                Regex regex = new Regex("\"ResultMessage\":\"(.*)\",\"ResultDat");
                if (!regex.IsMatch(response))
                { this.errmsg = "LoginJsonParseFailed"; return -1; }

                result = regex.Match(response).Groups[1].Value;
                //Debug.WriteLine(result);
                if (result == "Failed")
                    return 0;
                else if (result == "Token Expired")
                {
                    //this.errmsg = "登入逾時，請重新取得QRCode";
                    return -2;
                }
                else if (result == "Success")
                    return 1;
                else
                {
                    this.errmsg = response;
                    return -1;
                }
            }
            catch (Exception e)
            {
                this.errmsg = "Network Error on QRCode checking login status\n\n" + e.Message + "\n" + e.StackTrace;
            }

            return -1;
        }

        public string GetSessionkey()
        {
            string response = this.DownloadString("https://tw.beanfun.com/beanfun_block/bflogin/default.aspx?service=999999_T0");
            //this.DownloadString(this.ResponseHeaders["Location"]);
            //this.DownloadString(this.ResponseHeaders["Location"]);
            //response = this.ResponseHeaders["Location"];
            response = this.ResponseUri.ToString();
            if (response == null)
            { this.errmsg = "LoginNoResponse"; return null; }
            Regex regex = new Regex("skey=(.*)&display");
            if (!regex.IsMatch(response))
            { this.errmsg = "LoginNoSkey"; return null; }
            return regex.Match(response).Groups[1].Value;
        }

        public void Login(string id, string pass, int loginMethod, string securePass = null, GamaotpClass gamaotpClass = null, QRCodeClass qrcodeClass = null, string service_code = "610074", string service_region = "T9")
        {
            try
            {
                string response = null;
                Regex regex;
                string skey = null;
                string akey = null;
                string cardid = null;
                if (loginMethod == (int)LoginMethod.QRCode)
                {
                    skey = qrcodeClass.skey;
                }
                else
                {
                    skey = GetSessionkey();
                }

                switch (loginMethod)
                {
                    case (int)LoginMethod.Regular:
                        akey = RegularLogin(id, pass, skey);
                        break;
                    case (int)LoginMethod.Keypasco:
                        akey = KeypascoLogin(id, pass, skey);
                        break;
                    case (int)LoginMethod.PlaySafe:
                        string r = playsafeLogin(id, pass, skey);
                        if (r == null)
                            return;
                        string[] temp = r.Split(' ');
                        if (temp.Count() != 2)
                        { this.errmsg = "LoginPlaySafeResultError"; return; }
                        cardid = temp[0];
                        akey = temp[1];
                        break;
                    case (int)LoginMethod.QRCode:
                        akey = QRCodeLogin(qrcodeClass);
                        break;
                    default:
                        this.errmsg = "LoginNoMethod";
                        return;
                }
                if (akey == null)
                    return;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("SessionKey", skey);
                payload.Add("AuthKey", akey);
                Debug.WriteLine(skey);
                Debug.WriteLine(akey);
                response = Encoding.UTF8.GetString(this.UploadValues("https://tw.beanfun.com/beanfun_block/bflogin/return.aspx", payload));
                Debug.WriteLine(response);
                response = this.DownloadString("https://tw.beanfun.com/" + this.ResponseHeaders["Location"]);
                Debug.WriteLine(response);
                Debug.WriteLine(this.ResponseHeaders);

                this.webtoken = this.GetCookie("bfWebToken");
                if (this.webtoken == "")
                { this.errmsg = "LoginNoWebtoken"; return; }
                if (loginMethod == (int)LoginMethod.PlaySafe)
                    response = this.DownloadString("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D"+service_code+"_"+service_region+"&web_token=" + webtoken + "&cardid=" + cardid, Encoding.UTF8);
                else
                    response = this.DownloadString("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D"+service_code+"_"+service_region+"&web_token=" + webtoken, Encoding.UTF8);

                if (loginMethod == (int)LoginMethod.PlaySafe)
                {
                    regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                    if (!regex.IsMatch(response))
                    { this.errmsg = "LoginNoViewstate"; return; }
                    string viewstate = regex.Match(response).Groups[1].Value;
                    regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                    if (!regex.IsMatch(response))
                    { this.errmsg = "LoginNoEventvalidation"; return; }
                    string eventvalidation = regex.Match(response).Groups[1].Value;
                    payload = new NameValueCollection();
                    payload.Add("__VIEWSTATE", viewstate);
                    payload.Add("__EVENTVALIDATION", eventvalidation);
                    payload.Add("btnCheckPLASYSAFE", "Hidden+Button");
                    response = Encoding.UTF8.GetString(this.UploadValues("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D"+service_code+"_"+service_region+"&web_token=" + webtoken + "&cardid=" + cardid, payload));
                }

                // Add account list to ListView.
                regex = new Regex("<div id=\"(\\w+)\" sn=\"(\\d+)\" name=\"([^\"]+)\"");
                this.accountList.Clear();
                foreach (Match match in regex.Matches(response))
                {
                    if (match.Groups[1].Value == "" || match.Groups[2].Value == "" || match.Groups[3].Value == "")
                    { this.errmsg = "LoginNoAccountMatch"; return; }
                    accountList.Add(new AccountList(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
                }
                if (accountList.Count == 0)
                { this.errmsg = "LoginNoAccount"; return; }

                this.errmsg = null;
            }
            catch (Exception e)
            {
                this.errmsg = "LoginUnknown\n\n" + e.Message + "\n" + e.StackTrace; 
                return;
            }
        }

    }
}
