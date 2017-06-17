namespace BeanfunLogin
{
    partial class FormAccRecovery
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_export = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox_pw = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_export
            // 
            this.button_export.Location = new System.Drawing.Point(14, 71);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(75, 23);
            this.button_export.TabIndex = 0;
            this.button_export.Text = "匯出";
            this.button_export.UseVisualStyleBackColor = true;
            this.button_export.Click += new System.EventHandler(this.button_export_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(95, 71);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(75, 23);
            this.button_import.TabIndex = 1;
            this.button_import.Text = "回復";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(63, 43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(107, 22);
            this.textBox1.TabIndex = 2;
            // 
            // textBox_pw
            // 
            this.textBox_pw.Location = new System.Drawing.Point(63, 11);
            this.textBox_pw.Name = "textBox_pw";
            this.textBox_pw.Size = new System.Drawing.Size(107, 22);
            this.textBox_pw.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "密碼";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "資料";
            // 
            // FormAccRecovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(191, 110);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_pw);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_export);
            this.Name = "FormAccRecovery";
            this.Text = "帳密回復";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox_pw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}