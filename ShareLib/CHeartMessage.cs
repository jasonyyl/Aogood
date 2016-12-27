using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aogood.Network;
namespace Aogood.SHLib
{
    public enum EHeartMessageType
    {
        NONE = EModuleType.CHAT_SYSTEM * CSystemParameters.NetMessageRange,
        /// <summary>
        /// 心跳包：客户端到服务器
        /// </summary>
        MSG_CTS_HEART,
        /// <summary>
        ///  心跳包：服务器到客户端
        /// </summary>
        MSG_STC_HEART,
    }
    [Serializable]
    public class MSG_CTS_HEART : CNetworkMessage
    {
        public MSG_CTS_HEART()
            : base((int)EHeartMessageType.MSG_CTS_HEART, (int)EModuleType.HEART_SYSTEM)
        {

        }

    }
    [Serializable]
    public class MSG_STC_HEART : CNetworkMessage
    {
        public bool IsGet { get; set; }
        public MSG_STC_HEART()
            : base((int)EHeartMessageType.MSG_STC_HEART, (int)EModuleType.HEART_SYSTEM)
        {

        }

    }
}
