﻿using System;
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
        public byte[] MsgHead { get { return m_MsgHead; } set { m_MsgHead = value; } }
        /// <summary>
        /// 信息内容
        /// </summary>
        public byte[] MsgContent { get { return m_MsgContent; } set { m_MsgContent = value; } }
        byte[] m_MsgHead;
        byte[] m_MsgContent;

        public CMessagePackage()
        {
            m_MsgHead = new byte[4];
        }
        public CMessagePackage(CNetworkMessage msg)
        {
            m_MsgContent = GetBytes(msg);
            m_MsgHead = Aogood.Foundation.CMath.IntToBytes(m_MsgContent.Length);
        }

        public byte[] GetBytes(CNetworkMessage msg)
        {
            byte[] buffer;
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                binFormat.Serialize(stream, msg);
                buffer = new byte[stream.GetBuffer().Length];
                buffer = stream.GetBuffer();
            }
            return buffer;
        }

        public CNetworkMessage GetMessage(byte[] buffer)
        {
            CNetworkMessage msg;
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                stream.Position = 0;
                stream.Seek(0, SeekOrigin.Begin);
                msg = (CNetworkMessage)binFormat.Deserialize(stream);
            }
            return msg;
        }

    }
}
