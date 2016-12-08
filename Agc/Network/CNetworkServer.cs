﻿using System;
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


        #region 属性
        public bool IsListen { get; set; }

        #endregion


        #region 变量
        protected CNetworkMessageFactory m_factory;
        protected Dictionary<string, Socket> m_SocketProx;
        protected ManualResetEvent m_AllDone = new ManualResetEvent(false);
        protected ManualResetEvent m_SendDone = new ManualResetEvent(false);
        #endregion

        #region 方法
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
        public void SetMessageFactory(CNetworkMessageFactory factory)
        {
            m_factory = factory;
            factory.SetNetworkInstance(this);
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
                StateObject obj = new StateObject();
                obj.workSocket = proxSocket;
                m_SocketProx.Add(proxSocket.RemoteEndPoint.ToString(), proxSocket);
                Console.WriteLine("iP为{0}的用户连接到服务器", proxSocket.RemoteEndPoint);
                proxSocket.BeginReceive(obj.buffer, 0, obj.buffer.Length, SocketFlags.None, ReceiveCallback, obj);
            }
            catch
            {

            }


        }
        protected override void SendCallback(IAsyncResult ar)
        {
            try
            {

                // 返回连接的socket
                Socket client = (Socket)ar.AsyncState;

                // 成功发送内容
                int bytesSent = client.EndSend(ar);

                //获取客户端ip地址和端口
                IPEndPoint ipEndPoint = client.RemoteEndPoint as IPEndPoint;
                if (ipEndPoint != null)
                    Console.WriteLine("发送 {0} 字节数据到iP地址为：{1} 端口为：{2}的客户端", bytesSent, ipEndPoint.Address, ipEndPoint.Port);

                // 标志发送成功
                m_SendDone.Set();
            }
            catch (Exception e)
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket proxSocket = state.workSocket;
                IPEndPoint ipEndPoint = proxSocket.RemoteEndPoint as IPEndPoint;
                if (proxSocket != null && !proxSocket.Connected && ipEndPoint != null)
                {
                    CDebug.Log("客户端iP为:{0} Port为:{1}断开连接", ipEndPoint.Address, ipEndPoint.Port);
                    m_SocketProx.Remove(proxSocket.RemoteEndPoint.ToString());
                    proxSocket.Shutdown(SocketShutdown.Both);
                    proxSocket.Close();
                }
            }
        }
        protected override void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;
                StateObject state = (StateObject)ar.AsyncState;
                Socket proxSocket = state.workSocket;
                int bytesRead = proxSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    using (MemoryStream stream = new MemoryStream(state.buffer))
                    {
                        BinaryFormatter binFormat = new BinaryFormatter();
                        stream.Position = 0;
                        stream.Seek(0, SeekOrigin.Begin);
                        CNetworkMessage msgObj = (CNetworkMessage)binFormat.Deserialize(stream);
                        if (m_factory != null)
                            m_factory.AnalyseNetworkMessage(proxSocket, msgObj);
                    }
                }
                StateObject obj = new StateObject();
                obj.workSocket = proxSocket;
                proxSocket.BeginReceive(obj.buffer, 0, obj.buffer.Length, SocketFlags.None, ReceiveCallback, obj);
            }
            catch (Exception e)
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket proxSocket = state.workSocket;
                IPEndPoint ipEndPoint = proxSocket.RemoteEndPoint as IPEndPoint;
                if (proxSocket != null && !proxSocket.Connected && ipEndPoint != null)
                {
                    CDebug.Log("客户端iP为:{0} Port为:{1}断开连接", ipEndPoint.Address, ipEndPoint.Port);
                    m_SocketProx.Remove(proxSocket.RemoteEndPoint.ToString());
                    proxSocket.Shutdown(SocketShutdown.Both);
                    proxSocket.Close();
                }
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

    public class StateObject
    {
        // 客户端 Socket
        public Socket workSocket = null;
        // 接收的缓冲区的长度.
        public const int BufferSize = 1024;
        // 接收到的缓冲.
        public byte[] buffer = new byte[BufferSize];
        //回调
        public CNetworkMessageEvent ReceiverCallBack;
    }

    public class ResponseObject
    {
        DateTime m_DtStart = DateTime.MaxValue;
        //回调
        public CNetworkMessageEvent ReceiverCallBack;

        public CNetworkMessage RequestMessage = null;

        public CNetworkMessageResponseHandlerT<CNetworkMessage> ResponseMsg = null;

        public bool IsWait = false;
        public ResponseObject()
        {
            
        }
        public void StartWait(DateTime dt_start)
        {
            m_DtStart = dt_start;
            IsWait = true;
        }
        public float GetTime()
        {
            return (float)DateTime.Now.Subtract(m_DtStart).TotalSeconds;
        }
    }
}

