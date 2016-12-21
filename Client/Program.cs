using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aogood.Foundation;
using Aogood.Network;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            CNetworkClient netWClient = new CNetworkClient("192.168.8.118", 8899);
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "send")
                {
                    netWClient.Connect();
                    CCoroutineManager.StartCoroutine(Wait(netWClient));

                }
                if (command == "close")
                {
                    break;
                }
            }

            CCoroutineManager.Stop();

        }
        static IEnumerator Wait(Aogood.Network.CNetworkClient c)
        {
            Aogood.SHLib.MSG_CTS_CHAT send = new Aogood.SHLib.MSG_CTS_CHAT();
            send.Content = "i love you";
            Aogood.Network.CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_CHAT> chat = new Aogood.Network.CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_CHAT>();
            if (!c.SendResponsible(send, out chat))
            {
                Console.WriteLine("Send Failed");
            }
            yield return CNetworkClient.WaitForResponse(chat);
            if (chat.IsResponse)
            {
                Console.WriteLine(chat.ResponseMsg.Content);
            }
        }
    }
}
