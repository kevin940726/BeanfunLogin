using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSFISCATLLib;

namespace BeanfunLogin
{
    public class PlaySafe
    {
        public string cardType;
        public string CardReader;
        public string cardid;
        FSFISCClass fs;

        public PlaySafe()
        {
            this.fs = new FSFISCClass();
            this.cardType = null;
            this.CardReader = null;
            this.cardid = null;
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
                return null;
            }
            if (aaa == null)
                return null;
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
                    return null;

                return readername;
            }
        }

        public string GetPublicCN(string reader)
        {
            if (reader == "" || reader == null)
                return null;
            string rtn = fs.FSFISC_GetPublicCN(reader, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return null;
            return rtn;
        }

        public string GetOPInfo(string reader, string pin)
        {
            if (reader == "" || reader == null)
                return null;
            string rtn = fs.FSFISC_GetOPInfo(reader, pin, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return null;
            return rtn;
        }

        public string EncryptData(string reader, string pin, string data)
        {
            if (reader == "" || reader == null)
                return null;
            string rtn = fs.FSFISC_GetTAC(reader, pin, data, 0, 0);
            if (fs.FSFISC_GetErrorCode() != 0)
                return null;
            return rtn;
        }
    }
}
