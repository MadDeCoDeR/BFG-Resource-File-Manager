using BCnEncoder.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.utils
{
    static class Extensions
    {
        public static byte[] ToByteArray(this ColorRgba32 color)
        {
            byte[] bytes = [color.r, color.g, color.b, color.a];
            return bytes;
        }
    }
}
