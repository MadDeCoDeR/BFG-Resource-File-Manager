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
    sealed class AudioPlayer : Editor, IDisposable
    {
        private ManagerImpl manager;
        private SoundPlayer? audioPlayer;
        private Button? playButton;

        public AudioPlayer(ManagerImpl manager)
        {
            this.manager = manager;
        }

        public void start(Panel panel, TreeNode node)
        {
            string relativePath = PathParser.NodetoPath(node);
            Stream? file = manager.loadEntry(relativePath);
            if (file != null)
            {
                file.Seek(0, SeekOrigin.Begin);
                if (relativePath.EndsWith("idwav", System.StringComparison.InvariantCulture))
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
        }

        public void Dispose()
        {
            if (audioPlayer != null)
            {
                audioPlayer.Stop();
                audioPlayer.Dispose();
                audioPlayer = null;
            }

            if (playButton != null)
            {
                playButton.Dispose();
                playButton = null;
            }
        }
        private void diposePanel(object? sender, EventArgs e)
        {
            if (sender != null && ((Panel)sender).Parent == null)
            {
                if (audioPlayer != null)
                {
                    audioPlayer.Stop();
                }
                   
            }
        }

        private void playPause(object? sender, EventArgs e)
        {
            if (audioPlayer != null)
            {
                audioPlayer.Play();
            }
        }
    }
}
