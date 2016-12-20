using Aogood.SHLib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Aogood.Network
{



    public class CNetwork
    {
        #region Field
        protected bool m_IsSend;
        protected int m_Port;
        protected IPAddress m_IP;
        protected Socket m_SocketMain;
        protected ManualResetEvent m_SendDone = new ManualResetEvent(false);
        public event Action<ResponseObject> MessageReceiveEvent;
        #endregion

        #region Property

        /// <summary>
        /// 当前App的Socket
        /// </summary>
        public Socket SocketMain { get { return m_SocketMain; } }
        /// <summary>
        /// IP
        /// </summary>
        public IPAddress IP { get { return m_IP; } }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get { return m_Port; } }

        #endregion

        #region Public

        public CNetwork()
        {
        }
        public virtual void Send(RequestObject request)
        {
            try
            {
                if (request.workSocket.Connected)
                {
                    request.workSocket.BeginSend(request.msgPack.MsgHead, 0, request.msgPack.MsgHead.Length, 0, new AsyncCallback(SendCallBackHead), request);
                }
            }
            catch (Exception)
            {

            }
        }
        void SendCallBackHead(IAsyncResult ar)
        {
            try
            {
                RequestObject request = (RequestObject)ar.AsyncState;
                int bytesSent = request.workSocket.EndSend(ar);
                if (request.workSocket.Connected)
                {
                    request.workSocket.BeginSend(request.msgPack.MsgContent, 0, request.msgPack.MsgContent.Length, 0, new AsyncCallback(SendCallback), request);
                }
            }
            catch (Exception e)
            {

            }
        }
        void SendCallback(IAsyncResult ar)
        {
            try
            {
                RequestObject request = (RequestObject)ar.AsyncState;
                int bytesSent = request.workSocket.EndSend(ar);
                m_IsSend = true;
                Foundation.CAogoodFactory.Instance.RecycleObject(request);
                m_SendDone.Set();


            }
            catch (Exception e)
            {
                IPEndPoint ipEndPoint = m_SocketMain.RemoteEndPoint as IPEndPoint;
                if (m_SocketMain != null && !m_SocketMain.Connected && ipEndPoint != null)
                {
                    m_SocketMain.Shutdown(SocketShutdown.Both);
                    m_SocketMain.Close();
                }
            }
        }

        public virtual void Receive(ResponseObject state)
        {
            try
            {
                CMessagePackage msgPack = new CMessagePackage();
                state.msgPack = msgPack;
                state.workSocket.BeginReceive(msgPack.MsgHead, 0, msgPack.MsgHead.Length, 0, new AsyncCallback(ReceiveCallBackHead), state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        void ReceiveCallBackHead(IAsyncResult ar)
        {
            ResponseObject state = (ResponseObject)ar.AsyncState;
            int bytesRead = state.workSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                int content = Aogood.Foundation.CMath.BytesToInt(state.msgPack.MsgHead);
                state.msgPack.MsgContent = new byte[content];
                if (state.workSocket.Connected)
                {
                    state.workSocket.BeginReceive(state.msgPack.MsgContent, 0, state.msgPack.MsgContent.Length, 0, new AsyncCallback(ReceiveCallback), state);
                }
            }
        }
        void ReceiveCallback(IAsyncResult ar)
        {
            ResponseObject state = (ResponseObject)ar.AsyncState;
            int bytesRead = state.workSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                MessageReceiveEvent(state);
            }
        }
        #endregion
    }
}