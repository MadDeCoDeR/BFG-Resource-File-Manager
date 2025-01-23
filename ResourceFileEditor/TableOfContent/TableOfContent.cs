/*
===========================================================================

BFG Resource File Manager GPL Source Code
Copyright (C) 2021 George Kalampokis

This file is part of the BFG Resource File Manager GPL Source Code ("BFG Resource File Manager Source Code").

BFG Resource File Manager Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

BFG Resource File Manager Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with BFG Resource File Manager Source Code.  If not, see <http://www.gnu.org/licenses/>.

===========================================================================
*/
using ResourceFileEditor.utils;
using System;
using System.IO;

namespace ResourceFileEditor.TableOfContent
{
    sealed class TableOfContentEntry
    {
        public string? Filename { get; set; }
        public UInt32 filePos { get; set; }
        public UInt32 fileSize { get; set; }

        public Stream? file { get; set; }

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
            if (Filename != null)
            {
                return 12 + Filename.Length;
            }
            return 0;
        }

        public byte[] parseToBytes()
        {
            if (Filename != null)
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
            return [];
        }
    }
}
