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

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private SpWebClient web;
        private string skey;

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
                if (Properties.Settings.Default.gamePath == "")
                {
                    ModifyRegistry myRegistry = new ModifyRegistry();
                    myRegistry.BaseRegistryKey = Registry.CurrentUser;
                    myRegistry.SubKey = "Software\\Gamania\\MapleStory";
                    if (myRegistry.Read("Path") != "")
                        Properties.Settings.Default.gamePath = myRegistry.Read("Path");
                }

                if (this.textBox1.Text == "")
                    this.ActiveControl = this.textBox1;
                else if (this.textBox2.Text == "")
                    this.ActiveControl = this.textBox2;
                return true;
            }
            catch { return errexit("Initial Error", "初始化失敗，未知的錯誤。\nUnknown error.", 0); }
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

        // The  get OTP button. 
        private void textBox3_OnClick(object sender, EventArgs e)
        {
            if (textBox3.Text == "") return; 
            Clipboard.SetText(textBox3.Text);
            this.button3.Text = "已複製";
        }

        /* Handle other elements' statements. */
        private void BackToLogin_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.SendToBack();
            panel2.BringToFront();
            init();
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
            this.listView1.Enabled = false;
            this.copyOrNot = true;
            this.backgroundWorker1.RunWorkerAsync(listView1.SelectedItems[0].Index);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Clipboard.SetText(accountList[this.listView1.SelectedItems[0].Index].s_acc);
                this.textBox3.Text = "獲取密碼中...";
                this.listView1.Enabled = false;
                this.copyOrNot = false;
                this.backgroundWorker1.RunWorkerAsync(listView1.SelectedItems[0].Index);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                this.button3.Text = "獲取密碼";
        } 

    }
}
