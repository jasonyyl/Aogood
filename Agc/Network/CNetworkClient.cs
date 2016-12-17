using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Aogood.SHLib;
using Aogood.Foundation;

namespace Aogood.Network
{
    public class CNetworkClient : CNetwork
    {
        #region Field

        #endregion

        #region Property
        public bool IsConnected { get; protected set; }
        #endregion

        #region Method
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
            m_SendDone.Reset();

            responseMsg = new CNetworkMessageResponseHandlerT<T>();
            RequestObject request = new RequestObject(sendMsg);
            request.workSocket = m_SocketMain;
            Send(request);

            ResponseObject obj = new ResponseObject();
            obj.workSocket = m_SocketMain;
            obj.msgResponse = responseMsg;
            Receive(obj);

            m_SendDone.WaitOne();
            return m_IsSend;
        }
        protected void OnConnected(IAsyncResult ar)
        {
            Socket socketClient = (Socket)ar.AsyncState;
            if (socketClient != null)
                socketClient.EndConnect(ar);
            MessageReceiveEvent += MessageReceive;
            IsConnected = true;
        }

        private void MessageReceive(ResponseObject obj)
        {
            obj.msgResponse.SetResoneMessage(obj.msgPack.GetMessage(obj.msgPack.MsgContent));
            obj.msgResponse.IsResponse = true;
        }
        public static IEnumerator WaitForResponse(CNetworkMessageResponseHandler responseMsg, float timeOut = 1)
        {
            double m_WaitTime = 0;
            DateTime m_StartWaitTime = DateTime.Now;
            while (!responseMsg.IsResponse && m_WaitTime < timeOut)
            {
                m_WaitTime = DateTime.Now.Subtract(m_StartWaitTime).TotalSeconds;
                yield return null;
            }

        }
        public void Stop()
        {
            //关闭socket
            if (m_SocketMain != null)
            {
                if (m_SocketMain.Connected)
                {
                    m_SocketMain.Shutdown(SocketShutdown.Both);
                    m_SocketMain.Close();
                }
            }
            IsConnected = false;
        }

        #endregion
    }
}
