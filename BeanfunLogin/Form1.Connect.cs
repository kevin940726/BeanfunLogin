using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Net;
using System.Diagnostics;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private string viewstate;
        private string eventvalidation;
        private string captchaId;
        private string akey;
        private string webtoken;

        private string longPollingKey;
        private string secretCode;
        private string s_code = "610074";
        private string s_region = "T9";
        private string otp;
        private bool copyOrNot;

        private string getCurrentTime()
        {
            DateTime date = DateTime.Now;
            return date.ToString("yyyyMMddHHmmss.fff");
        }

        // Login do work.
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = "";
            try
            {
                int loginMethod = (int)e.Argument;
                string userID = this.textBox1.Text;
                string pass = this.textBox2.Text;
                string securePass = this.textBox4.Text;
                string result;

                if (loginMethod == 0)
                {
                    result = regularLogin(userID, pass);
                    if (result != "OK")
                        { e.Result = result; return; }
                }
                else if (loginMethod == 1)
                {
                    result = keypascoLogin(userID, pass);
                    if (result != "OK")
                        { e.Result = result; return; }
                }
                else if (loginMethod == 2)
                {
                    result = gamaotpLogin(userID, pass);
                    if (result != "OK")
                        { e.Result = result; return; }
                }
                else if (loginMethod == 3)
                {
                    result = otpLogin(userID, pass);
                    if (result != "OK")
                        { e.Result = result; return; }
                }
                else if (loginMethod == 4)
                {
                    result = otpELogin(userID, pass, securePass);
                    if (result != "OK")
                        { e.Result = result; return; }
                }
                else
                {
                    e.Result = "登入失敗，未選擇登入方式。\nFailed to select login method.";
                    return;
                }

                NameValueCollection payload = new NameValueCollection();
                payload = new NameValueCollection();
                payload.Add("SessionKey", this.skey);
                payload.Add("AuthKey", this.akey);
                string response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.beanfun.com/beanfun_block/bflogin/return.aspx", payload));
                this.webtoken = this.web.getCookie("bfWebToken");
                if (this.webtoken == "")
                    { e.Result = "登入失敗。\nNo response for webtoken."; return; }
                response = this.web.DownloadString("http://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D610074_T9&web_token=" + webtoken, Encoding.UTF8);
                if (response == "")
                    { e.Result = "登入失敗，無法取得帳號列表。\nNo response for account list."; return; }

                // Add account list to ListView.
                Regex regex = new Regex("<div id=\"(\\w+)\" sn=\"(\\d+)\" name=\"([^\"]+)\"");
                this.accountList = new List<AccountListClass>();
                foreach (Match match in regex.Matches(response))
                {
                    if (match.Groups[1].Value == "" || match.Groups[2].Value == "" || match.Groups[3].Value == "")
                        { e.Result = "登入失敗，無法取得帳號列表。\nNo match for account list."; return; }
                    this.accountList.Add(new AccountListClass(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
                }
                if (this.accountList.Count == 0)
                    { e.Result = "登入失敗，找不到遊戲帳號。\nNo account found."; return; }
                
            }
            catch
            {
                e.Result = "登入失敗，未知的錯誤。\nUnknown Error."; 
            }
        }

        // Login completed.
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.panel2.Enabled = true;
            this.UseWaitCursor = false;
            this.button1.Text = "登入";
            if (e.Error != null)
            {
                errexit("Login Failed", e.Error.Message, 1);
                return;
            }
            if ((string)e.Result != "")
            {
                errexit("Login Failed", (string)e.Result, 1);
                return;
            }

            try
            {
                listView1.Items.Clear();
                foreach (AccountListClass account in accountList)
                {
                    string[] row = { WebUtility.HtmlDecode(account.s_name), account.s_acc };
                    var listViewItem = new ListViewItem(row);
                    this.listView1.Items.Add(listViewItem);
                }

                // Handle panel switching.
                this.ActiveControl = null;
                this.panel2.SendToBack();
                this.panel1.BringToFront();
                this.AcceptButton = this.button3;
                if (Properties.Settings.Default.autoSelect == true)
                {
                    this.textBox3.Text = "獲取密碼中...";
                    this.listView1.Enabled = false;
                    this.copyOrNot = true;
                    this.backgroundWorker1.RunWorkerAsync(Properties.Settings.Default.autoSelectIndex);
                }
                if (Properties.Settings.Default.keepLogged)
                    this.ping.RunWorkerAsync();
            }
            catch
            {
                errexit("Login Failed", "登入失敗，無法取得帳號列表。\nAccount list or panel failed.", 1);
            }
            
        }

        private bool getOTP(int index)
        {
            try
            {
                string response = this.web.DownloadString("http://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start_step2.aspx%3Fservice_code%3D" + s_code + "%26service_region%3D" + s_region + "%26sotp%3D" + accountList[index].s_otp + "&web_token=" + webtoken);
                if (response == "")
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nNo response by \"s_otp\".", 2);
                Regex regex = new Regex("GetResultByLongPolling&key=(.*)\"");
                if (!regex.IsMatch(response))
                    return errexit("Get OTP Fail", "密碼獲取失敗，請嘗試重新登入。\nCannot find \"longPullingKey\".", 1);
                this.longPollingKey = regex.Match(response).Groups[1].Value;
                regex = new Regex("ServiceAccountCreateTime: \"([^\"]+)\"");
                if (!regex.IsMatch(response))
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nCannot find \"createTime\".", 2);
                this.accountList[index].s_createTime = regex.Match(response).Groups[1].Value;
                response = this.web.DownloadString("http://tw.newlogin.beanfun.com/generic_handlers/get_cookies.ashx");
                if (response == "")
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nNo response from cookies.", 2);
                regex = new Regex("var m_strSecretCode = '(.*)';");
                if (!regex.IsMatch(response))
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nCannot find \"secrectCode\".", 2);
                this.secretCode = regex.Match(response).Groups[1].Value;

                NameValueCollection payload = new NameValueCollection();
                payload.Add("service_code", s_code);
                payload.Add("service_region", s_region);
                payload.Add("service_account_id", accountList[index].s_acc);
                payload.Add("service_sotp", accountList[index].s_otp);
                payload.Add("service_display_name", accountList[index].s_name);
                payload.Add("service_create_time", accountList[index].s_createTime);
                response = Encoding.UTF8.GetString(this.web.UploadValues("http://tw.new.beanfun.com/beanfun_block/generic_handlers/record_service_start.ashx", payload));
                response = this.web.DownloadString("http://tw.new.beanfun.com/generic_handlers/get_result.ashx?meth=GetResultByLongPolling&key=" + longPollingKey + "&_=" + getCurrentTime());
                response = this.web.DownloadString("http://tw.new.beanfun.com/beanfun_block/generic_handlers/get_webstart_otp.ashx?SN=" + longPollingKey + "&WebToken=" + webtoken + "&SecretCode=" + secretCode + "&ppppp=FE40250C435D81475BF8F8009348B2D7F56A5FFB163A12170AD615BBA534B932&ServiceCode=610074&ServiceRegion=T9&ServiceAccount=" + accountList[index].s_acc + "&CreateTime=" + accountList[index].s_createTime.Replace(" ", "%20"));
                if (response == "")
                    return errexit("Get OTP Fail", "密碼獲取失敗，無法取得OTP密碼。\nNo response for OTP.", 2);
                response = response.Substring(2);
                string key = response.Substring(0, 8);
                string plain = response.Substring(8);
                this.otp = DecryptDES(plain, key);

                return true;
            }
            catch
            {
                return errexit("Get OTP Fail", "密碼獲取失敗，未知的錯誤。\nUnknown Error.", 2);
            }
        }

        // getOTP do work.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = (int)e.Argument;
            if (!getOTP(index))
                e.Result = -1;
            else
            {
                e.Result = index;
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "MapleStory")
                        return;
                }
                ProcessStartInfo gamestart = new ProcessStartInfo();
                gamestart.FileName = Properties.Settings.Default.gamePath;
                gamestart.Verb = "runas";
                try
                {
                    Process.Start(gamestart);
                }
                catch
                {
                    errexit("Failed to Start", "啟動失敗，請嘗試手動以系統管理員身分啟動遊戲。", 2);
                }
            }
        }

        // getOTP completed.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button3.Text = "獲取密碼";
            this.listView1.Enabled = true;
            if (e.Error != null)
            {
                this.textBox3.Text = "獲取失敗";
                errexit("Get OTP Fail", e.Error.Message, 2);
                return;
            }
            int index = (int)e.Result;

            if (index != -1)
            {
                this.textBox3.Text = this.otp;
                if (this.copyOrNot)
                    Clipboard.SetText(textBox3.Text);
                this.Text = "進行遊戲 - " + WebUtility.HtmlDecode(accountList[index].s_name);
            }
            else
                this.textBox3.Text = "獲取失敗";
        }

    }
}
