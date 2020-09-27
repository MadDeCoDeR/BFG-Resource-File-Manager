using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager
{
    interface Manager
    {
        void loadFile(Stream file);

        void writeFile(Stream file);

        void AddFile(Stream file, string relativePath);

        void CreateFile();

        void DeleteEntry(string relativePath);

        void ExtractEntry(string relativePath, string outputFolder);

        long GetFileSize(string relativePath);

        long GetResourceFileSize();
    }
}
