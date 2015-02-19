using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using FSFISCATLLib;

namespace BeanfunLogin
{
    public partial class main : Form
    {
        private string cardType = "";
        private string CardReader = "";
        private string cardid = "";
        FSFISCClass fs;

        private string getServerTime2()
        {
            DateTime date = DateTime.Now;
            return date.Year.ToString() + (date.Month - 1).ToString() + date.ToString("ddHHmmssfff");
        }

        public string GetReader()
        {
            string readername = "";
            object aaa;
            try
            {
                aaa = fs.FSFISC_GetReaderNames(0);
            }
            catch
            {
                return "Fail";
            }
            if (aaa == null)
                return "Fail";
            else
            {
                var readers = ((System.Collections.IEnumerable)aaa).Cast<object>().Select(x => x.ToString()).ToArray();
                foreach (var reader in readers)
                {
                    var cardflag = fs.FSFISC_GetCardType2(reader);
                    if (fs.FSFISC_GetErrorCode() != 0)
                        cardflag = -1;
                    else if (cardflag == 0)
                    {
                        readername = reader;
                        cardType = "F";
                    }
                    else if (cardflag == 1)
                    {
                        readername = reader;
                        cardType = "G";
                    }

                }
                if (readername != "")
                {
                    CardReader = readername;
                }
                else
                    return "Fail";

                return readername;
            }
        }

        private string GetPublicCN(string reader)
        {
            if (reader == "" || reader == null)
                return "Fail";
            string rtn = fs.FSFISC_GetPublicCN(reader, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }

        private string GetOPInfo(string reader, string pin)
        {
            if (reader == "" || reader == null)
                return "Fail";
            string rtn = fs.FSFISC_GetOPInfo(reader, pin, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }

        private string EncryptData(string reader, string pin, string data)
        {
            if (reader == "" || reader == null)
                return "Fail";
            string rtn = fs.FSFISC_GetTAC(reader, pin, data, 0, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return "Fail";
            return rtn;
        }
    }
}
