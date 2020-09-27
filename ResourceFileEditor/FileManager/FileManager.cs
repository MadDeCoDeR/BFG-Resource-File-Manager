using ResourceFileEditor.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.FileManager
{
    class FileManager
    {
        public static UInt32 readUint32(Stream stream, int pos)
        {
            byte[] buffer = new byte[4];
            stream.Position = pos;
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static UInt32 readUint32Swapped(Stream stream, int pos)
        {
            byte[] buffer = new byte[4];
            stream.Position = pos;
            stream.Read(buffer, 0, buffer.Length);
            ByteSwap.swapBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static byte[] readByteArray(Stream stream, int pos, int size)
        {
            byte[] buffer = new byte[size];
            stream.Position = pos;
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public static void writeUint32(Stream stream, int pos, UInt32 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Position = pos;
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void writeUint32Swapped(Stream stream, int pos, UInt32 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            ByteSwap.swapBytes(buffer);
            stream.Position = pos;
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void writeByteArray(Stream stream, int pos, byte[] data)
        {
            stream.Position = pos;
            stream.Write(data, 0, data.Length);
        }
    }
}
