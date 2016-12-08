using System;
using Aogood.Foundation;

namespace Aogood.Network
{
    [Serializable]
    public class CNetworkMessage : CMessage
    {
        public int MessageSystemId { get; set; }
        public CNetworkMessage(int messageId, int messageSystem)
        {
            MessageId = messageId;
            MessageSystemId = messageSystem;
        }
    }
}
