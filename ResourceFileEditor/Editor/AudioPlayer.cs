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
using ResourceFileEditor.Manager.Audio;
using ResourceFileEditor.utils;
using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace ResourceFileEditor.Editor
{
    class AudioPlayer : Editor
    {
        private ManagerImpl manager;
        private SoundPlayer audioPlayer;
        private Button playButton;

        public AudioPlayer(ManagerImpl manager)
        {
            this.manager = manager;
        }
        public void start(Panel panel, TreeNode node)
        {
            string relativePath = PathParser.NodetoPath(node);
            Stream file = manager.loadEntry(relativePath);
            file.Seek(0, SeekOrigin.Begin);
            if (relativePath.EndsWith("idwav"))
            {
                file = AudioManager.LoadFile(file);
            }
            audioPlayer = new SoundPlayer(file);
            Panel playerPanel = new Panel();
            playerPanel.Width = panel.Width;
            playerPanel.Height = panel.Height;
            playerPanel.Dock = DockStyle.Fill;
            playerPanel.ParentChanged += new EventHandler(diposePanel);
            playButton = new Button();
            playButton.Text = "play";
            playButton.Location = new System.Drawing.Point(playerPanel.Width / 2, playerPanel.Height / 2);
            playButton.Click += new EventHandler(playPause);
            playerPanel.Controls.Add(playButton);
            panel.Controls.Add(playerPanel);
        }

        private void diposePanel(object sender, EventArgs e)
        {
            if (((Panel)sender).Parent == null)
            {
                audioPlayer.Stop();
                audioPlayer.Dispose();
                audioPlayer = null;
                   
            }
        }

        private void playPause(object sender, EventArgs e)
        {
            audioPlayer.Play();
        }
    }
}
