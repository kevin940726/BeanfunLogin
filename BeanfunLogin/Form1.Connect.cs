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
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using CSharpAnalytics;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private string otp;

        // Login do work.
        private void loginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.pingWorker.IsBusy)
                Thread.Sleep(137);
            Debug.WriteLine("loginWorker starting");
            Thread.CurrentThread.Name = "Login Worker";
            e.Result = "";
            try
            {
                if (Properties.Settings.Default.loginMethod != (int)LoginMethod.QRCode)
                    this.bfClient = new BeanfunClient();
                this.bfClient.Login(this.accountInput.Text, this.passwdInput.Text, Properties.Settings.Default.loginMethod, this.qrcodeClass, this.service_code, this.service_region);
                if (this.bfClient.errmsg != null)
                    e.Result = this.bfClient.errmsg;
                else
                    e.Result = null;
            }
            catch (Exception ex)
            {
                e.Result = "登入失敗，未知的錯誤。\n\n" + ex.Message + "\n" + ex.StackTrace; 
            }
        }

        // Login completed.
        private void loginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Properties.Settings.Default.GAEnabled && this.timedActivity != null)
            {
                AutoMeasurement.Client.Track(this.timedActivity);
                this.timedActivity = null;
            }
            if (Properties.Settings.Default.keepLogged && !this.pingWorker.IsBusy)
                this.pingWorker.RunWorkerAsync();
            Debug.WriteLine("loginWorker end");
            this.panel2.Enabled = true;
            this.UseWaitCursor = false;
            this.loginButton.Text = "登入";
            if (e.Error != null)
            {
                errexit(e.Error.Message, 1);
                return;
            }
            if ((string)e.Result != null)
            {
                errexit((string)e.Result, 1);
                return;
            }

            try
            {
                listView1.Items.Clear();
                foreach (var account in this.bfClient.accountList)
                {
                    string[] row = { WebUtility.HtmlDecode(account.sname), account.sacc };
                    var listViewItem = new ListViewItem(row);
                    this.listView1.Items.Add(listViewItem);
                }

                // Handle panel switching.
                this.ActiveControl = null;
                this.Size = new System.Drawing.Size(300, 290);
                this.panel2.SendToBack();
                this.panel1.BringToFront();
                this.AcceptButton = this.getOtpButton;
                if (Properties.Settings.Default.autoSelectIndex < this.listView1.Items.Count)
                    this.listView1.Items[Properties.Settings.Default.autoSelectIndex].Selected = true;
                this.listView1.Select();
                if (Properties.Settings.Default.autoSelect == true && Properties.Settings.Default.autoSelectIndex < this.bfClient.accountList.Count())
                {
                    if (this.pingWorker.IsBusy)
                    {
                        this.pingWorker.CancelAsync();
                    }
                    this.textBox3.Text = "獲取密碼中...";
                    this.listView1.Enabled = false;
                    this.getOtpButton.Enabled = false;
                    timedActivity = new CSharpAnalytics.Activities.AutoTimedEventActivity("GetOTP", Properties.Settings.Default.loginMethod.ToString());
                    if (Properties.Settings.Default.GAEnabled) {
                        AutoMeasurement.Client.TrackEvent("GetOTP" + Properties.Settings.Default.loginMethod.ToString(), "GetOTP"); 
                    }
                    this.getOtpWorker.RunWorkerAsync(Properties.Settings.Default.autoSelectIndex);
                }
                if (Properties.Settings.Default.keepLogged && !this.pingWorker.IsBusy)
                    this.pingWorker.RunWorkerAsync();
                ShowToolTip(listView1, "步驟1", "選擇欲開啟的遊戲帳號，雙擊以複製帳號。");
                ShowToolTip(getOtpButton, "步驟2", "按下以在右側產生並自動複製密碼，至遊戲中貼上帳密登入。");
                Tip.SetToolTip(getOtpButton, "點擊獲取密碼");
                Tip.SetToolTip(listView1, "雙擊即自動複製");
                Tip.SetToolTip(textBox3, "點擊一次即自動複製");
                Properties.Settings.Default.showTip = false;
                Properties.Settings.Default.Save();
            }
            catch
            {
                errexit("登入失敗，無法取得帳號列表。", 1);
            }
            
        }

        // getOTP do work.
        private void getOtpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.pingWorker.IsBusy)
                Thread.Sleep(133);
            Debug.WriteLine("getOtpWorker start");
            Thread.CurrentThread.Name = "GetOTP Worker";
            int index = (int)e.Argument;
            Debug.WriteLine("Count = " + this.bfClient.accountList.Count + " | index = " + index);
            if (this.bfClient.accountList.Count <= index)
            {
                return;
            }
            Debug.WriteLine("call GetOTP");
            this.otp = this.bfClient.GetOTP(Properties.Settings.Default.loginMethod, this.bfClient.accountList[index], this.service_code, this.service_region);
            Debug.WriteLine("call GetOTP done");
            if (this.otp == null)
                e.Result = -1;
            else 
            {
                e.Result = index;
                if (false == Properties.Settings.Default.opengame)
                {
                    Debug.WriteLine("no open game");
                    return;
                }

                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "MapleStory")
                    {
                        Debug.WriteLine("find game");
                        return;
                    }
                }
                
                try
                {
                    Debug.WriteLine("try open game");
                    if (File.Exists(Properties.Settings.Default.gamePath))
                        Process.Start(Properties.Settings.Default.gamePath, "tw.login.maplestory.gamania.com 8484 BeanFun " + this.bfClient.accountList[index].sacc + " " + this.otp);
                    Debug.WriteLine("try open game done");
                }
                catch
                {
                    errexit("啟動失敗，請嘗試手動以系統管理員身分啟動遊戲。", 2);
                }
            }

            return;
        }

        // getOTP completed.
        private void getOtpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Properties.Settings.Default.GAEnabled && this.timedActivity != null)
            {
                AutoMeasurement.Client.Track(this.timedActivity);
                this.timedActivity = null;
            }

            const int VK_TAB = 0x09;
            const byte VK_CONTROL = 0x11;
            const int VK_V = 0x56;
            const int VK_ENTER = 0x0d;
            const byte KEYEVENTF_EXTENDEDKEY = 0x1;
            const byte KEYEVENTF_KEYUP = 0x2;

            Debug.WriteLine("getOtpWorker end");
            this.getOtpButton.Text = "獲取密碼";
            this.listView1.Enabled = true;
            this.getOtpButton.Enabled = true;
            if (e.Error != null)
            {
                this.textBox3.Text = "獲取失敗";
                errexit(e.Error.Message, 2);
                return;
            }
            int index = (int)e.Result;

            if (index == -1)
            {
                this.textBox3.Text = "獲取失敗";
                errexit(this.bfClient.errmsg, 2);
            }
            else
            {
                int accIndex = listView1.SelectedItems[0].Index;
                string acc = this.bfClient.accountList[index].sacc;
                this.Text = "進行遊戲 - " + WebUtility.HtmlDecode(this.bfClient.accountList[index].sname);

                try
                {
                    Clipboard.SetText(acc);
                }
                catch
                {
                    return;
                }

                IntPtr hWnd;
                if (autoPaste.Checked == true && (hWnd = WindowsAPI.FindWindow(null, "MapleStory")) != IntPtr.Zero)
                {
                    WindowsAPI.SetForegroundWindow(hWnd);

                    WindowsAPI.keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_EXTENDEDKEY, 0);
                    WindowsAPI.keybd_event(VK_V, 0x9e, 0, 0);
                    Thread.Sleep(100);
                    WindowsAPI.keybd_event(VK_V, 0x9e, KEYEVENTF_KEYUP, 0);
                    Thread.Sleep(100);
                    WindowsAPI.keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);

                    WindowsAPI.keybd_event(VK_TAB, 0, KEYEVENTF_EXTENDEDKEY, 0);
                    WindowsAPI.keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
                }

                this.textBox3.Text = this.otp;
                try
                {
                    Clipboard.SetText(textBox3.Text);
                }
                catch
                {
                    return;
                }

                Thread.Sleep(150);

                if (autoPaste.Checked == true && (hWnd = WindowsAPI.FindWindow(null, "MapleStory")) != IntPtr.Zero)
                {
                    WindowsAPI.keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_EXTENDEDKEY, 0);
                    WindowsAPI.keybd_event(VK_V, 0x9e, 0, 0);
                    Thread.Sleep(100);
                    WindowsAPI.keybd_event(VK_V, 0x9e, KEYEVENTF_KEYUP, 0);
                    Thread.Sleep(100);
                    WindowsAPI.keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);

                    WindowsAPI.keybd_event(VK_ENTER, 0, 0, 0);
                    WindowsAPI.keybd_event(VK_ENTER, 0, KEYEVENTF_KEYUP, 0);

                    listView1.Items[accIndex].BackColor = Color.Green;
                    listView1.Items[accIndex].Selected = false;
                }


            }

            if (Properties.Settings.Default.keepLogged && !this.pingWorker.IsBusy)
                this.pingWorker.RunWorkerAsync();
        }

        // Ping to Beanfun website.
        private void pingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "ping Worker";
            Debug.WriteLine("pingWorker start");
            const int WaitSecs = 60; // 1min

            while (Properties.Settings.Default.keepLogged)
            {
                if (this.pingWorker.CancellationPending)
                {
                    Debug.WriteLine("break duo to cancel");
                    break;
                }

                if (this.getOtpWorker.IsBusy || this.loginWorker.IsBusy)
                {
                    Debug.WriteLine("ping.busy sleep 1s");
                    System.Threading.Thread.Sleep(1000 * 1);
                    continue;
                }

                if(this.bfClient != null)
                    this.bfClient.Ping();

                for (int i = 0; i < WaitSecs; ++i)
                {
                    if (this.pingWorker.CancellationPending)
                        break;
                    System.Threading.Thread.Sleep(1000 * 1);
                }
            }
        }
        
        private void pingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("ping.done");
        }

        private void qrWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.bfClient = new BeanfunClient();
            string skey = this.bfClient.GetSessionkey();
            this.qrcodeClass = this.bfClient.GetQRCodeValue(skey);
        }

        private void qrWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.loginMethodInput.Enabled = true;
            wait_qrWorker_notify.Visible = false;
            if (this.qrcodeClass == null)
                wait_qrWorker_notify.Text = "QRCode取得失敗";
            else
            {
                qrcodeImg.Image = qrcodeClass.bitmap;
                qrCheckLogin.Enabled = true;
            }
        }

        private void qrCheckLogin_Tick(object sender, EventArgs e)
        {
            if (this.qrcodeClass == null)
            {
                MessageBox.Show("QRCode not get yet");
                return;
            }
            int res = this.bfClient.QRCodeCheckLoginStatus(this.qrcodeClass);
            if (res != 0)
                this.qrCheckLogin.Enabled = false;
            if (res == 1)
            {
                loginButton_Click(null, null);
            }
            if (res == -2)
            {
                comboBox1_SelectedIndexChanged(null, null);
            }
        }
    }
}
