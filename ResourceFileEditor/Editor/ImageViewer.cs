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
using ResourceFileEditor.Manager.Image;
using ResourceFileEditor.utils;
using StbImageSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ResourceFileEditor.Editor
{
    class ImageViewer : Editor
    {
        private ManagerImpl manager;

        public ImageViewer(ManagerImpl manager)
        {
            this.manager = manager;
        }
        public void start(Panel panel, TreeNode node)
        {
            string relativePath = PathParser.NodetoPath(node);
            Stream file = manager.loadEntry(relativePath);
            PictureBox pictureBox = new PictureBox();
            pictureBox.Width = panel.Width;
            pictureBox.Height = panel.Height;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Location = new Point(0, 0);
            pictureBox.BackColor = Color.AntiqueWhite;
            pictureBox.BackgroundImage = generateBackgound();
            if (relativePath.EndsWith("bimage"))
            {
                //TODO
                ImageManager imageManager = new ImageManager();
                pictureBox.Image = imageManager.LoadImage(file);
            }
            else
            {
                pictureBox.Image = this.loadBitmap(file);
            }
            pictureBox.ParentChanged += new EventHandler(disposeImage);
            panel.Controls.Add(pictureBox);
        }

        private void disposeImage(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Parent == null && ((PictureBox)sender).Image != null)
            {
                ((PictureBox)sender).Image.Dispose();
                ((PictureBox)sender).Image = null;
                ((PictureBox)sender).BackgroundImage.Dispose();
                ((PictureBox)sender).BackgroundImage = null;
            }
        }

        private Bitmap loadBitmap(Stream file)
        {
            ImageResult imageResult = ImageResult.FromStream(file, ColorComponents.RedGreenBlueAlpha);
            byte[] data = imageResult.Data;
            // Convert rgba to bgra
            for (int i = 0; i < imageResult.Width * imageResult.Height; ++i)
            {
                byte r = data[i * 4];
                byte g = data[i * 4 + 1];
                byte b = data[i * 4 + 2];
                byte a = data[i * 4 + 3];


                data[i * 4] = b;
                data[i * 4 + 1] = g;
                data[i * 4 + 2] = r;
                data[i * 4 + 3] = a;
            }
            // Create Bitmap
            Bitmap bmp = new Bitmap(imageResult.Width, imageResult.Height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, imageResult.Width, imageResult.Height), ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            Marshal.Copy(imageResult.Data, 0, bmpData.Scan0, bmpData.Stride * bmp.Height);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private Bitmap generateBackgound()
        {
            int size = 20;
            Bitmap backgound = new Bitmap(size * 2, size * 2);
            using (SolidBrush brush = new SolidBrush(Color.LightGray))
            using (Graphics G = Graphics.FromImage(backgound))
            {
                G.FillRectangle(brush, 0, 0, size, size);
                G.FillRectangle(brush, size, size, size, size);
            }
            return backgound;
        }
    }
}
