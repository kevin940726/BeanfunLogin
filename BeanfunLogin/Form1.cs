using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private SpWebClient web;
        private string skey;
        private string viewstate;
        private string eventvalidation;
        private string akey;
        private string webtoken;

        private string longPollingKey;
        private string secretCode;
        private string s_code = "610074";
        private string s_region = "T9";

        public List<AccountListClass> accountList;

        public main()
        {
            InitializeComponent();
            init();
        }

        public bool errexit(string title, string msg, int method)
        {
            MessageBox.Show(msg, title);
            if (method == 0)
                Application.Exit();
            else if (method == 1)
            {
                this.panel1.SendToBack();
                this.panel2.BringToFront();
                init();
            }
            return false;
        }

        private string getCurrentTime()
        {
            DateTime date = DateTime.Now;
            return date.ToString("yyyyMMddHHmmss.fff");
        }

        public bool init()
        {
            try
            {
                this.Text = "BeanfunLogin - By Kai";
                this.AcceptButton = this.button1;
                this.web = new SpWebClient(new CookieContainer());
                this.web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36");
                
                string response = this.web.DownloadString("http://tw.beanfun.com/beanfun_block/bflogin/default.aspx?service=999999_T0");
                if (response == "")
                    return errexit("Initial Error", "初始化失敗，請檢查網路連線。\nInternet error.", 0);
                response = this.web.ResponseUri.ToString();
                Regex regex = new Regex("skey=(.*)&display");
                if (!regex.IsMatch(response))
                    return errexit("Initial Error", "初始化失敗。\nCannot find session key.", 0);
                this.skey = regex.Match(response).Groups[1].Value;

                if (Properties.Settings.Default.rememberAccount == true)
                    this.textBox1.Text = Properties.Settings.Default.AccountID;
                if (Properties.Settings.Default.rememberPwd == true)
                {
                    this.checkBox1.Enabled = false;
                    // Load password.
                    if (File.Exists("UserState.dat"))
                    {
                        Byte[] cipher = File.ReadAllBytes("UserState.dat");
                        string entropy = Properties.Settings.Default.entropy;
                        byte[] plaintext = ProtectedData.Unprotect(cipher, Encoding.UTF8.GetBytes(entropy), DataProtectionScope.CurrentUser);
                        this.textBox2.Text = System.Text.Encoding.UTF8.GetString(plaintext);
                    }
                }
                if (Properties.Settings.Default.autoLogin == true)
                {
                    if (login(this.textBox1.Text, this.textBox2.Text))
                        this.panel2.SendToBack();
                }
                if (this.textBox1.Text == "")
                    this.ActiveControl = this.textBox1;
                else if (this.textBox2.Text == "")
                    this.ActiveControl = this.textBox2;
                return true;
            }
            catch { return errexit("Initial Error", "初始化失敗，未知的錯誤。\nUnknown error.", 0); }
        }

        private bool login(string userID, string pass)
        {
            try
            {
                string response = this.web.DownloadString("https://tw.newlogin.beanfun.com/login/id-pass_form.aspx?skey=" + skey);
                Regex regex = new Regex("id=\"__VIEWSTATE\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return errexit("Login Fail", "登入失敗。\nCannot find \"__VIEWSTATE\".", 1);
                this.viewstate = regex.Match(response).Groups[1].Value;
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*)\" />");
                if (!regex.IsMatch(response))
                    return errexit("Login Fail", "登入失敗。\nCannot find \"__EVENTVALIDATION\".", 1);
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
                    return errexit("Login Fail", "登入失敗。\nNo response for webtoken.", 1);
                regex = new Regex("akey=(.*)");
                if (!regex.IsMatch(this.web.ResponseUri.ToString()))
                    return errexit("Login Fail", "登入失敗，帳號或密碼錯誤。\nNo response for authentication key.", 1);
                this.akey = regex.Match(this.web.ResponseUri.ToString()).Groups[1].Value;

                payload = new NameValueCollection();
                payload.Add("SessionKey", this.skey);
                payload.Add("AuthKey", this.akey);
                response = Encoding.UTF8.GetString(this.web.UploadValues("https://tw.beanfun.com/beanfun_block/bflogin/return.aspx", payload));
                response = this.web.DownloadString("http://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start.aspx%3Fservice_code_and_region%3D610074_T9&web_token=" + webtoken, Encoding.UTF8);
                if (response == "")
                    return errexit("Login Fail", "登入失敗，無法取得帳號列表。\nNo response for account list.", 1);

                // Add account list to ListView.
                regex = new Regex("<div id=\"(\\w+)\" sn=\"(\\d+)\" name=\"([^\"]+)\"");
                this.accountList = new List<AccountListClass>();
                foreach (Match match in regex.Matches(response))
                {
                    this.accountList.Add(new AccountListClass(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
                }
                if (this.accountList.Count == 0)
                    return errexit("Login Fail", "登入失敗，找不到遊戲帳號。\nNo account found.", 1);
                listView1.Items.Clear();
                foreach (AccountListClass account in accountList)
                {
                    string[] row = { WebUtility.HtmlDecode(account.s_name), account.s_acc };
                    var listViewItem = new ListViewItem(row);
                    this.listView1.Items.Add(listViewItem);
                }
               
                // Handle panel switching.
                this.ActiveControl = null;
                this.panel1.BringToFront();
                this.AcceptButton = this.button3;
                if (Properties.Settings.Default.autoSelect == true)
                {
                    getOTP(Properties.Settings.Default.autoSelectIndex);
                    Clipboard.SetText(this.textBox3.Text);
                }
                return true;
            }
            catch {
                errexit("Login Fail", "登入失敗，未知的錯誤。\nUnknown Error.", 1); 
                return false;
            }
        }

        private bool getOTP(int index)
        {
            try
            {
                this.textBox3.Text = "獲取密碼中...";
                this.listView1.Enabled = false;
                string response = this.web.DownloadString("http://tw.beanfun.com/beanfun_block/auth.aspx?channel=game_zone&page_and_query=game_start_step2.aspx%3Fservice_code%3D" + s_code + "%26service_region%3D" + s_region + "%26sotp%3D" + accountList[index].s_otp + "&web_token=" + webtoken);
                if (response == "")
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nNo response by \"s_otp\".", 2);
                Regex regex = new Regex("GetResultByLongPolling&key=(.*)\"");
                if (!regex.IsMatch(response))
                    return errexit("Get OTP Fail", "密碼獲取失敗。\nCannot find \"longPullingKey\".", 2);
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

                this.button3.Text = "獲取密碼";
                this.textBox3.Text = DecryptDES(plain, key);
                this.listView1.Enabled = true;
                this.Text = "進行遊戲 - " + WebUtility.HtmlDecode(accountList[index].s_name);
                return true;
            }
            catch {
                this.listView1.Enabled = true;
                return errexit("Get OTP Fail", "密碼獲取失敗，未知的錯誤。\nUnknown Error.", 2);              
            }
        }

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
            catch 
            {
                errexit("Decrypt Fail", "密碼獲取失敗，密碼解析失敗。\nFailed to decrypt the OTP.", 1);
                return "";
            }
        }

        // The login botton.
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
                Properties.Settings.Default.AccountID = this.textBox1.Text;
            if (this.checkBox2.Checked == true)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("UserState.dat", FileMode.Create)))
                {
                    // Create random entropy of 8 characters.
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var random = new Random();
                    string entropy = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

                    Properties.Settings.Default.entropy = entropy;
                    writer.Write(ciphertext(this.textBox2.Text, entropy));
                }
            }
            else
            {
                Properties.Settings.Default.entropy = "";
                File.Delete("UserState.dat");
            }
            Properties.Settings.Default.Save();
            
            if (login(this.textBox1.Text, this.textBox2.Text))
                this.panel2.SendToBack();
        }

        // Building ciphertext by 3DES.
        private byte[] ciphertext(string plaintext, string key)
        {
            byte[] plainByte = Encoding.UTF8.GetBytes(plaintext);
            byte[] entropy = Encoding.UTF8.GetBytes(key);
            return ProtectedData.Protect(plainByte, entropy, DataProtectionScope.CurrentUser);
        }

        /* Handle elements' statements. */ 
        private void textBox3_OnClick(object sender, EventArgs e)
        {
            if (textBox3.Text == "") return; 
            Clipboard.SetText(textBox3.Text);
            this.button3.Text = "已複製";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.SendToBack();
            panel2.BringToFront();
            init();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
            {
                Properties.Settings.Default.autoLogin = true;
                this.checkBox1.Checked = true;
                this.checkBox2.Checked = true;
                this.checkBox1.Enabled = false;
                this.checkBox2.Enabled = false;
            }
            else
            {
                Properties.Settings.Default.autoLogin = false;
                this.checkBox1.Enabled = true;
                this.checkBox2.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                Properties.Settings.Default.rememberPwd = true;
                this.checkBox1.Checked = true;
                this.checkBox2.Checked = true;
                this.checkBox1.Enabled = false;
            }
            else
            {
                Properties.Settings.Default.rememberPwd = false;
                this.checkBox1.Enabled = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (this.checkBox4.Checked == true && this.listView1.SelectedItems[0].Index != -1 && this.listView1.SelectedItems[0].Index <= accountList.Count)
                {
                    Properties.Settings.Default.autoSelectIndex = this.listView1.SelectedItems[0].Index;
                    Properties.Settings.Default.autoSelect = true;
                }
                else if (this.checkBox4.Checked == false)
                    Properties.Settings.Default.autoSelect = false;
            }
            Properties.Settings.Default.Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            if (Properties.Settings.Default.autoSelect == true)
            {
                Properties.Settings.Default.autoSelectIndex = listView1.SelectedItems[0].Index;
                Properties.Settings.Default.Save();
            }
            this.textBox3.Text = "獲取密碼中...";
            getOTP(listView1.SelectedItems[0].Index);
            Clipboard.SetText(this.textBox3.Text);
            this.button3.Text = "獲取密碼";
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Clipboard.SetText(accountList[this.listView1.SelectedItems[0].Index].s_acc);
                getOTP(this.listView1.SelectedItems[0].Index);
                this.button3.Text = "密碼";
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                this.button3.Text = "獲取密碼";
        }

    }
}
