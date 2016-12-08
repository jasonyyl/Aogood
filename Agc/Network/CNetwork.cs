using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Aogood.Network
{
    public class CNetwork
    {
        #region Field
        protected int m_Port;
        protected IPAddress m_IP;
        protected Socket m_SocketMain;
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
        public virtual bool Send(Socket client, CNetworkMessage message)
        {

            bool isSend = false;
            byte[] bt;
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                binFormat.Serialize(stream, message);
                bt = new byte[stream.GetBuffer().Length];
                bt = stream.GetBuffer();
            }
            try
            {
                if (client.Connected)
                    client.BeginSend(bt, 0, bt.Length, 0, new AsyncCallback(SendCallback), client);
                isSend = true;
            }
            catch (Exception)
            {
                isSend = false;
            }
            return isSend;
        }
        protected virtual void SendCallback(IAsyncResult ar)
        {

        }
        protected virtual void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        protected virtual void ReceiveCallback(IAsyncResult ar)
        {

        }

        #endregion
    }
}
