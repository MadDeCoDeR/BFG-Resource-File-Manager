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
namespace ResourceFileEditor.utils
{
    sealed class ByteSwap
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
