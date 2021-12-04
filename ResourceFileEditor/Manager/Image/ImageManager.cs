using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager.Image
{
    class ImageManager
    {

        private static readonly UInt32 FILE_MAGIC = (('B' << 0) | ('I' << 8) | ('M' << 16) | (10 << 24));
        public Bitmap LoadImage(Stream file)
        {
            Bitmap img = null;

            int index = 0;
            DateTime  timestamp = DateTime.FromBinary((long)FileManager.FileManager.readUint64Swapped(file, index));
            index += 8;
            UInt32 magic = FileManager.FileManager.readUint32Swapped(file, index);
            index += 4;
            if (magic == FILE_MAGIC)
            {
                UInt32 texType = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                UInt32 format = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                UInt32 colorFormat = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                UInt32 width = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                UInt32 height = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                UInt32 numLevels = FileManager.FileManager.readUint32Swapped(file, index);
                index += 4;
                if (texType == 2)
                {
                    numLevels *= 6;
                }

                List<ImageData> images = new List<ImageData>();
                for (int i = 0; i < numLevels; i++)
                {
                    images.Add(new ImageData());
                    images[i].level = FileManager.FileManager.readUint32Swapped(file, index);
                    index += 4;
                    images[i].destZ = FileManager.FileManager.readUint32Swapped(file, index);
                    index += 4;
                    images[i].width = FileManager.FileManager.readUint32Swapped(file, index);
                    index += 4;
                    images[i].height = FileManager.FileManager.readUint32Swapped(file, index);
                    index += 4;
                    images[i].dataSize = FileManager.FileManager.readUint32Swapped(file, index);
                    index += 4;
                    images[i].data = FileManager.FileManager.readByteArray(file, index, (int)images[i].dataSize);
                    index += (int)images[i].dataSize;
                }
                img = CovertToBitmap(images[0], texType, format, colorFormat);
            }

            return img;
        }

        private Bitmap CovertToBitmap(ImageData image, UInt32 texType, UInt32 format, UInt32 colorFormat)
        {
            byte[] data = image.data;
            if (texType == 1 && format == 8 && colorFormat == 1)
            {

            }
            /*// Convert rgba to bgra
            for (int i = 0; i < (int)image.width * (int)image.height; ++i)
            {
                byte r = data[i * 4];
                byte g = data[i * 4 + 1];
                byte b = data[i * 4 + 2];
                byte a = data[i * 4 + 3];


                data[i * 4] = b;
                data[i * 4 + 1] = g;
                data[i * 4 + 2] = r;
                data[i * 4 + 3] = a;
            }*/

            // Create Bitmap
            Bitmap bmp = new Bitmap((int)image.width, (int)image.height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, (int)image.width, (int)image.height), ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            Marshal.Copy(data, 0, bmpData.Scan0, (int)image.dataSize);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}
