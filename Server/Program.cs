using Aogood.Network;
using Aogood.Foundation;
using Aogood.SHLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static CNetworkServer s;
        static CModuleManager m_ModuleManger;
        static void Main(string[] args)
        {
            m_ModuleManger = new CModuleManager();
            m_ModuleManger.LoadModule(EModuleType.LOGIN_SYSTEM);
            s = new CNetworkServer("192.168.8.100", 8899);
            s.MessageReceiveEvent += MessageReceiveEventHandler;
            s.LogEvent += LogEventHandler;
            s.Start();
            Console.ReadKey();
        }
        private static void MessageReceiveEventHandler(ResponseObject obj)
        {
            CNetworkMessage msg = m_ModuleManger.GetHandledMessage(obj.msgPack.GetMessage(obj.msgPack.MsgContent));
            RequestObject requset = CAogoodFactory.Instance.GetObject<RequestObject>();
            requset.SetRequsetMessage(msg);
            requset.workSocket = obj.workSocket;
            s.Send(requset);
            s.Receive(obj);
        }
        private static void LogEventHandler(string log)
        {
            Console.WriteLine(log);
        }
    }
}
