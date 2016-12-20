using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aogood.Foundation
{
    public static class CMath
    {
        /// <summary>
        /// int转byte的数组，默认低位在前，高位灾后
        /// </summary>
        public static byte[] IntToBytes(int value, bool isReverse = false)
        {
            byte[] src = new byte[4];
            if (!isReverse)
            {
                src[3] = (byte)((value >> 24) & 0xFF);
                src[2] = (byte)((value >> 16) & 0xFF);
                src[1] = (byte)((value >> 8) & 0xFF);
                src[0] = (byte)(value & 0xFF);
            }
            else
            {
                src[0] = (byte)((value >> 24) & 0xFF);
                src[1] = (byte)((value >> 16) & 0xFF);
                src[2] = (byte)((value >> 8) & 0xFF);
                src[3] = (byte)(value & 0xFF);
            }
            return src;
        }

        public static int BytesToInt(byte[] src, bool isReverse = false)
        {
            int value = 0;
            if (!isReverse)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    value |= (int)((src[i] & 0xFF) << 8 * i);
                }
            }
            else
            {
                for (int i = 0; i < src.Length; i++)
                {
                    value |= (int)((src[src.Length - i] & 0xFF) << 8 * i);
                }
            }
            return value;
        }
    }
}
