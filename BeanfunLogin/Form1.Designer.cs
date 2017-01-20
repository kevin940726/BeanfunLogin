namespace BeanfunLogin
{
    partial class main
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main));
            this.panel1 = new System.Windows.Forms.Panel();
            this.autoPaste = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.keepLogged = new System.Windows.Forms.CheckBox();
            this.getOtpButton = new System.Windows.Forms.Button();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.CharName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Account = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BackToLogin_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetGamePath_ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.wait_qrWorker_notify = new System.Windows.Forms.Label();
            this.gamaotp_challenge_code_output = new System.Windows.Forms.Label();
            this.gamaotp_label = new System.Windows.Forms.Label();
            this.export = new System.Windows.Forms.Button();
            this.import = new System.Windows.Forms.Button();
            this.delete = new System.Windows.Forms.Button();
            this.accounts = new System.Windows.Forms.ListBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.rememberAccount = new System.Windows.Forms.CheckBox();
            this.extraCodeInput = new System.Windows.Forms.TextBox();
            this.secPassLabel = new System.Windows.Forms.Label();
            this.loginMethodInput = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.rememberAccPwd = new System.Windows.Forms.CheckBox();
            this.passwdInput = new System.Windows.Forms.TextBox();
            this.passLabel = new System.Windows.Forms.Label();
            this.accountLabel = new System.Windows.Forms.Label();
            this.accountInput = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.qrcodeImg = new System.Windows.Forms.PictureBox();
            this.getOtpWorker = new System.ComponentModel.BackgroundWorker();
            this.loginWorker = new System.ComponentModel.BackgroundWorker();
            this.pingWorker = new System.ComponentModel.BackgroundWorker();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.Notification = new System.Windows.Forms.ToolTip(this.components);
            this.qrWorker = new System.ComponentModel.BackgroundWorker();
            this.qrCheckLogin = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qrcodeImg)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.autoPaste);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.keepLogged);
            this.panel1.Controls.Add(this.getOtpButton);
            this.panel1.Controls.Add(this.checkBox4);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(443, 252);
            this.panel1.TabIndex = 0;
            // 
            // autoPaste
            // 
            this.autoPaste.AutoSize = true;
            this.autoPaste.Checked = global::BeanfunLogin.Properties.Settings.Default.autoPaste;
            this.autoPaste.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoPaste.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "autoPaste", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.autoPaste.Location = new System.Drawing.Point(207, 24);
            this.autoPaste.Name = "autoPaste";
            this.autoPaste.Size = new System.Drawing.Size(75, 20);
            this.autoPaste.TabIndex = 10;
            this.autoPaste.Text = "自動輸入";
            this.autoPaste.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::BeanfunLogin.Properties.Settings.Default.opengame;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "opengame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(112, 24);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(99, 20);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "自動開啟遊戲";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // keepLogged
            // 
            this.keepLogged.AutoSize = true;
            this.keepLogged.Checked = global::BeanfunLogin.Properties.Settings.Default.keepLogged;
            this.keepLogged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.keepLogged.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "keepLogged", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.keepLogged.Enabled = false;
            this.keepLogged.Location = new System.Drawing.Point(291, 27);
            this.keepLogged.Name = "keepLogged";
            this.keepLogged.Size = new System.Drawing.Size(75, 20);
            this.keepLogged.TabIndex = 8;
            this.keepLogged.Text = "保持登入";
            this.keepLogged.UseVisualStyleBackColor = true;
            this.keepLogged.Visible = false;
            this.keepLogged.CheckedChanged += new System.EventHandler(this.keepLogged_CheckedChanged);
            // 
            // getOtpButton
            // 
            this.getOtpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.getOtpButton.Location = new System.Drawing.Point(25, 212);
            this.getOtpButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.getOtpButton.Name = "getOtpButton";
            this.getOtpButton.Size = new System.Drawing.Size(76, 27);
            this.getOtpButton.TabIndex = 6;
            this.getOtpButton.Text = "啟動遊戲";
            this.getOtpButton.UseVisualStyleBackColor = true;
            this.getOtpButton.Click += new System.EventHandler(this.getOtpButton_Click);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = global::BeanfunLogin.Properties.Settings.Default.autoSelect;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "autoSelect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox4.Location = new System.Drawing.Point(17, 24);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(99, 20);
            this.checkBox4.TabIndex = 5;
            this.checkBox4.Text = "下次自動選擇";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(107, 214);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(145, 23);
            this.textBox3.TabIndex = 3;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox3.Click += new System.EventHandler(this.textBox3_OnClick);
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CharName,
            this.Account});
            this.listView1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.LabelEdit = true;
            this.listView1.Location = new System.Drawing.Point(14, 46);
            this.listView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(254, 160);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // CharName
            // 
            this.CharName.Text = "帳號名稱";
            this.CharName.Width = 80;
            // 
            // Account
            // 
            this.Account.Text = "遊戲帳號(雙擊複製)";
            this.Account.Width = 180;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BackToLogin_ToolStripMenuItem,
            this.SetGamePath_ToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(443, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BackToLogin_ToolStripMenuItem
            // 
            this.BackToLogin_ToolStripMenuItem.Name = "BackToLogin_ToolStripMenuItem";
            this.BackToLogin_ToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.BackToLogin_ToolStripMenuItem.Text = "返回登入頁面";
            this.BackToLogin_ToolStripMenuItem.Click += new System.EventHandler(this.BackToLogin_ToolStripMenuItem_Click);
            // 
            // SetGamePath_ToolStripMenuItem1
            // 
            this.SetGamePath_ToolStripMenuItem1.Name = "SetGamePath_ToolStripMenuItem1";
            this.SetGamePath_ToolStripMenuItem1.Size = new System.Drawing.Size(91, 20);
            this.SetGamePath_ToolStripMenuItem1.Text = "設定遊戲路徑";
            this.SetGamePath_ToolStripMenuItem1.Click += new System.EventHandler(this.SetGamePath_ToolStripMenuItem1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.wait_qrWorker_notify);
            this.panel2.Controls.Add(this.gamaotp_challenge_code_output);
            this.panel2.Controls.Add(this.gamaotp_label);
            this.panel2.Controls.Add(this.export);
            this.panel2.Controls.Add(this.import);
            this.panel2.Controls.Add(this.delete);
            this.panel2.Controls.Add(this.accounts);
            this.panel2.Controls.Add(this.comboBox2);
            this.panel2.Controls.Add(this.rememberAccount);
            this.panel2.Controls.Add(this.extraCodeInput);
            this.panel2.Controls.Add(this.secPassLabel);
            this.panel2.Controls.Add(this.loginMethodInput);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.checkBox3);
            this.panel2.Controls.Add(this.rememberAccPwd);
            this.panel2.Controls.Add(this.passwdInput);
            this.panel2.Controls.Add(this.passLabel);
            this.panel2.Controls.Add(this.accountLabel);
            this.panel2.Controls.Add(this.accountInput);
            this.panel2.Controls.Add(this.loginButton);
            this.panel2.Controls.Add(this.qrcodeImg);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(443, 256);
            this.panel2.TabIndex = 25;
            // 
            // wait_qrWorker_notify
            // 
            this.wait_qrWorker_notify.AutoSize = true;
            this.wait_qrWorker_notify.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.wait_qrWorker_notify.Location = new System.Drawing.Point(47, 120);
            this.wait_qrWorker_notify.Name = "wait_qrWorker_notify";
            this.wait_qrWorker_notify.Size = new System.Drawing.Size(204, 24);
            this.wait_qrWorker_notify.TabIndex = 44;
            this.wait_qrWorker_notify.Text = "取得QRCode中 請稍後";
            this.wait_qrWorker_notify.Visible = false;
            // 
            // gamaotp_challenge_code_output
            // 
            this.gamaotp_challenge_code_output.AutoSize = true;
            this.gamaotp_challenge_code_output.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.gamaotp_challenge_code_output.ForeColor = System.Drawing.Color.Red;
            this.gamaotp_challenge_code_output.Location = new System.Drawing.Point(96, 135);
            this.gamaotp_challenge_code_output.Name = "gamaotp_challenge_code_output";
            this.gamaotp_challenge_code_output.Size = new System.Drawing.Size(0, 21);
            this.gamaotp_challenge_code_output.TabIndex = 42;
            // 
            // gamaotp_label
            // 
            this.gamaotp_label.AutoSize = true;
            this.gamaotp_label.Font = new System.Drawing.Font("微軟正黑體", 11.25F);
            this.gamaotp_label.Location = new System.Drawing.Point(30, 135);
            this.gamaotp_label.Name = "gamaotp_label";
            this.gamaotp_label.Size = new System.Drawing.Size(54, 19);
            this.gamaotp_label.TabIndex = 41;
            this.gamaotp_label.Text = "認證碼";
            this.gamaotp_label.Visible = false;
            // 
            // export
            // 
            this.export.Location = new System.Drawing.Point(275, 217);
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(50, 30);
            this.export.TabIndex = 40;
            this.export.Text = "讀取";
            this.export.UseVisualStyleBackColor = true;
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // import
            // 
            this.import.Location = new System.Drawing.Point(330, 217);
            this.import.Name = "import";
            this.import.Size = new System.Drawing.Size(50, 30);
            this.import.TabIndex = 39;
            this.import.Text = "儲存";
            this.import.UseVisualStyleBackColor = true;
            this.import.Click += new System.EventHandler(this.import_Click);
            // 
            // delete
            // 
            this.delete.Location = new System.Drawing.Point(385, 217);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(50, 30);
            this.delete.TabIndex = 38;
            this.delete.Text = "刪除";
            this.delete.UseVisualStyleBackColor = true;
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // accounts
            // 
            this.accounts.FormattingEnabled = true;
            this.accounts.ItemHeight = 16;
            this.accounts.Location = new System.Drawing.Point(275, 14);
            this.accounts.Name = "accounts";
            this.accounts.Size = new System.Drawing.Size(160, 196);
            this.accounts.TabIndex = 37;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(158, 17);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(110, 24);
            this.comboBox2.TabIndex = 36;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // rememberAccount
            // 
            this.rememberAccount.AutoSize = true;
            this.rememberAccount.Checked = global::BeanfunLogin.Properties.Settings.Default.rememberAccount;
            this.rememberAccount.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rememberAccount.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "rememberAccount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rememberAccount.Location = new System.Drawing.Point(77, 182);
            this.rememberAccount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rememberAccount.Name = "rememberAccount";
            this.rememberAccount.Size = new System.Drawing.Size(75, 20);
            this.rememberAccount.TabIndex = 30;
            this.rememberAccount.Text = "記住帳號";
            this.rememberAccount.UseVisualStyleBackColor = true;
            // 
            // extraCodeInput
            // 
            this.extraCodeInput.Location = new System.Drawing.Point(95, 133);
            this.extraCodeInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.extraCodeInput.Name = "extraCodeInput";
            this.extraCodeInput.PasswordChar = '*';
            this.extraCodeInput.Size = new System.Drawing.Size(145, 23);
            this.extraCodeInput.TabIndex = 35;
            this.extraCodeInput.Visible = false;
            // 
            // secPassLabel
            // 
            this.secPassLabel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.secPassLabel.Location = new System.Drawing.Point(21, 133);
            this.secPassLabel.Name = "secPassLabel";
            this.secPassLabel.Size = new System.Drawing.Size(69, 19);
            this.secPassLabel.TabIndex = 34;
            this.secPassLabel.Text = "安全密碼";
            this.secPassLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.secPassLabel.Visible = false;
            // 
            // loginMethodInput
            // 
            this.loginMethodInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.loginMethodInput.FormattingEnabled = true;
            this.loginMethodInput.Items.AddRange(new object[] {
            "一般登入",
            "金鑰一哥",
            "GAMAOTP",
            "OTP",
            "聰明鎖",
            "PLAYSAFE",
            "QRCode"});
            this.loginMethodInput.Location = new System.Drawing.Point(78, 17);
            this.loginMethodInput.Name = "loginMethodInput";
            this.loginMethodInput.Size = new System.Drawing.Size(74, 24);
            this.loginMethodInput.TabIndex = 33;
            this.loginMethodInput.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 33;
            this.label1.Text = "登入模式";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = global::BeanfunLogin.Properties.Settings.Default.autoLogin;
            this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "autoLogin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox3.Location = new System.Drawing.Point(77, 214);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(75, 20);
            this.checkBox3.TabIndex = 32;
            this.checkBox3.Text = "自動登入";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // rememberAccPwd
            // 
            this.rememberAccPwd.AutoSize = true;
            this.rememberAccPwd.Checked = global::BeanfunLogin.Properties.Settings.Default.rememberPwd;
            this.rememberAccPwd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BeanfunLogin.Properties.Settings.Default, "rememberPwd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rememberAccPwd.Location = new System.Drawing.Point(158, 182);
            this.rememberAccPwd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rememberAccPwd.Name = "rememberAccPwd";
            this.rememberAccPwd.Size = new System.Drawing.Size(75, 20);
            this.rememberAccPwd.TabIndex = 31;
            this.rememberAccPwd.Text = "記住帳密";
            this.rememberAccPwd.UseVisualStyleBackColor = true;
            this.rememberAccPwd.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // passwdInput
            // 
            this.passwdInput.Location = new System.Drawing.Point(95, 93);
            this.passwdInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.passwdInput.Name = "passwdInput";
            this.passwdInput.PasswordChar = '*';
            this.passwdInput.Size = new System.Drawing.Size(145, 23);
            this.passwdInput.TabIndex = 29;
            // 
            // passLabel
            // 
            this.passLabel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.passLabel.Location = new System.Drawing.Point(21, 93);
            this.passLabel.Name = "passLabel";
            this.passLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.passLabel.Size = new System.Drawing.Size(69, 19);
            this.passLabel.TabIndex = 28;
            this.passLabel.Text = "密碼";
            this.passLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // accountLabel
            // 
            this.accountLabel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.accountLabel.Location = new System.Drawing.Point(21, 52);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(69, 19);
            this.accountLabel.TabIndex = 27;
            this.accountLabel.Text = "帳號";
            this.accountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // accountInput
            // 
            this.accountInput.Location = new System.Drawing.Point(95, 52);
            this.accountInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.accountInput.Name = "accountInput";
            this.accountInput.Size = new System.Drawing.Size(145, 23);
            this.accountInput.TabIndex = 26;
            // 
            // loginButton
            // 
            this.loginButton.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.loginButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loginButton.Location = new System.Drawing.Point(165, 210);
            this.loginButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(87, 29);
            this.loginButton.TabIndex = 25;
            this.loginButton.Text = "登入";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // qrcodeImg
            // 
            this.qrcodeImg.Location = new System.Drawing.Point(46, 48);
            this.qrcodeImg.Name = "qrcodeImg";
            this.qrcodeImg.Size = new System.Drawing.Size(190, 190);
            this.qrcodeImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.qrcodeImg.TabIndex = 43;
            this.qrcodeImg.TabStop = false;
            this.qrcodeImg.Visible = false;
            // 
            // getOtpWorker
            // 
            this.getOtpWorker.WorkerReportsProgress = true;
            this.getOtpWorker.WorkerSupportsCancellation = true;
            this.getOtpWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.getOtpWorker_DoWork);
            this.getOtpWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.getOtpWorker_RunWorkerCompleted);
            // 
            // loginWorker
            // 
            this.loginWorker.WorkerReportsProgress = true;
            this.loginWorker.WorkerSupportsCancellation = true;
            this.loginWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loginWorker_DoWork);
            this.loginWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loginWorker_RunWorkerCompleted);
            // 
            // pingWorker
            // 
            this.pingWorker.WorkerReportsProgress = true;
            this.pingWorker.WorkerSupportsCancellation = true;
            this.pingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.pingWorker_DoWork);
            this.pingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.pingWorker_RunWorkerCompleted);
            // 
            // Tip
            // 
            this.Tip.AutoPopDelay = 5000;
            this.Tip.InitialDelay = 500;
            this.Tip.IsBalloon = true;
            this.Tip.ReshowDelay = 100;
            // 
            // Notification
            // 
            this.Notification.AutoPopDelay = 5000;
            this.Notification.InitialDelay = 0;
            this.Notification.ReshowDelay = 100;
            // 
            // qrWorker
            // 
            this.qrWorker.WorkerReportsProgress = true;
            this.qrWorker.WorkerSupportsCancellation = true;
            this.qrWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.qrWorker_DoWork);
            this.qrWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.qrWorker_RunWorkerCompleted);
            // 
            // qrCheckLogin
            // 
            this.qrCheckLogin.Interval = 2000;
            this.qrCheckLogin.Tick += new System.EventHandler(this.qrCheckLogin_Tick);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 252);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "main";
            this.Text = "BeanfunLogin - By Kai";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qrcodeImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader CharName;
        private System.Windows.Forms.ColumnHeader Account;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox rememberAccPwd;
        private System.Windows.Forms.CheckBox rememberAccount;
        private System.Windows.Forms.TextBox passwdInput;
        private System.Windows.Forms.Label passLabel;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.TextBox accountInput;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Button getOtpButton;
        private System.ComponentModel.BackgroundWorker getOtpWorker;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BackToLogin_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetGamePath_ToolStripMenuItem1;
        private System.Windows.Forms.ComboBox loginMethodInput;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker loginWorker;
        private System.Windows.Forms.TextBox extraCodeInput;
        private System.Windows.Forms.Label secPassLabel;
        private System.ComponentModel.BackgroundWorker pingWorker;
        private System.Windows.Forms.CheckBox keepLogged;
        private System.Windows.Forms.ToolTip Tip;
        private System.Windows.Forms.ToolTip Notification;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button export;
        private System.Windows.Forms.Button import;
        private System.Windows.Forms.Button delete;
        private System.Windows.Forms.ListBox accounts;
        private System.Windows.Forms.Label gamaotp_challenge_code_output;
        private System.Windows.Forms.Label gamaotp_label;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox autoPaste;
        private System.Windows.Forms.PictureBox qrcodeImg;
        private System.ComponentModel.BackgroundWorker qrWorker;
        private System.Windows.Forms.Label wait_qrWorker_notify;
        private System.Windows.Forms.Timer qrCheckLogin;


    }
}

