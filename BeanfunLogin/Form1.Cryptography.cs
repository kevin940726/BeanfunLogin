using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BeanfunLogin
{
    public partial class main : Form
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
            catch
            {
                errexit("Decrypt Fail", "密碼獲取失敗，密碼解析失敗。\nFailed to decrypt the OTP.", 1);
                return "";
            }
        }

        // Building ciphertext by 3DES.
        private byte[] ciphertext(string plaintext, string key)
        {
            byte[] plainByte = Encoding.UTF8.GetBytes(plaintext);
            byte[] entropy = Encoding.UTF8.GetBytes(key);
            return ProtectedData.Protect(plainByte, entropy, DataProtectionScope.CurrentUser);
        }

    }
}
