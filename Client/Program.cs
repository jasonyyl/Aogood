using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Aogood.Network.CNetworkClient n = new Aogood.Network.CNetworkClient("192.168.8.118",8899);
            n.Start();      
                 
        }
    }
}
