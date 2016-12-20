using Aogood.Network;
using Aogood.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static Aogood.Network.CNetworkServer s;
        static void Main(string[] args)
        {
            s = new Aogood.Network.CNetworkServer("192.168.8.118", 8899);
            s.Start();
            s.MessageReceiveEvent += MessageReceiveEventHandler;
            Console.ReadKey();
        }

        private static void MessageReceiveEventHandler(ResponseObject obj)
        {
            Aogood.SHLib.MSG_STC_CHAT msg = new Aogood.SHLib.MSG_STC_CHAT();
            msg.Content = "你好";
            RequestObject requset = CAogoodFactory.Instance.GetObject<RequestObject>();
            requset.SetRequsetMessage(msg);
            requset.workSocket = obj.workSocket;
            s.Send(requset);
            s.Receive(obj);
        }
    }
}
