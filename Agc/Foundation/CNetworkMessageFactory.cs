using System;
using System.Collections.Generic;
using Aogood.Network;
using System.Net.Sockets;
using System.Threading;
using Aogood.SHLib;

namespace Aogood.Foundation
{
    public class CNetworkMessageFactory
    {
        /// <summary>
        /// 注册该事件监听网络消息
        /// </summary>
        public event CNetworkMessageEvent NetworkMessageGet;

        protected CNetwork m_Network;

        public void SetNetworkInstance(CNetwork network)
        {
            m_Network = network;
        }
        public void AnalyseNetworkMessage(Socket proxSocket, CNetworkMessage msg)
        {
            CNetworkMessageEventArgs arg = new CNetworkMessageEventArgs();
            arg.NetworkMsg = msg;
            arg.ProxSocket = proxSocket;
            if (NetworkMessageGet != null)
                NetworkMessageGet(this, arg);
        }
    }
}
