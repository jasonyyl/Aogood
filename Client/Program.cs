using System;
using System.Collections;
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
            Aogood.Foundation.ClientApp.Start();
            Aogood.Network.CNetworkClient n = new Aogood.Network.CNetworkClient("192.168.8.118", 8899);
            n.Start();
            Aogood.Foundation.CCoroutineManager.StartCoroutine(Wait(n));
            Console.ReadKey();
            Aogood.Foundation.ClientApp.Stop();

        }
        static IEnumerator Wait(Aogood.Network.CNetworkClient c)
        {
            Aogood.SHLib.MSG_CTS_CHAT send = new Aogood.SHLib.MSG_CTS_CHAT();
            send.Content = "我爱你";
            Aogood.Network.CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_CHAT> chat = new Aogood.Network.CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_CHAT>();
            if (!c.SendResponsible(send, out chat))
            {
                Console.WriteLine("Send Failed");
            }
            yield return Aogood.Network.CNetworkClient.WaitForResponse(chat);
            if (chat.IsResponse)
            {
                Console.WriteLine(chat.ResponseMsg.Content);
            }
        }
    }
}
