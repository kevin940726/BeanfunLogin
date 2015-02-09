using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanfunLogin
{
    public class AccountListClass
    {
         public string s_acc;
         public string s_otp;
         public string s_name;
         public string s_createTime;

         public AccountListClass() { this.s_acc = ""; this.s_otp = ""; this.s_name = ""; this.s_createTime = ""; }
         public AccountListClass(string id, string sn, string name) { this.s_acc = id; this.s_otp = sn; this.s_name = name; }
    }
}
