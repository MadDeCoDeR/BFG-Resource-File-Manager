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
