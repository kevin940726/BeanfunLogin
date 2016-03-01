/*
 * 開發此功能主要用為多帳號時儲存
 * 以原有加解密寫法為基礎
 * 加上一層wrapper並用Serializable方式儲存資料
 * thanks to Stackoverflow :p
 * http://stackoverflow.com/questions/5869922/c-sharp-encrypt-serialized-file-before-writing-to-disk
 * http://stackoverflow.com/questions/16352879/write-list-of-objects-to-a-file
 * 
 * Date: 2016/3/1
 * Author: 葉家郡 (a.k.a 某數)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace BeanfunLogin
{
    [Serializable]
    class AccountRecords
    {
        public List<string> accountList = null, passwdList = null;
        public List<int> methodList = null;
    }

    class AccountManager
    {
        private AccountRecords accountRecords = null;
    

        public bool init()
        {
            return loadRecord();
        }

        #region helper function
        private void accRecInit()
        {
            if (accountRecords == null)
                accountRecords = new AccountRecords();

            if (accountRecords.accountList == null)
                accountRecords.accountList = new List<string>();
            if (accountRecords.passwdList == null)
                accountRecords.passwdList = new List<string>();
            if (accountRecords.methodList == null)
                accountRecords.methodList = new List<int>();
        }

        private bool loadRecord()
        {
            var raw = readRawData();
            if (raw != null)
            {
                byte[] cipher = Convert.FromBase64String(raw);

                using (Stream stream = new MemoryStream(cipher))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    accountRecords = (AccountRecords)bformatter.Deserialize(stream);
                }
            }
            accRecInit();

            return true;
        }

        private bool storeRecord()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Serialize to memory instead of to file
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, accountRecords);

                // This resets the memory stream position for the following read operation
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Get the bytes
                var bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);

                writeRawData(Convert.ToBase64String(bytes));
            }

            return true;
        }
        #endregion

        #region rawdata IO
        /*
         * read ciphertext from File
         * decrypt it and return
         */
        private string readRawData()
        {
            try
            {
                if (File.Exists("UserList.dat"))
                {
                    Byte[] cipher = File.ReadAllBytes("UserList.dat");
                    string entropy = Properties.Settings.Default.entropyForList;
                    byte[] plaintext = ProtectedData.Unprotect(cipher, Encoding.UTF8.GetBytes(entropy), DataProtectionScope.CurrentUser);
                    return System.Text.Encoding.UTF8.GetString(plaintext);
                }

                return null;
            }
            catch { return null; }
        }

        /*
         * encrypt plaintext and store to File
         * and save key in Program Setting
         */
        private void writeRawData(string plaintext)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("UserList.dat", FileMode.Create)))
            {
                // Create random entropy of 8 characters.
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                string entropy = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

                Properties.Settings.Default.entropyForList = entropy;
                writer.Write(ciphertext(plaintext, entropy));
            }
            Properties.Settings.Default.Save();
        }

        private byte[] ciphertext(string plaintext, string key)
        {
            byte[] plainByte = Encoding.UTF8.GetBytes(plaintext);
            byte[] entropy = Encoding.UTF8.GetBytes(key);
            return ProtectedData.Protect(plainByte, entropy, DataProtectionScope.CurrentUser);
        }
        #endregion

        #region Interface
        public bool addAccount(string account,string password,int method)
        {
            bool isExists = false;
            for( int i = 0 ; i < accountRecords.accountList.Count ; ++i )
            {
                if(account == accountRecords.accountList[i])
                {
                    accountRecords.passwdList[i] = password;
                    accountRecords.methodList[i] = method;
                    isExists = true;
                    break;
                }
            }

            if (!isExists)
            {
                accountRecords.accountList.Add(account);
                accountRecords.passwdList.Add(password);
                accountRecords.methodList.Add(method);
            }

            storeRecord();

            return true;
        }

        public string getPasswordByAccount(string account)
        {
            for (int i = 0; i < accountRecords.accountList.Count; ++i)
            {
                if (account == accountRecords.accountList[i])
                {
                    return accountRecords.passwdList[i];
                }
            }
            return null;
        }

        public int getMethodByAccount(string account)
        {
            for (int i = 0; i < accountRecords.accountList.Count; ++i)
            {
                if (account == accountRecords.accountList[i])
                {
                    return accountRecords.methodList[i];
                }
            }

            return -1;
        }

        public bool removeAccount(string account)
        {
            for (int i = 0; i < accountRecords.accountList.Count; ++i)
            {
                if (account == accountRecords.accountList[i])
                {
                    accountRecords.accountList.RemoveAt(i);
                    accountRecords.passwdList.RemoveAt(i);
                    accountRecords.methodList.RemoveAt(i);

                    storeRecord();
                    return true;
                }
            }

            return false;
        }

        public string[] getAccountList()
        {
            return accountRecords.accountList.ToArray();
        }
        #endregion
    }
}
