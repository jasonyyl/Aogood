using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Aogood.Network.CNetworkServer s = new Aogood.Network.CNetworkServer("192.168.8.118", 8899);
            s.Start();
            Console.ReadKey();
        }
    }
}
