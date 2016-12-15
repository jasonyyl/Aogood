using Aogood.Foundation;
using Aogood.SHLib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Aogood.Network
{
    public class CNetworkClient : CNetwork
    {
        public bool IsConnected { get; protected set; }

        static DateTime m_StartWaitTime = DateTime.MaxValue;
        protected int m_HeartTimeInterval = 5000;
        protected Thread m_ThreadSendReceive;
        protected CNetworkMessageFactory m_factory;
        protected System.Timers.Timer m_TimerCount = new System.Timers.Timer();
        private ManualResetEvent m_ResponseDone = new ManualResetEvent(false);
        private ManualResetEvent m_ConnectDone = new ManualResetEvent(false);      
        private ManualResetEvent m_ReceiveDone = new ManualResetEvent(false);
        protected List<ResponseObject> m_WaitForSend = new List<ResponseObject>();
        protected List<ResponseObject> m_WaitForResponse = new List<ResponseObject>();
        protected ResponseObject m_CurrentWait = new ResponseObject();
        public CNetworkClient(string ip, int port)
        {
            m_Port = port;
            m_IP = IPAddress.Parse(ip);
            //m_ThreadSendReceive = new Thread(new ParameterizedThreadStart(Send));
            IsConnected = false;
        }
        public void Start()
        {

            try
            {
                m_SocketMain = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(m_IP, m_Port);
                m_SocketMain.BeginConnect(remoteEP, new AsyncCallback(OnConnected), m_SocketMain);
            }
            catch (Exception e)
            {

            }

        }
        public bool SendResponsible<T>(CNetworkMessage sendMsg, out CNetworkMessageResponseHandlerT<T> responseMsg) where T : CNetworkMessage
        {
            m_IsSend = false;
            responseMsg = new CNetworkMessageResponseHandlerT<T>();
            Send(m_SocketMain, sendMsg);
            m_SendDone.WaitOne();
            return m_IsSend;
        }
        protected void OnConnected(IAsyncResult ar)
        {
            Socket socketClient = (Socket)ar.AsyncState;
            if (socketClient != null)
                socketClient.EndConnect(ar);
            IsConnected = true;
            Console.WriteLine("connnect");

        }
        //protected override void ReceiveCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        //String content = String.Empty;
        //        //StateObject state = (StateObject)ar.AsyncState;
        //        //Socket proxSocket = state.workSocket;
        //        //int bytesRead = proxSocket.EndReceive(ar);

        //        //if (bytesRead > 0)
        //        //{
        //        //    using (MemoryStream stream = new MemoryStream(state.buffer))
        //        //    {
        //        //        BinaryFormatter binFormat = new BinaryFormatter();
        //        //        stream.Position = 0;
        //        //        stream.Seek(0, SeekOrigin.Begin);
        //        //        CNetworkMessage msgObj = (CNetworkMessage)binFormat.Deserialize(stream);
        //        //        //消息广播到MessageFactory
        //        //        if (m_factory != null)
        //        //            m_factory.AnalyseNetworkMessage(proxSocket, msgObj);
        //        //        //采用消息对的形式响应
        //        //        if (m_WaitForResponse.Count > 0)
        //        //        {
        //        //            m_WaitForResponse[0].ReceiverCallBack(this, new CNetworkMessageEventArgs() { NetworkMsg = msgObj });
        //        //            m_WaitForResponse.RemoveAt(0);
        //        //        }

        //        //    }
        //        //}
        //        m_ReceiveDone.Set();
        //        m_ResponseDone.Set();

        //    }
        //    catch (Exception e)
        //    {
        //        StateObject state = (StateObject)ar.AsyncState;
        //        Socket proxSocket = state.workSocket;
        //        IPEndPoint ipEndPoint = proxSocket.RemoteEndPoint as IPEndPoint;
        //        if (proxSocket != null && !proxSocket.Connected && ipEndPoint != null)
        //        {
        //            CDebug.Log("服务器iP为:{0} Port为:{1}断开连接", ipEndPoint.Address, ipEndPoint.Port);
        //            proxSocket.Shutdown(SocketShutdown.Both);
        //            proxSocket.Close();
        //        }
        //    }
        //}
        public static IEnumerator WaitForResponse(CNetworkMessageResponseHandler responseMsg)
        {
            double m_WaitTime = 0;
            m_StartWaitTime = DateTime.Now;
            while (!responseMsg.IsResponse() && m_WaitTime < 2)
            {
                m_WaitTime = DateTime.Now.Subtract(m_StartWaitTime).TotalSeconds;
                yield return null;
            }

        }
        public void Stop()
        {
            m_TimerCount.Stop();
            //关闭socket
            if (m_SocketMain != null)
            {
                if (m_SocketMain.Connected)
                {
                    m_SocketMain.Shutdown(SocketShutdown.Both);
                    m_SocketMain.Close();
                }
            }
            //关闭线程
            if (m_ThreadSendReceive != null)
                m_ThreadSendReceive.Abort();
            IsConnected = false;
        }
    }
}
