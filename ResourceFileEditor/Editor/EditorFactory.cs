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
using ResourceFileEditor.Manager;
using ResourceFileEditor.utils;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ResourceFileEditor.Editor
{
    class EditorFactory
    {

        private static Dictionary<FileCheck.FileTypes, Editor> registeredEditors = new Dictionary<FileCheck.FileTypes, Editor>();

        public EditorFactory(ManagerImpl manager) {
            registeredEditors.Add(FileCheck.FileTypes.UNKNOWN, new TextEditor(manager));
            registeredEditors.Add(FileCheck.FileTypes.AUDIO, new AudioPlayer(manager));
            registeredEditors.Add(FileCheck.FileTypes.IMAGE, new ImageViewer(manager));
        }
        public void openEditor(FileCheck.FileTypes type, Panel panel, TreeNode treeNode)
        {
            registeredEditors[type].start(panel, treeNode);
        }
    }
}
