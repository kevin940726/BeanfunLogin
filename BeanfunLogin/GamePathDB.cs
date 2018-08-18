using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanfunLogin
{
    class GamePathDB
    {
        Dictionary<string, string> alias = new Dictionary<string, string>()
            {
                {"新楓之谷", "MapleStory.exe"},
                {"艾爾之光", "elsword.exe"},
                {"跑跑", "KartRider.exe"},
            };

        Dictionary<string, string> db;
        public GamePathDB()
        {
            string oldGamePath = Properties.Settings.Default.gamePath;
            string raw = Properties.Settings.Default.gamePathDB;
            db = new Dictionary<string, string>();

            if (oldGamePath != "")
            {
                db["MapleStory"] = oldGamePath;
                Properties.Settings.Default.gamePath = "";
            }

            try
            {
                db = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw);
            }
            catch
            {
                Properties.Settings.Default.gamePathDB = JsonConvert.SerializeObject(db);
            }

            Properties.Settings.Default.Save();
        }

        public string GetAlias(string key)
        {
            foreach (string k in alias.Keys)
            {
                if (key.Contains(k))
                {
                    return alias[k];
                }
            }

            return key;
        }

        public string Get(string key)
        {
            string val = "";
            return db.TryGetValue(GetAlias(key), out val) == true ? val : "";
        }

        public void Set(string key, string val)
        {

            db[GetAlias(key)] = val;
        }

        public void Save()
        {
            Properties.Settings.Default.gamePathDB = JsonConvert.SerializeObject(db);
            Properties.Settings.Default.Save();
        }
    }
}
