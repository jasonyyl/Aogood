using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Aogood.Network
{
    public class CMessagePackage
    {
        /// <summary>
        /// 信息头
        /// </summary>
        public byte[] MsgHead { get { return m_MsgHead; } }
        /// <summary>
        /// 信息内容
        /// </summary>
        public byte[] MsgContent { get { return m_MsgContent; } }
        byte[] m_MsgHead;
        byte[] m_MsgContent;
        public CMessagePackage(CNetworkMessage msg)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                binFormat.Serialize(stream, msg);
                m_MsgContent = new byte[stream.GetBuffer().Length];
                m_MsgContent = stream.GetBuffer();
            }
            m_MsgHead = Aogood.Foundation.CMath.IntToBytes(m_MsgContent.Length);
        }
    }
}
