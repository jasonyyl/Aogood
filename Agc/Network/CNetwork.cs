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
        protected bool m_IsSend = false;
        protected int m_Port;
        protected IPAddress m_IP;
        protected Socket m_SocketMain;
        protected ManualResetEvent m_SendDone = new ManualResetEvent(false);
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
        public virtual void Send(Socket client, CNetworkMessage message)
        {

            try
            {
                CMessagePackage msgPack = new CMessagePackage(message);
                if (client.Connected)
                {
                    client.BeginSend(msgPack.MsgHead, 0, msgPack.MsgHead.Length, 0, new AsyncCallback(SendCallBackHead), msgPack);
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
                CMessagePackage msgPack = (CMessagePackage)ar.AsyncState;

                if (m_SocketMain.Connected)
                {
                    m_SocketMain.BeginSend(msgPack.MsgContent, 0, msgPack.MsgContent.Length, 0, new AsyncCallback(SendCallback), msgPack);
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
                int bytesSent = m_SocketMain.EndSend(ar);
                m_SendDone.Set();
                m_IsSend = true;
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
        protected virtual void Receive(StateObject state)
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
            StateObject state = (StateObject)ar.AsyncState;
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
            StateObject state = (StateObject)ar.AsyncState;
            int bytesRead = state.workSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                MSG_CTS_CHAT msg = state.msgPack.GetMessage(state.msgPack.MsgContent) as MSG_CTS_CHAT;
                Console.WriteLine(msg.Content);
            }
        }
        #endregion
    }
}