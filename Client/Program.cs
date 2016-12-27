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
        static CNetworkClient c;
        static void Main(string[] args)
        {
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "connect")
                {
                    c = new CNetworkClient("192.168.8.100", 8899);
                    c.LogEvent += LogEventHandler;
                    c.Connect();
                }
                if (command == "login")
                {
                    Console.Write("LoginName:");
                    string loginName = Console.ReadLine();
                    Console.Write("Password:");
                    string pwd = Console.ReadLine();
                    CCoroutineManager.StartCoroutine(LoginCoroutine(c, loginName, pwd));
                }
                if (command == "close")
                {
                    break;
                }
            }

            CCoroutineManager.Stop();

        }

        static IEnumerator LoginCoroutine(CNetworkClient c, string loginName, string pwd)
        {
            Aogood.SHLib.MSG_CTS_LOGIN loginMsg = new Aogood.SHLib.MSG_CTS_LOGIN();
            loginMsg.LoginName = loginName;
            loginMsg.PassWord = pwd;
            CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_LOGIN> login = new CNetworkMessageResponseHandlerT<Aogood.SHLib.MSG_STC_LOGIN>();
            if (!c.SendResponsible(loginMsg, out login))
            {
                Console.WriteLine("Send Failed");
            }
            yield return CNetworkClient.WaitForResponse(login);
            if (login.IsResponse)
            {
                Console.WriteLine(login.ResponseMsg.LoginMsg);
            }
        }

        private static void LogEventHandler(string log)
        {
            Console.WriteLine(log);
        }
    }
}
