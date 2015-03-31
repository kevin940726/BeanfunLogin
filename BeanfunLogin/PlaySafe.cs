using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSFISCATLLib;
using FSP11CRYPTATLLib;

namespace BeanfunLogin
{
    public class PlaySafe
    {
        public string cardType;
        public string CardReader;
        public string cardid;
        FSFISCClass fs;
        FSP11CRYPTATLLib.KENP11CryptClass fsPKI;

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


        // PKI
        public string FSCAPISign(string pass, string original)
        {
            fsPKI = new KENP11CryptClass();
            var rtn = fsPKI.FSXP11Init("gclib.dll");
            if (rtn != 0)
            {
                var ErrCode = fsPKI.GetErrorCode();
                if (ErrCode == 9110)
                {
                    return "憑證卡讀取失敗( 晶片卡驅動程式未安裝 )";
                }
                else if (ErrCode == 9056)
                {
                    return "憑證卡讀取失敗( 請插入晶片卡 )";
                }
                else
                {
                    return "憑證卡讀取失敗(" + ErrCode + ")";
                }
            }

            rtn = fsPKI.FSXP11SessionOpen();
            if (rtn != 0)
            {
                fsPKI.FSXP11Final();
                return "憑證卡開啟失敗(" + fsPKI.GetErrorCode() + ")";
            }

            var serialNumber = fsPKI.FSP11_GetSerialNumber();
            var SNErrCode = fsPKI.GetErrorCode();
            if (SNErrCode != 0)
            {
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                return "read card serial number fail(" + SNErrCode + ")";
            }
            serialNumber = serialNumber.Substring(0, 16);
            this.cardid = serialNumber;

            rtn = fsPKI.FSXP11Login(pass);
            if (rtn != 0)
            {
                var varErrorCode = fsPKI.GetErrorCode();
                rtn = fsPKI.FSP11_GetRetryCounter(0x00020000);
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                if (varErrorCode == 9039)
                {
                    return "密碼驗證失敗:(還有" + rtn + "次機會" + ")";
                }
                else if (varErrorCode == 9043)
                {
                    return "密碼輸入錯誤已達八次，請用購買認證序號解鎖!";
                }
                else
                {
                    return "憑證卡登入失敗" + varErrorCode + ")";
                }
            }

            /*rtn = fsPKI.FSP11_GetPinFlag();
            var ErrorCode = fsPKI.GetErrorCode();
            if (ErrorCode != 0)
            {
                fsPKI.FSXP11Logout();
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                return "憑證卡讀取資料失敗(" + ErrorCode + ")";
            }
            else if (rtn == 9990)
            {
                fsPKI.FSXP11Logout();
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                return "憑證卡讀取資料失敗(" + fsPKI.GetErrorCode() + ")";
            }*/

            return FSP11CheckCert(original);
        }

        private string FSP11CheckCert(string original)
        {
            var count = fsPKI.FSXP11GetObjectList(0);
            if (fsPKI.GetErrorCode() != 0)
            {
                fsPKI.FSXP11Logout();
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                return "Error on funtcion FSXP11GetObjectList, error code=" + fsPKI.GetErrorCode();
            }

            var CertLabel = "";
            for (var i = 0; i < count; i++)
            {
                if (fsPKI.FSXP11GetObjectListObjectType(i) == 0x00000011)
                {
                    CertLabel = fsPKI.FSXP11GetObjectListLabel(i);
                    if (CertLabel == "PlaySAFE")
                        break;
                    else
                        CertLabel = "";
                }
                if (fsPKI.GetErrorCode() != 0)
                {
                    fsPKI.FSXP11Logout();
                    fsPKI.FSXP11SessionClose();
                    fsPKI.FSXP11Final();
                    return "憑證存取失敗,請您關閉程式後重新再試(" + fsPKI.GetErrorCode() + ")";
                }
            }

            if (CertLabel != "PlaySAFE")
            {
                fsPKI.FSXP11Logout();
                fsPKI.FSXP11SessionClose();
                fsPKI.FSXP11Final();
                return "找不到指定物件 Label[PlaySAFE]";
            }

            return SignatureData(original);
        }

        private string SignatureData(string original)
        {
            var signature = fsPKI.FSP11Sign("PlaySAFE", 0, original, 0);
            var SignErrCode = fsPKI.GetErrorCode();
            fsPKI.FSXP11Logout();
            fsPKI.FSXP11SessionClose();
            fsPKI.FSXP11Final();
            if (SignErrCode != 0)
            {
                return "簽章失敗(" + SignErrCode + ")";
            }

            return signature;
        }

    }
}
