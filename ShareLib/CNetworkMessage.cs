using System;
using Aogood.Foundation;

namespace Aogood.SHLib
{
    [Serializable]
    public class CNetworkMessage : CMessage
    {
        public int MessageModuleId { get; set; }
        public CNetworkMessage(int messageId, int messageSystem)
        {
            MessageId = messageId;
            MessageModuleId = messageSystem;
        }
    }
}
