using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aogood.SHLib;

namespace Aogood.Network
{
    public abstract class CNetworkMessageResponseHandler
    {
        public bool IsResponse { get; set; }

        public abstract void SetResoneMessage(CNetworkMessage response);

    }

    public class CNetworkMessageResponseHandlerT<T> : CNetworkMessageResponseHandler where T : CNetworkMessage
    {
        public T ResponseMsg { get { return GetResponseMessage(); } }
        CNetworkMessage m_ResponseMsg;

        T GetResponseMessage()
        {
            if (m_ResponseMsg != null)
            {
                return (T)m_ResponseMsg;
            }
            return null;
        }
        public override void SetResoneMessage(CNetworkMessage response)
        {
            m_ResponseMsg = response;
        }
    }
}
