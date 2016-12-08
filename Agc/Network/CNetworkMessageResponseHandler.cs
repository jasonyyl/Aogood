using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aogood.Network
{
    public abstract class CNetworkMessageResponseHandler
    {
        public abstract bool IsResponse();
    }

    public class CNetworkMessageResponseHandlerT<T> : CNetworkMessageResponseHandler where T : CNetworkMessage
    {
        bool m_IsResponse = false;
        public override bool IsResponse()
        {
            return m_IsResponse;
        }
    }
}
