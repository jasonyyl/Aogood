using System;
using Aogood.Network;

namespace Aogood.SHLib
{

    public enum EChatSystemMessageType
    {
        NONE = EModuleType.CHAT_SYSTEM * CSystemParameters.NetMessageRange,
        /// <summary>
        /// 发送信息：客户端到服务器
        /// </summary>
        MSG_CTS_CHAT,
        /// <summary>
        ///  发送信息：服务器到客户端
        /// </summary>
        MSG_STC_CHAT,
    }
    [Serializable]
    public class MSG_CTS_CHAT : CNetworkMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }

        public string Receiver { get; set; }
        public MSG_CTS_CHAT()
            : base((int)EChatSystemMessageType.MSG_CTS_CHAT, (int)EModuleType.CHAT_SYSTEM)
        {

        }
    }
    [Serializable]
    public class MSG_STC_CHAT : CNetworkMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Receiver { get; set; }
        public MSG_STC_CHAT()
            : base((int)EChatSystemMessageType.MSG_STC_CHAT, (int)EModuleType.CHAT_SYSTEM)
        {

        }
    }
}
