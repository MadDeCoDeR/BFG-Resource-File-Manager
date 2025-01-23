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
using System;
using System.IO;
using System.Windows.Forms;

namespace ResourceFileEditor.Editor
{
    sealed class TextEditor : Editor
    {
        private ManagerImpl manager;
        private EventHandler editTextHandler;
        public TextEditor(ManagerImpl managerImpl)
        {
            this.manager = managerImpl;
            editTextHandler = new EventHandler(editText);

        }
        public void start(Panel panel, TreeNode node)
        {
            TextBox textBox = new TextBox();
            string relativePath = PathParser.NodetoPath(node);
            Stream? file = manager.loadEntry(relativePath);
            if (file != null)
            {
                TextReader textReader = new StreamReader(file);
                textBox.Text = textReader.ReadToEnd();
                textBox.Multiline = true;
                textBox.ScrollBars = ScrollBars.Both;
                textBox.Width = panel.Width;
                textBox.Height = panel.Height;
                textBox.Dock = DockStyle.Fill;
                textBox.ReadOnly = false;
                textBox.Name = relativePath;
                textBox.AcceptsReturn = true;
                textBox.AcceptsTab = true;
                textBox.Capture = true;
                textBox.ImeMode = ImeMode.On;
                textBox.MaxLength = Int32.MaxValue;
                textBox.ParentChanged += new EventHandler(clearText);
                textBox.TextChanged += editTextHandler;
                panel.Controls.Add(textBox);
            }
        }

        private void clearText(object? sender, EventArgs e)
        {
            if (sender != null && ((TextBox)sender).Parent == null)
            {
                ((TextBox)sender).TextChanged -= editTextHandler;
                ((TextBox)sender).Text = null;
                ((TextBox)sender).Dispose();
            }
        }

        private void editText(object? sender, EventArgs e)
        {
            Stream data = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(data);
            if (sender != null)
            {
                streamWriter.Write(((TextBox)sender).Text);
                streamWriter.Flush();
                data.Position = 0;
                manager.updateEntry(((TextBox)sender).Name, data);
            }
        }
    }
}
