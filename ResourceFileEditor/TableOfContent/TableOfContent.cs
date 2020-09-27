using ResourceFileEditor.utils;
using System;
using System.IO;

namespace ResourceFileEditor.TableOfContent
{
    class TableOfContentEntry
    {
        public string Filename { get; set; }
        public UInt32 filePos { get; set; }
        public UInt32 fileSize { get; set; }

        public Stream file { get; set; }

        public static TableOfContentEntry[] parseBytes(byte[] buffer, int size)
        {
            TableOfContentEntry[] tocs = new TableOfContentEntry[size];
            int pos = 0;
            byte[] tempBuffer = new byte[4];
            for (int i = 0; i < size; i++)
            {
                tocs[i] = new TableOfContentEntry();
                int filenameLength = BitConverter.ToInt32(buffer, pos);
                pos += 4;
                tocs[i].Filename = System.Text.Encoding.UTF8.GetString(buffer, pos, filenameLength);
                pos += filenameLength;
                Array.Copy(buffer, pos, tempBuffer, 0, 4);
                ByteSwap.swapBytes(tempBuffer);
                tocs[i].filePos = BitConverter.ToUInt32(tempBuffer, 0);
                pos += 4;
                Array.Copy(buffer, pos, tempBuffer, 0, 4);
                ByteSwap.swapBytes(tempBuffer);
                tocs[i].fileSize = BitConverter.ToUInt32(tempBuffer, 0);
                pos += 4;
            }
            return tocs;
        }

        public long GetByteSize()
        {
            return 12 + Filename.Length;
        }

        public byte[] parseToBytes()
        {

            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[4];

            ms.Write(BitConverter.GetBytes(Filename.Length), 0, 4);
            buffer = System.Text.Encoding.UTF8.GetBytes(Filename);
            ms.Write(buffer, 0, Filename.Length);
            buffer = BitConverter.GetBytes(filePos);
            ByteSwap.swapBytes(buffer);
            ms.Write(buffer, 0, 4);
            buffer = BitConverter.GetBytes(fileSize);
            ByteSwap.swapBytes(buffer);
            ms.Write(buffer, 0, 4);

            return ms.ToArray();
        }
    }
}
