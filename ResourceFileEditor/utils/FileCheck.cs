using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.utils
{
    class FileCheck
    {
        public static Boolean isFile(string name)
        {
            return name.Contains(".");
        }
    }
}
