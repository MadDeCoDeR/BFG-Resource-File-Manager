using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.utils
{
    class ByteSwap
    {
        public static void swapBytes(byte[] buffer)
        {
            for (int i = 0; i < (buffer.Length / 2); i++)
            {
                swapSingleBytes(ref buffer[i], ref buffer[buffer.Length - (i + 1)]);
            }
        }

        private static void swapSingleBytes(ref byte byte1, ref byte byte2)
        {
            byte temp = byte1;
            byte1 = byte2;
            byte2 = temp;
        }
    }
}
