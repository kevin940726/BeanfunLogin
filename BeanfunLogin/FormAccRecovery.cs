using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeanfunLogin
{
    public partial class FormAccRecovery : Form
    {
        private AccountManager accMan;
        public FormAccRecovery(AccountManager a)
        {
            InitializeComponent();

            accMan = a;
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            string plaintext = accMan.exportRecord();
            byte[] plain_bytes = Encoding.UTF8.GetBytes(plaintext);

            var md5 = new MD5CryptoServiceProvider();
            byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(textBox_pw.Text));
            RijndaelManaged provider_AES = new RijndaelManaged();
            ICryptoTransform encrypt_AES = provider_AES.CreateEncryptor(key, md5.ComputeHash(Encoding.UTF8.GetBytes("xnumtw")));
            
            byte[] output = encrypt_AES.TransformFinalBlock(plain_bytes, 0, plain_bytes.Length);
            textBox1.Text = Convert.ToBase64String(output);
            
            MessageBox.Show("匯出完成");
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            byte[] byte_pwd = Encoding.UTF8.GetBytes(textBox_pw.Text);
            MD5CryptoServiceProvider provider_MD5 = new MD5CryptoServiceProvider();
            byte[] byte_pwdMD5 = provider_MD5.ComputeHash(byte_pwd);

            RijndaelManaged provider_AES = new RijndaelManaged();
            ICryptoTransform decrypt_AES = provider_AES.CreateDecryptor(byte_pwdMD5, provider_MD5.ComputeHash(Encoding.UTF8.GetBytes("xnumtw")));

            byte[] input = Convert.FromBase64String(textBox1.Text);
            try
            {
                byte[] byte_secretContent = decrypt_AES.TransformFinalBlock(input, 0, input.Length);
                string plaintext = Encoding.UTF8.GetString(byte_secretContent);

                if (false == accMan.importRecord(plaintext))
                {
                    MessageBox.Show("匯入失敗");
                }
                else
                {
                    MessageBox.Show("匯入成功");
                }
            }
            catch
            {
                MessageBox.Show("密碼或資料錯誤，解密失敗");
                return;
            }
        }
    }
}
