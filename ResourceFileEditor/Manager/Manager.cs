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
using System.IO;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager
{
    interface Manager
    {
        void loadFile(Stream file);

        void writeFile(Stream file);

        void AddFile(Stream file, string relativePath);

        void CreateFile();

        void CloseFile();

        void DeleteEntry(string relativePath);

        Task ExtractFolder(string relativePath, string outputFolder);

        Task ExtractEntry(string relativePath, string outputFolder);

        Task ExtractAndExportFolder(string relativePath, string outputFolder);

        Task ExportEntry(string relativePath, string outputFolder);

        Stream loadEntry(string relativePath);

        void updateEntry(string relativePath, Stream data);

        long GetFileSize(string relativePath);

        long GetResourceFileSize();

        string GetResourceFileName();

        string GetResourceFullPath();
    }
}
