using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading;

namespace BeanfunLogin
{
    public partial class BeanfunClient : WebClient
    {

        // Decrypt OTP.
        private string DecryptDES(string hexString, string key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.None;
                des.Key = Encoding.ASCII.GetBytes(key);
                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                ICryptoTransform desencrypt = des.CreateDecryptor();
                return Encoding.ASCII.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));
            }
            catch (Exception e)
            {
                this.errmsg = "DecryptDESError\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        public string GetOTP(int loginMethod, AccountList acc, string service_code="610074", string service_region="T9")
        {
            try
            {
                string response;
                if (true) //loginMethod == (int)LoginMethod.PlaySafe || loginMethod == (int)LoginMethod.QRCode)
                    response = this.DownloadString("https://tw.beanfun.com/beanfun_block/game_zone/game_start_step2.aspx?service_code="+service_code+"&service_region="+service_region+"&sotp=" + acc.sotp + "&dt=" + GetCurrentTime(2), Encoding.UTF8);
                else
                    response = this.DownloadString("https://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start_step2.aspx%3Fservice_code%3D"+service_code+"%26service_region%3D"+service_region+"%26sotp%3D" + acc.sotp + "&web_token=" + this.webtoken);
                if (response == "")
                { this.errmsg = "OTPNoResponse"; return null; }

                Regex regex = new Regex("GetResultByLongPolling&key=(.*)\"");
                if (!regex.IsMatch(response))
                { this.errmsg = "OTPNoLongPollingKey"; return null; }
                string longPollingKey = regex.Match(response).Groups[1].Value;
                if (acc.screatetime == null)
                {
                    regex = new Regex("ServiceAccountCreateTime: \"([^\"]+)\"");
                    if (!regex.IsMatch(response))
                    { this.errmsg = "OTPNoCreateTime"; return null; }
                    acc.screatetime = regex.Match(response).Groups[1].Value;
                }
                response = this.DownloadString("https://tw.newlogin.beanfun.com/generic_handlers/get_cookies.ashx", Encoding.UTF8);

                regex = new Regex("var m_strSecretCode = '(.*)';");
                if (!regex.IsMatch(response))
                { this.errmsg = "OTPNoSecretCode"; return null; }
                string secretCode = regex.Match(response).Groups[1].Value;
                
                NameValueCollection payload = new NameValueCollection();
                payload.Add("service_code", service_code);
                payload.Add("service_region", service_region);
                payload.Add("service_account_id", acc.sacc);
                payload.Add("service_sotp", acc.sotp);
                payload.Add("service_display_name", acc.sname);
                payload.Add("service_create_time", acc.screatetime);
                // testing...
                System.Net.ServicePointManager.Expect100Continue = false;
                this.UploadValues("https://tw.beanfun.com/beanfun_block/generic_handlers/record_service_start.ashx", payload);
                response = this.DownloadString("https://tw.beanfun.com/generic_handlers/get_result.ashx?meth=GetResultByLongPolling&key=" + longPollingKey + "&_=" + GetCurrentTime());
                //Thread.Sleep(5000);
                //Debug.WriteLine(Environment.TickCount);
                response = this.DownloadString("http://tw.beanfun.com/beanfun_block/generic_handlers/get_webstart_otp.ashx?SN=" + longPollingKey + "&WebToken=" + this.webtoken + "&SecretCode=" + secretCode + "&ppppp=1F552AEAFF976018F942B13690C990F60ED01510DDF89165F1658CCE7BC21DBA&ServiceCode=" + service_code+"&ServiceRegion="+service_region+"&ServiceAccount=" + acc.sacc + "&CreateTime=" + acc.screatetime.Replace(" ", "%20") + "&d=" + Environment.TickCount);
                response = response.Substring(2);
                string key = response.Substring(0, 8);
                string plain = response.Substring(8);
                string otp = DecryptDES(plain, key);
                if (otp != null)
                    this.errmsg = null;       
           
                return otp;
            }
            catch (Exception e)
            {
                this.errmsg = "獲取密碼失敗，請嘗試重新登入。\n\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

    }
}
