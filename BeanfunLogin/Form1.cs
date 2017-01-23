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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using CSharpAnalytics;

namespace BeanfunLogin
{
    enum LoginMethod : int {
        Regular = 0,
        Keypasco = 1,
        PlaySafe = 2,
        QRCode = 3
    };

    public partial class main : Form
    {
        private AccountManager accountManager = null;

        public BeanfunClient bfClient;

        public BeanfunClient.GamaotpClass gamaotpClass;
        public BeanfunClient.QRCodeClass qrcodeClass;

        private string service_code = "610074" , service_region = "T9";

        public List<GameService> gameList = new List<GameService>();

        private CSharpAnalytics.Activities.AutoTimedEventActivity timedActivity = null;

        public main()
        {
            AutoMeasurement.Instance = new WinFormAutoMeasurement();
            AutoMeasurement.DebugWriter = d => Debug.WriteLine(d);
            AutoMeasurement.Start(new MeasurementConfiguration("UA-75983216-4"));

            timedActivity = new CSharpAnalytics.Activities.AutoTimedEventActivity("FormLoad", Properties.Settings.Default.loginMethod.ToString());
            InitializeComponent();
            init();
            CheckForUpdate();

            if (this.timedActivity != null)
            {
                AutoMeasurement.Client.Track(this.timedActivity);
                this.timedActivity = null;
            }
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
            string originalMsg = msg;
            AutoMeasurement.Client.TrackException(msg);

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
                    if (Properties.Settings.Default.loginMethod == (int)LoginMethod.PlaySafe)
                        msg = "密碼獲取失敗，請檢查晶片卡是否插入讀卡機，且讀卡機運作正常。\n若仍出現此訊息，請嘗試重新登入。";
                    else
                    {
                        msg = "已從伺服器斷線，請重新登入。";
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
            this.Size = new System.Drawing.Size(459, 290);
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
                this.AcceptButton = this.loginButton;
                this.bfClient = null;
                this.accountManager = new AccountManager();

                bool res = accountManager.init();
                if (res == false)
                    errexit("帳號記錄初始化失敗，未知的錯誤。", 0);
                refreshAccountList();
                // Properties.Settings.Default.Reset(); //SetToDefault.                  

                // Handle settings.
                if (Properties.Settings.Default.rememberAccount == true)
                    this.accountInput.Text = Properties.Settings.Default.AccountID;
                if (Properties.Settings.Default.rememberPwd == true)
                {
                    this.rememberAccount.Enabled = false;
                    // Load password.
                    if (File.Exists("UserState.dat"))
                    {
                        try
                        {
                            Byte[] cipher = File.ReadAllBytes("UserState.dat");
                            string entropy = Properties.Settings.Default.entropy;
                            byte[] plaintext = ProtectedData.Unprotect(cipher, Encoding.UTF8.GetBytes(entropy), DataProtectionScope.CurrentUser);
                            this.passwdInput.Text = System.Text.Encoding.UTF8.GetString(plaintext);
                        }
                        catch
                        {
                            File.Delete("UserState.dat");
                        }
                    }
                }
                if (Properties.Settings.Default.autoLogin == true)
                {
                    this.UseWaitCursor = true;
                    this.panel2.Enabled = false;
                    this.loginButton.Text = "請稍後...";
                    this.loginWorker.RunWorkerAsync(Properties.Settings.Default.loginMethod);
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
            catch (Exception e)
            { 
                return errexit("初始化失敗，未知的錯誤。" + e.Message, 0); 
            }
        }

        public class GameService
        {
            public string name { get; set; }
            public string service_code { get; set; }
            public string service_region { get; set; }

            public GameService(string name, string service_code, string service_region)
            {
                this.name = name;
                this.service_code = service_code;
                this.service_region = service_region;
            }
        }

        public void CheckForUpdate()
        {
            try
            {
                WebClient wc = new WebClient();
                string res = Encoding.UTF8.GetString(wc.DownloadData("http://tw.beanfun.com/game_zone/"));
                Regex reg = new Regex("Services.ServiceList = (.*);");
                if (reg.IsMatch(res))
                {
                    string json = reg.Match(res).Groups[1].Value;
                    JObject o = JObject.Parse(json);
                    foreach (var game in o["Rows"])
                    {
                        Debug.Write(game["serviceCode"]);
                        this.comboBox2.Items.Add((string)game["ServiceFamilyName"]);
                        gameList.Add(new GameService((string)game["ServiceFamilyName"], (string)game["ServiceCode"], (string)game["ServiceRegion"]));
                    }
                }

                this.comboBox2.SelectedIndex = Properties.Settings.Default.loginGame;

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
        private void loginButton_Click(object sender, EventArgs e)
        {

            foreach (ListViewItem item in listView1.Items)
                item.BackColor = DefaultBackColor;
            if (this.pingWorker.IsBusy)
            {
                this.pingWorker.CancelAsync();
            }
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

            this.loginButton.Text = "請稍後...";
            timedActivity = new CSharpAnalytics.Activities.AutoTimedEventActivity("Login", Properties.Settings.Default.loginMethod.ToString());
            this.loginWorker.RunWorkerAsync(Properties.Settings.Default.loginMethod);
        }    

        // The get OTP button.
        private void getOtpButton_Click(object sender, EventArgs e)
        {
            if (this.pingWorker.IsBusy)
            {
                this.pingWorker.CancelAsync();
            }
            if (listView1.SelectedItems.Count <= 0 || this.loginWorker.IsBusy) return;
            if (Properties.Settings.Default.autoSelect == true)
            {
                Properties.Settings.Default.autoSelectIndex = listView1.SelectedItems[0].Index;
                Properties.Settings.Default.Save();
            }

            this.textBox3.Text = "獲取密碼中...";
            this.listView1.Enabled = false;
            this.getOtpButton.Enabled = false;
            timedActivity = new CSharpAnalytics.Activities.AutoTimedEventActivity("GetOTP", Properties.Settings.Default.loginMethod.ToString());
            this.getOtpWorker.RunWorkerAsync(listView1.SelectedItems[0].Index);
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

            AutoMeasurement.Client.TrackEvent("set game path", "set game path");
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

            AutoMeasurement.Client.TrackEvent(this.checkBox3.Checked ? "autoLoginOn" : "autoLoginOff", "loginCheckbox");
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

            AutoMeasurement.Client.TrackEvent(this.rememberAccPwd.Checked ? "rememberPwdOn" : "rememberPwdOff", "rememberPwdCheckbox");
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

            AutoMeasurement.Client.TrackEvent(this.checkBox4.Checked ? "autoSelectOn" : "autoSelectOff", "autoSelectCheckbox");
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
                this.getOtpButton.Text = "獲取密碼";
        }

        // login method changed event
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrCheckLogin.Enabled = false;

            accountInput.Visible = true;
            accountLabel.Visible = true;

            passLabel.Visible = true;
            passwdInput.Visible = true;

            extraCodeInput.Visible = false;
            gamaotp_label.Visible = false;
            secPassLabel.Visible = false;

            qrcodeImg.Visible = false;

            rememberAccount.Visible = true;
            rememberAccPwd.Visible = true;
            checkBox3.Visible = true;
            loginButton.Visible = true;

            wait_qrWorker_notify.Visible = false;

            this.gamaotp_challenge_code_output.Text = "";

            Properties.Settings.Default.loginMethod = this.loginMethodInput.SelectedIndex;

            if (Properties.Settings.Default.loginMethod == (int)LoginMethod.PlaySafe)
            {
                this.passLabel.Text = "PIN碼";
            }
            else if (Properties.Settings.Default.loginMethod == (int)LoginMethod.QRCode)
            {
                accountInput.Visible = false;
                accountLabel.Visible = false;

                passLabel.Visible = false;
                passwdInput.Visible = false;

                qrcodeImg.Visible = true;

                rememberAccount.Visible = false;
                rememberAccPwd.Visible = false;
                checkBox3.Visible = false;
                loginButton.Visible = false;
                qrcodeImg.Image = null;
                wait_qrWorker_notify.Text = "取得QRCode中 請稍後";
                wait_qrWorker_notify.Visible = true;

                this.qrWorker.RunWorkerAsync(null);
                this.loginMethodInput.Enabled = false;
            }
            else
            {
                this.passLabel.Text = "密碼";
            }
        }

        private void keepLogged_CheckedChanged(object sender, EventArgs e)
        {
            if (keepLogged.Checked)
                if (!this.pingWorker.IsBusy)
                    this.pingWorker.RunWorkerAsync();
            else
                    if (this.pingWorker.IsBusy)
                    {
                        this.pingWorker.CancelAsync();
                    }
            Properties.Settings.Default.Save();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.loginGame = this.comboBox2.SelectedIndex;
            try
            {
                service_code = gameList[this.comboBox2.SelectedIndex].service_code;
                service_region = gameList[this.comboBox2.SelectedIndex].service_region;
            }
            catch
            {

            }
            
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

            AutoMeasurement.Client.TrackEvent("remove", "accountMananger");
        }

        private void import_Click(object sender, EventArgs e)
        {
            bool res = accountManager.addAccount(accountInput.Text, passwdInput.Text, loginMethodInput.SelectedIndex);
            if (res == false)
                errexit("帳號記錄新增失敗",0);
            refreshAccountList();

            AutoMeasurement.Client.TrackEvent("add", "accountMananger");
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

                AutoMeasurement.Client.TrackEvent("fill", "accountMananger");
            }
        }

        private void autoPaste_CheckedChanged(object sender, EventArgs e)
        {
            AutoMeasurement.Client.TrackEvent(this.autoPaste.Checked ? "autoPasteOn" : "autoPasteOff", "autoPasteCheckbox");
        }

        private void rememberAccount_CheckedChanged(object sender, EventArgs e)
        {
            AutoMeasurement.Client.TrackEvent(this.rememberAccount.Checked ? "rememberAccountOn" : "rememberAccountOff", "rememberAccountCheckbox");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();

            AutoMeasurement.Client.TrackEvent(this.checkBox1.Checked ? "autoLaunchOn" : "autoLaunchOff", "autoLaunchCheckbox");
        }







    }
}
