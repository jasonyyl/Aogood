using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Aogood.Foundation;

namespace Aogood.Network
{
    public class CNetworkServer : CNetwork
    {
        #region Field
        protected Dictionary<string, Socket> m_SocketProx;
        protected ManualResetEvent m_AllDone = new ManualResetEvent(false);
        #endregion

        #region Property
        public bool IsListen { get; set; }

        #endregion

        #region Method
        public CNetworkServer(string ip, int port)
        {
            m_Port = port;
            m_IP = IPAddress.Parse(ip);
            IsListen = false;
            m_SocketProx = new Dictionary<string, Socket>();
            m_SocketMain = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(m_IP, m_Port);
                m_SocketMain.Bind(ipEndPoint);
                m_SocketMain.Listen(10);
                IsListen = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.SocketAcceptClientAsyns), m_SocketMain);
            }
            catch (Exception e)
            {

            }

        }
        private void SocketAcceptClientAsyns(object state)
        {

            while (true)
            {
                m_AllDone.Reset();
                try
                {
                    //等待客户端连接
                    Socket socket = state as Socket;
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
                }
                catch
                {
                }
                m_AllDone.WaitOne();
            }

        }
        private void AcceptCallback(IAsyncResult ar)
        {
            m_AllDone.Set();
            try
            {
                Socket listenerSocket = (Socket)ar.AsyncState;
                Socket proxSocket = listenerSocket.EndAccept(ar);
                ResponseObject state = new ResponseObject();
                state.workSocket = proxSocket;
                m_SocketProx.Add(proxSocket.RemoteEndPoint.ToString(), proxSocket);
                Console.WriteLine("iP为{0}的用户连接到服务器", proxSocket.RemoteEndPoint);
                Receive(state);
            }
            catch
            {

            }


        }
        public void Stop(Socket sokcet)
        {


        }
        public void Stop()
        {
            if (m_SocketProx != null)
            {
                foreach (var item in m_SocketProx)
                {
                    item.Value.Shutdown(SocketShutdown.Both);
                    item.Value.Close();
                }
                m_SocketProx.Clear();
            }
            if (m_SocketMain != null)
            {
                m_SocketMain.Close();
            }
            IsListen = false;
        }


        #endregion
    }
}

