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

        #region Event
        public event Action<string> LogEvent;
        #endregion
        #region Public

        public CNetwork()
        {
        }

        public virtual void Send(RequestObject request)
        {
            Socket workSocket = request.workSocket;
            try
            {
                if (workSocket.Connected)
                {
                    workSocket.BeginSend(request.msgPack.MsgHead, 0, request.msgPack.MsgHead.Length, 0, new AsyncCallback(SendCallBackHead), request);
                }
            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }
        }
        void SendCallBackHead(IAsyncResult ar)
        {
            RequestObject request = (RequestObject)ar.AsyncState;
            Socket workSocket = request.workSocket;
            try
            {
                int bytesSent = workSocket.EndSend(ar);
                if (workSocket.Connected)
                {
                    workSocket.BeginSend(request.msgPack.MsgContent, 0, request.msgPack.MsgContent.Length, 0, new AsyncCallback(SendCallback), request);
                }
            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }
        }
        void SendCallback(IAsyncResult ar)
        {
            RequestObject request = (RequestObject)ar.AsyncState;
            Socket workSocket = request.workSocket;
            try
            {
                int bytesSent = workSocket.EndSend(ar);
                m_IsSend = true;
                Foundation.CAogoodFactory.Instance.RecycleObject(request);
                m_SendDone.Set();
            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }
        }

        public virtual void Receive(ResponseObject state)
        {
            Socket workSocket = state.workSocket;
            try
            {
                CMessagePackage msgPack = new CMessagePackage();
                state.msgPack = msgPack;
                workSocket.BeginReceive(msgPack.MsgHead, 0, msgPack.MsgHead.Length, 0, new AsyncCallback(ReceiveCallBackHead), state);

            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }
        }
        void ReceiveCallBackHead(IAsyncResult ar)
        {
            ResponseObject state = (ResponseObject)ar.AsyncState;
            Socket workSocket = state.workSocket;
            try
            {
                int bytesRead = workSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    int content = Aogood.Foundation.CMath.BytesToInt(state.msgPack.MsgHead);
                    state.msgPack.MsgContent = new byte[content];
                    if (workSocket.Connected)
                    {
                        workSocket.BeginReceive(state.msgPack.MsgContent, 0, state.msgPack.MsgContent.Length, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }
            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }

        }
        void ReceiveCallback(IAsyncResult ar)
        {
            ResponseObject state = (ResponseObject)ar.AsyncState;
            Socket workSocket = state.workSocket;
            try
            {
                int bytesRead = workSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    MessageReceiveEvent(state);
                }
            }
            catch (SocketException e)
            {
                ExpectionOccur(e, workSocket);
            }

        }
        protected void ExpectionOccur(SocketException e, Socket s)
        {
            if (e != null)
            {
                Log("ErrorCode:" + (SocketError)e.ErrorCode + " \nErrorMessage:" + e.Message);
                ExceptionHandle(e, s);
            }

        }
        void ExceptionHandle(SocketException e, Socket s)
        {
            if (s == null)
                return;
            switch (e.SocketErrorCode)
            {
                case SocketError.ConnectionReset:
                    {
                        s.Shutdown(SocketShutdown.Both);
                        s.Close();
                    }
                    break;
                default:
                    break;
            }
        }

        public void Log(string log)
        {
            if (LogEvent != null && !string.IsNullOrEmpty(log))
            {
                LogEvent(log);
            }
        }
        #endregion
    }
}