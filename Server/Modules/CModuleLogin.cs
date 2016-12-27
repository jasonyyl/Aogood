using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aogood.SHLib;

namespace Server
{
    public class CModuleLogin : CModule
    {
        public override EModuleType GetModuleType()
        {
            return EModuleType.LOGIN_SYSTEM;
        }

        public override CNetworkMessage MessageHandle(CNetworkMessage msg)
        {
            MSG_CTS_LOGIN mc = msg as MSG_CTS_LOGIN;
            if (mc != null)
            {
                MSG_STC_LOGIN ms = new MSG_STC_LOGIN();
                if (mc.LoginName == "vlisk" && mc.PassWord == "1")
                {
                    ms.IsLogin = true;
                    ms.LoginMsg = "login success";
                }
                else
                {
                    ms.IsLogin = false;
                    ms.LoginMsg = "login failed";
                }
                return ms;
            }
            return null;
        }
    }
}
