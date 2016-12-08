
namespace Aogood.Network
{

    public class CNetworkMessageEventArgs
    {
        public CNetworkMessage NetworkMsg { get; set; }

        public System.Net.Sockets.Socket ProxSocket { get; set; }

    }
}
