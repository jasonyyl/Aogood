using Aogood.Network;
using System;

namespace Aogood.SHLib
{
    public enum ELoginSystemMessageType
    {
        NONE = EModuleType.LOGIN_SYSTEM * CSystemParameters.NetMessageRange,
        /// <summary>
        /// 登录信息：客户端到服务器
        /// </summary>
        MSG_CTS_LOGIN,
        /// <summary>
        ///  登录信息：服务器到客户端
        /// </summary>
        MSG_STC_LOGIN,
    }

    [Serializable]
    public class MSG_CTS_LOGIN : CNetworkMessage
    {

        public string LoginName { get; set; }
        public string PassWord { get; set; }
        public MSG_CTS_LOGIN()
            : base((int)ELoginSystemMessageType.MSG_CTS_LOGIN, (int)EModuleType.LOGIN_SYSTEM)
        {

        }

    }
    [Serializable]
    public class MSG_STC_LOGIN : CNetworkMessage
    {
        public bool IsLogin { get; set; }
        public string LoginMsg { get; set; }
        public MSG_STC_LOGIN()
            : base((int)ELoginSystemMessageType.MSG_STC_LOGIN, (int)EModuleType.LOGIN_SYSTEM)
        {

        }

    }
}
