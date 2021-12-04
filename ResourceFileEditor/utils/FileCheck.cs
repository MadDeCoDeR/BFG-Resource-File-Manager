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
using System;
using System.IO;

namespace ResourceFileEditor.utils
{
    class FileCheck
    {

        public enum FileTypes
        {
           IMAGE, UNKNOWN, AUDIO
        }
        public static Boolean isFile(string name)
        {
            return name.Contains(".");
        }

        public static Boolean isExportableToStandard(string name)
        {
            return name.EndsWith("idwav");
        }

        public static FileTypes getFileType(Stream file, string filename)
        {
            string fileext = filename.Substring(filename.LastIndexOf(".") + 1);
            if (fileext != null)
            {
                switch (fileext)
                {
                    case "tga":
                    //case "bimage": //GK: Not yet
                    case "jpg":
                    case "png":
                        return FileTypes.IMAGE;
                    case "wav":
                    case "idwav":
                        return FileTypes.AUDIO;
                }
            }
            return FileTypes.UNKNOWN;
        }

        public static string getPathSeparator()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return "/";
            }
            return "\\";
        }
    }
}
