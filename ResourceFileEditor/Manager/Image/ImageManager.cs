using BCnEncoder.Shared;
using ResourceFileEditor.utils;
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
    sealed class ImageManager
    {

        private static readonly UInt32 IMAGE_FILE_MAGIC = (('B' << 0) | ('I' << 8) | ('M' << 16) | (10 << 24));
        public static Stream? LoadImage(Stream file)
        {
            Stream? img = null;

            int index = 0;
            DateTime  timestamp = DateTime.FromBinary((long)FileManager.FileManager.readUint64Swapped(file, index));
            index += 8;
            UInt32 magic = FileManager.FileManager.readUint32Swapped(file, index);
            index += 4;
            if (magic == IMAGE_FILE_MAGIC)
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
                if (texType == (UInt32)TextureType.TT_CUBIC)
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
                //TODO Multilevel Textures
                img = CovertToBitmap(images[0], texType, format, colorFormat);
            }

            return img;
        }

        private static Stream? CovertToBitmap(ImageData image, UInt32 texType, UInt32 format, UInt32 colorFormat)
        {
            if (image.data != null)
            {
                byte[] data = image.data;
                Stream imageStream = new MemoryStream();
                if (format != (UInt32)TextureFormat.FMT_RGBA8 && format != (UInt32)TextureFormat.FMT_RGB565)
                {
                    BCnEncoder.Decoder.BcDecoder decoder = new BCnEncoder.Decoder.BcDecoder();
                    CompressionFormat compressionFormat = CompressionFormat.Bc1WithAlpha;
                    switch ((TextureFormat)format)
                    {
                        case TextureFormat.FMT_DXT5:
                            compressionFormat = CompressionFormat.Bc3;
                            break;
                        case TextureFormat.FMT_DXT1:
                            compressionFormat = CompressionFormat.Bc1;
                            break;
                        case TextureFormat.FMT_ALPHA:
                        case TextureFormat.FMT_LUM8:
                            compressionFormat = CompressionFormat.Bc4;
                            break;
                    }
                    data = decoder.DecodeRaw(image.data, (int)image.width, (int)image.height, compressionFormat).SelectMany(color => color.ToByteArray()).ToArray();
                }
                else if (format == (UInt32)TextureFormat.FMT_RGB565)
                {
                    Stream parsedImageBuffer = new MemoryStream();
                   /* UInt16 red_mask = 0xF800;
                    UInt16 green_mask = 0x7E0;
                    UInt16 blue_mask = 0x1F;*/
                    Stream imageBuffer = new MemoryStream(image.data);
                    int index = 0;
                    while (index < imageBuffer.Length)
                    {
                        byte[] pixelBuffer = new byte[2];
                        imageBuffer.ReadExactly(pixelBuffer, 0, 2);
                        index += 2;
                        UInt16 pixel = BitConverter.ToUInt16(pixelBuffer, 0);
                        byte color = (byte)(pixel >> 8);
                        byte alpha = (byte)(pixel & 255);
                        byte[] parsedPixel = [color, color, color, alpha];
                        parsedImageBuffer.Write(parsedPixel, 0, 4);
                    }
                    parsedImageBuffer.Position = 0;
                    data = new byte[parsedImageBuffer.Length];
                    parsedImageBuffer.ReadExactly(data, 0, (int)parsedImageBuffer.Length);
                }

                StbImageWriteSharp.ImageWriter imageWriter = new StbImageWriteSharp.ImageWriter();
                imageWriter.WriteTga(data, (int)image.width, (int)image.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, imageStream);
                return imageStream;
            }
            return null;
        }
    }
}
