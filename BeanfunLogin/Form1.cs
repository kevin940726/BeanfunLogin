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
using Utility.ModifyRegistry;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private AccountManager accountManager = null;

        public BeanfunClient bfClient;

        public BeanfunClient.GamaotpClass gamaotpClass;

        private string service_code = "610074" , service_region = "T9";

        public main()
        {
            InitializeComponent();
            init();
            CheckForUpdate();
        }

        public void ShowToolTip(IWin32Window ui, string title, string des, int iniDelay = 2000, bool repeat = false)
        {
            if (Properties.Settings.Default.showTip || repeat)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.ToolTipTitle = title;
                toolTip.UseFading = true;
                toolTip.UseAnimation = true;
                toolTip.IsBalloon = true;
                toolTip.InitialDelay = iniDelay;

                toolTip.Show(string.Empty, ui, 3000);
                toolTip.Show(des, ui);
            }
        }

        public bool errexit(string msg, int method, string title = null)
        {
            switch (msg)
            {
                case "LoginNoResponse":
                    msg = "初始化失敗，請檢查網路連線。";
                    method = 0;
                    break;
                case "LoginNoSkey":
                    method = 0;
                    break;
                case "LoginNoAkey":
                    msg = "登入失敗，帳號或密碼錯誤。";
                    break;
                case "LoginNoAccountMatch":
                    msg = "登入失敗，無法取得帳號列表。";
                    break;
                case "LoginNoAccount":
                    msg = "登入失敗，找不到遊戲帳號。";
                    break;
                case "LoginNoResponseVakten":
                    msg = "登入失敗，與伺服器驗證失敗，請檢查是否安裝且已執行vakten程式。";
                    break;
                case "LoginUnknown":
                    msg = "登入失敗，請稍後再試";
                    method = 0;
                    break;
                case "OTPNoLongPollingKey":
                    if (Properties.Settings.Default.loginMethod == 5)
                        msg = "密碼獲取失敗，請檢查晶片卡是否插入讀卡機，且讀卡機運作正常。\n若仍出現此訊息，請嘗試重新登入。";
                    else
                    {
                        msg = "密碼獲取失敗，請嘗試重新登入。";
                        method = 1;
                    }
                    break;
                case "LoginNoReaderName":
                    msg = "登入失敗，找不到晶片卡或讀卡機，請檢查晶片卡是否插入讀卡機，且讀卡機運作正常。\n若還是發生此情形，請嘗試重新登入。";
                    break;
                case "LoginNoCardType":
                    msg = "登入失敗，晶片卡讀取失敗。";
                    break;
                case "LoginNoCardId":
                    msg = "登入失敗，找不到讀卡機。";
                    break;
                case "LoginNoOpInfo":
                    msg = "登入失敗，讀卡機讀取失敗。";
                    break;
                case "LoginNoEncryptedData":
                    msg = "登入失敗，晶片卡讀取失敗。";
                    break;
                case "OTPUnknown":
                    msg = "獲取密碼失敗，請嘗試重新登入。";
                    break;
                default:
                    break;
            }

            MessageBox.Show(msg, title);
            if (method == 0)
                Application.Exit();
            else if (method == 1)
            {
                BackToLogin();
            }
            return false;
        }

        public void BackToLogin()
        {
            panel1.SendToBack();
            panel2.BringToFront();
            Properties.Settings.Default.autoLogin = false;
            init();
        }

        public bool init()
        {
            try
            {
                this.Text = "BeanfunLogin - v" + Properties.Settings.Default.currentVersion.ToString().Insert(1, ".").Insert(3, ".");
                this.AcceptButton = this.button1;
                this.bfClient = null;
                this.accountManager = new AccountManager();

                bool res = accountManager.init();
                if (res == false)
                    errexit("帳號記錄初始化失敗，未知的錯誤。", 0);
                refreshAccountList();
                //Properties.Settings.Default.Reset(); //SetToDefault.                  

                // Handle settings.
                if (Properties.Settings.Default.rememberAccount == true)
                    this.accountInput.Text = Properties.Settings.Default.AccountID;
                if (Properties.Settings.Default.rememberPwd == true && Properties.Settings.Default.loginMethod != 2)
                {
                    this.rememberAccount.Enabled = false;
                    // Load password.
                    if (File.Exists("UserState.dat"))
                    {
                        Byte[] cipher = File.ReadAllBytes("UserState.dat");
                        string entropy = Properties.Settings.Default.entropy;
                        byte[] plaintext = ProtectedData.Unprotect(cipher, Encoding.UTF8.GetBytes(entropy), DataProtectionScope.CurrentUser);
                        this.passwdInput.Text = System.Text.Encoding.UTF8.GetString(plaintext);
                    }
                }
                if (Properties.Settings.Default.autoLogin == true && Properties.Settings.Default.loginMethod != 2 && Properties.Settings.Default.loginMethod != 4)
                {
                    this.UseWaitCursor = true;
                    this.panel2.Enabled = false;
                    this.button1.Text = "請稍後...";
                    this.backgroundWorker2.RunWorkerAsync(Properties.Settings.Default.loginMethod);
                }
                if (Properties.Settings.Default.gamePath == "")
                {
                    ModifyRegistry myRegistry = new ModifyRegistry();
                    myRegistry.BaseRegistryKey = Registry.CurrentUser;
                    myRegistry.SubKey = "Software\\Gamania\\MapleStory";
                    if (myRegistry.Read("Path") != "")
                        Properties.Settings.Default.gamePath = myRegistry.Read("Path");
                }

                this.loginMethodInput.SelectedIndex = Properties.Settings.Default.loginMethod;
                this.comboBox2.SelectedIndex = Properties.Settings.Default.loginGame;
                this.textBox3.Text = "";

                if (this.accountInput.Text == "")
                    this.ActiveControl = this.accountInput;
                else if (this.passwdInput.Text == "")
                    this.ActiveControl = this.passwdInput;

                // .NET textbox full mode bug.
                this.accountInput.ImeMode = ImeMode.OnHalf;
                this.passwdInput.ImeMode = ImeMode.OnHalf;
                return true;
            }
            catch { return errexit("初始化失敗，未知的錯誤。", 0); }
        }

        public void CheckForUpdate()
        {
            try
            {
                WebClient wc = new WebClient();
                string response = wc.DownloadString("https://raw.githubusercontent.com/kevin940726/BeanfunLogin/master/zh-TW.md");
                Regex regex = new Regex("Version (\\d\\.\\d\\.\\d)");
                if (!regex.IsMatch(response))
                    return;
                int latest = Convert.ToInt32(Regex.Replace(regex.Match(response).Groups[1].Value, "\\.", ""));
                if (latest > Properties.Settings.Default.currentVersion)
                {
                    Regex versionlog = new Regex(".*此版本更新(.*)### 目錄.*", RegexOptions.Multiline | RegexOptions.Singleline);
                    DialogResult result = MessageBox.Show("有新的版本(" + regex.Match(response).Groups[1].Value + ")可以下載，是否前往下載？\n(此對話窗只會顯示一次)\n\n此版本更新：" + versionlog.Match(response).Groups[1].Value, "檢查更新", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://github.com/kevin940726/BeanfunLogin/blob/master/zh-TW.md");
                    }
                    Properties.Settings.Default.currentVersion = latest;
                    Properties.Settings.Default.Save();
                }
            }
            catch { return; }
        }

        private void refreshAccountList()
        {
            string[] accArray = accountManager.getAccountList();
            accounts.Items.Clear();
            accounts.Items.AddRange(accArray);
        }

        // The login botton.
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.ping.IsBusy)
                this.ping.CancelAsync();
            if (this.rememberAccount.Checked == true)
                Properties.Settings.Default.AccountID = this.accountInput.Text;
            if (this.rememberAccPwd.Checked == true)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("UserState.dat", FileMode.Create)))
                {
                    // Create random entropy of 8 characters.
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var random = new Random();
                    string entropy = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

                    Properties.Settings.Default.entropy = entropy;
                    writer.Write(ciphertext(this.passwdInput.Text, entropy));
                }
            }
            else
            {
                Properties.Settings.Default.entropy = "";
                File.Delete("UserState.dat");
            }
            Properties.Settings.Default.Save();

            this.UseWaitCursor = true;
            this.panel2.Enabled = false;
            this.button1.Text = "請稍後...";
            this.backgroundWorker2.RunWorkerAsync(Properties.Settings.Default.loginMethod);
        }    

        // The get OTP button.
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.ping.IsBusy)
                this.ping.CancelAsync();
            if (listView1.SelectedItems.Count <= 0 || this.backgroundWorker2.IsBusy) return;
            if (Properties.Settings.Default.autoSelect == true)
            {
                Properties.Settings.Default.autoSelectIndex = listView1.SelectedItems[0].Index;
                Properties.Settings.Default.Save();
            }

            this.textBox3.Text = "獲取密碼中...";
            this.listView1.Enabled = false;
            this.button3.Enabled = false;
            this.backgroundWorker1.RunWorkerAsync(listView1.SelectedItems[0].Index);
        }

        // Ping to Beanfun website.
        private void ping_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Ping Worker";
            Debug.WriteLine("Ping Worker started");
            while (!this.ping.CancellationPending && Properties.Settings.Default.keepLogged)
            {
                Debug.WriteLine("Trying keep logged");
                try
                {
                    if (this.backgroundWorker1.IsBusy)
                    {
                        Debug.WriteLine("busy");
                        System.Threading.Thread.Sleep(1000 * 1);
                        continue;
                    }
                    this.bfClient.Ping();
                    System.Threading.Thread.Sleep(1000 * 60 * 2);
                }
                catch
                {
                    ;
                }
            }
        }

        // Building ciphertext by 3DES.
        private byte[] ciphertext(string plaintext, string key)
        {
            byte[] plainByte = Encoding.UTF8.GetBytes(plaintext);
            byte[] entropy = Encoding.UTF8.GetBytes(key);
            return ProtectedData.Protect(plainByte, entropy, DataProtectionScope.CurrentUser);
        }


        /* Handle other elements' statements. */
        private void BackToLogin_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackToLogin();
        }

        private void SetGamePath_ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "新楓之谷主程式 (MapleStory.exe)|MapleStory.exe|All files (*.*)|*.*";
            openFileDialog.Title = "Set MapleStory.exe Path.";
            openFileDialog.InitialDirectory = Properties.Settings.Default.gamePath;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                Properties.Settings.Default.gamePath = file;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
            {
                Properties.Settings.Default.autoLogin = true;
                this.rememberAccount.Checked = true;
                this.rememberAccPwd.Checked = true;
                this.rememberAccount.Enabled = false;
                this.rememberAccPwd.Enabled = false;
            }
            else
            {
                Properties.Settings.Default.autoLogin = false;
                this.rememberAccount.Enabled = true;
                this.rememberAccPwd.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rememberAccPwd.Checked == true)
            {
                Properties.Settings.Default.rememberPwd = true;
                this.rememberAccount.Checked = true;
                this.rememberAccPwd.Checked = true;
                this.rememberAccount.Enabled = false;
            }
            else
            {
                Properties.Settings.Default.rememberPwd = false;
                this.rememberAccount.Enabled = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (this.checkBox4.Checked == true && this.listView1.SelectedItems[0].Index != -1 && this.listView1.SelectedItems[0].Index <= this.bfClient.accountList.Count())
                {
                    Properties.Settings.Default.autoSelectIndex = this.listView1.SelectedItems[0].Index;
                    Properties.Settings.Default.autoSelect = true;
                }
                else
                    Properties.Settings.Default.autoSelect = false;
            }
            Properties.Settings.Default.Save();
        }

        private void textBox3_OnClick(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox3.Text == "獲取失敗") return;
            Clipboard.SetText(textBox3.Text);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Clipboard.SetText(this.bfClient.accountList[this.listView1.SelectedItems[0].Index].sacc);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                this.button3.Text = "獲取密碼";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.loginMethod = this.loginMethodInput.SelectedIndex;
            if (Properties.Settings.Default.loginMethod == 4)
            {
                this.rememberAccount.Location = new Point(49, 155);
                this.rememberAccPwd.Location = new Point(165, 155);
                this.label4.Visible = true;
                this.textBox4.Visible = true;
            }
            else
            {
                this.rememberAccount.Location = new Point(107, 127);
                this.rememberAccPwd.Location = new Point(107, 155);
                this.label4.Visible = false;
                this.textBox4.Visible = false;
            }
            if (Properties.Settings.Default.loginMethod == 2)
            {
                this.label3.Text = "安全密碼";
                this.bfClient = new BeanfunClient();
                this.gamaotpClass = this.bfClient.GetGamaotpPassCode(this.bfClient.GetSessionkey());
                if (this.bfClient.errmsg != null)
                    errexit(this.bfClient.errmsg, 2);
                else
                    errexit("通行密碼： " + this.gamaotpClass.motp, 2, "GAMAOTP");
            }         
            else if (Properties.Settings.Default.loginMethod == 3)
            {
                this.label3.Text = "安全密碼";
            }
            else if (Properties.Settings.Default.loginMethod == 5)
            {
                this.label3.Text = "PIN碼";
            }
            else
            {
                this.label3.Text = "密碼";
            }
        }

        private void keepLogged_CheckedChanged(object sender, EventArgs e)
        {
            if (keepLogged.Checked)
                if (!this.ping.IsBusy)
                    this.ping.RunWorkerAsync();
            else
                if (this.ping.IsBusy)
                    this.ping.CancelAsync();
            Properties.Settings.Default.Save();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox2.SelectedItem.ToString())
            {
                case "新楓之谷":
                    service_code = "610074";
                    service_region = "T9";
                    break;
                case "天堂":
                    service_code = "600035";
                    service_region = "T7";
                    break;
                case "天堂。健康伺服器":
                    service_code = "600037";
                    service_region = "T7";
                    break;
                case "天堂。免費伺服器":
                    service_code = "600041";
                    service_region = "BE";
                    break;
                case "絕對武力 online":
                    service_code = "610153";
                    service_region = "TN";
                    break;
                case "夢幻之星ONLINE 2":
                    service_code = "611187";
                    service_region = "BC";
                    break;
                case "英雄三國(HOK)":
                    service_code = "611413";
                    service_region = "BJ";
                    break;
                case "跑跑卡丁車":
                    service_code = "610096";
                    service_region = "TE";
                    break;
                case "艾爾之光":
                    service_code = "300148";
                    service_region = "AF";
                    break;
                case "新瑪奇mabinogi":
                    service_code = "600309";
                    service_region = "A2";
                    break;
                case "蠟筆小新 Online":
                    service_code = "611143";
                    service_region = "B8";
                    break;
                case "C9第九大陸":
                    service_code = "611154";
                    service_region = "B9";
                    break;
                case "瑪奇英雄傳":
                    service_code = "610670";
                    service_region = "DX";
                    break;
                case "爆爆王":
                    service_code = "610085";
                    service_region = "TC";
                    break;
                case "泡泡大亂鬥":
                    service_code = "610502";
                    service_region = "DN";
                    break;
                case "楓之谷體驗伺服器":
                    service_code = "610075";
                    service_region = "T9";
                    break;
                case "火爆小鬥士":
                    service_code = "610917";
                    service_region = "EN";
                    break;
                case "夢境":
                    service_code = "610648";
                    service_region = "D7";
                    break;
                case "戲谷麻將":
                    service_code = "610478";
                    service_region = "AX";
                    break;
                case "戲谷大老二":
                    service_code = "610481";
                    service_region = "AX";
                    break;
                case "戲谷自摸":
                    service_code = "610479";
                    service_region = "AX";
                    break;
                case "戲谷十三支":
                    service_code = "610482";
                    service_region = "AX";
                    break;
                case "戲谷柏青哥":
                    service_code = "610486";
                    service_region = "AX";
                    break;
                case "戲谷真接龍":
                    service_code = "610487";
                    service_region = "AX";
                    break;
                case "戲谷跑馬風雲":
                    service_code = "610478";
                    service_region = "AX";
                    break;
                case "戲谷德州撲克":
                    service_code = "610484";
                    service_region = "AX";
                    break;
                case "戲谷夢幻滿貫":
                    service_code = "610485";
                    service_region = "AX";
                    break;
                case "戲谷暗棋":
                    service_code = "610480";
                    service_region = "AX";
                    break;
                default:
                    service_code = "610074";
                    service_region = "T9";
                    break;
            }
            Properties.Settings.Default.loginGame = this.comboBox2.SelectedIndex;
            //Properties.Settings.Default.Save();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(accounts);
            selectedItems = accounts.SelectedItems;

            if (accounts.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    accountManager.removeAccount(accounts.GetItemText(selectedItems[i]));
                    refreshAccountList();
                }
            }
            //else
                 // MessageBox.Show("Debe seleccionar un email");
        }

        private void import_Click(object sender, EventArgs e)
        {
            bool res = accountManager.addAccount(accountInput.Text, passwdInput.Text, loginMethodInput.SelectedIndex);
            if (res == false)
                errexit("帳號記錄新增失敗",0);
            refreshAccountList();
        }

        private void export_Click(object sender, EventArgs e)
        {
            if(accounts.SelectedIndex != -1)
            {
                string account = accounts.SelectedItem.ToString();
                string passwd = accountManager.getPasswordByAccount(account);
                int method = accountManager.getMethodByAccount(account);

                if( passwd == null || method == -1 )
                {
                    errexit("帳號記錄讀取失敗。", 0);
                }

                accountInput.Text = account;
                passwdInput.Text = passwd;
                loginMethodInput.SelectedIndex = method;

                comboBox1_SelectedIndexChanged(null, null);
            }
        }

        

    }
}
