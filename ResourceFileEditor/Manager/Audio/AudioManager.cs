using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager.Audio
{
    sealed class AudioManager
    {
        const UInt32 SOUND_MAGIC_IDMSA = 0x6D7A7274;
        public static Stream LoadFile(Stream file)
        {
            MemoryStream data = new MemoryStream();
            int index = 0;
            int extSize = 2;
            
            UInt32 magic = FileManager.FileManager.readUint32Swapped(file, index);
            if (magic == SOUND_MAGIC_IDMSA)
            {
                setWaveHeader(data);
            
                WaveFormat waveFormat = new WaveFormat();
                index += 4;
                DateTime timestamp = DateTime.FromBinary((long)FileManager.FileManager.readUint64Swapped(file, index));
                index += 8;
                bool loaded = BitConverter.ToBoolean(FileManager.FileManager.readByteArray(file, index, 1), 0);
                index += 1;
                int playBegin = FileManager.FileManager.readIntSwapped(file, index);
                index += 4;
                int playLength = FileManager.FileManager.readIntSwapped(file, index);
                index += 4;
                byte[] fbr = FileManager.FileManager.readByteArray(file, index, WaveFormat.Basic.classSize);
                data.Write(fbr, 0, WaveFormat.Basic.classSize);
                index += WaveFormat.Basic.classSize;
                waveFormat.basic = WaveFormat.Basic.fromByteArray(fbr);
                switch(waveFormat.basic.formatTag)
                {
                    case (ushort)WaveFormat.Basic.FormatTag.FORMAT_PCM:
                        extSize = 0;
                        break;
                    case (ushort)WaveFormat.Basic.FormatTag.FORMAT_ADPCM:
                        data.Write(BitConverter.GetBytes((short)WaveFormat.Extra.Adpcm.classSize), 0, 2);
                        waveFormat.extraSize = FileManager.FileManager.readUint16(file, index);
                        index += 2;
                        byte[] abr = FileManager.FileManager.readByteArray(file, index, WaveFormat.Extra.Adpcm.classSize);
                        index += WaveFormat.Extra.Adpcm.classSize;
                        data.Write(abr, 0, WaveFormat.Extra.Adpcm.classSize);
                        extSize += WaveFormat.Extra.Adpcm.classSize;
                        break;
                    case (ushort)WaveFormat.Basic.FormatTag.FORMAT_XMA2:
                        data.Write(BitConverter.GetBytes((short)WaveFormat.Extra.Xma2.classSize), 0, 2);
                        waveFormat.extraSize = FileManager.FileManager.readUint16(file, index);
                        index += 2;
                        byte[] xbr = FileManager.FileManager.readByteArray(file, index, WaveFormat.Extra.Xma2.classSize);
                        index += WaveFormat.Extra.Xma2.classSize;
                        data.Write(xbr, 0, WaveFormat.Extra.Xma2.classSize);
                        extSize += WaveFormat.Extra.Xma2.classSize;
                        break;
                    case (ushort)WaveFormat.Basic.FormatTag.FORMAT_EXTENSIBLE:
                        data.Write(BitConverter.GetBytes((short)WaveFormat.Extra.Extensible.classSize), 0, 2);
                        waveFormat.extraSize = FileManager.FileManager.readUint16(file, index);
                        index += 2;
                        byte[] ebr = FileManager.FileManager.readByteArray(file, index, WaveFormat.Extra.Extensible.classSize);
                        index += WaveFormat.Extra.Extensible.classSize;
                        data.Write(ebr, 0, WaveFormat.Extra.Extensible.classSize);
                        extSize += WaveFormat.Extra.Extensible.classSize;
                        break;
                }
                data.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                int amplitudeNum = FileManager.FileManager.readIntSwapped(file, index);
                index += 4;
                byte[] amplitudes = FileManager.FileManager.readByteArray(file, index, amplitudeNum);
                index += amplitudeNum;
                int totalBufferSize = FileManager.FileManager.readIntSwapped(file, index);
                index += 4;
                data.Write(BitConverter.GetBytes(totalBufferSize), 0, 4);
                int bufferNum = FileManager.FileManager.readIntSwapped(file, index);
                index += 4;
                GeneratedBuffer[] generatedBuffers = new GeneratedBuffer[bufferNum];
                for (int i = 0; i < bufferNum; i++)
                {
                    generatedBuffers[i] = new GeneratedBuffer();
                    generatedBuffers[i].numSamples = FileManager.FileManager.readIntSwapped(file, index);
                    index += 4;
                    generatedBuffers[i].bufferSize = FileManager.FileManager.readIntSwapped(file, index);
                    index += 4;
                    generatedBuffers[i].buffer = FileManager.FileManager.readByteArray(file, index, generatedBuffers[i].bufferSize);
                    if (generatedBuffers[i].buffer != null)
                    {
                        data.Write(generatedBuffers[i].buffer!, 0, generatedBuffers[i].bufferSize);
                    }
                    
                }
                if (waveFormat.basic.formatTag == (ushort)WaveFormat.Basic.FormatTag.FORMAT_XMA2)
                {
                    data.Write(Encoding.ASCII.GetBytes("seek"), 0 , 4);
                    for (int i = 0; i < bufferNum; i++)
                    {
                        data.Write(BitConverter.GetBytes(generatedBuffers[i].numSamples), 0, 4);
                    }
                }
            }
           
            data.Position = 4;
            data.Write(BitConverter.GetBytes(data.Length - 8), 0, 4);
            data.Position = 16;
            data.Write(BitConverter.GetBytes(WaveFormat.Basic.classSize + extSize), 0, 4);
            data.Position = 0;
            return data;
        }

        private static void setWaveHeader(MemoryStream data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes("RIFF");
            data.Write(buffer, 0, 4);
            data.Write(BitConverter.GetBytes(0), 0, 4); //Stream size TBF
            buffer = Encoding.ASCII.GetBytes("WAVE");
            data.Write(buffer, 0, 4);
            buffer = Encoding.UTF8.GetBytes("fmt ");
            data.Write(buffer, 0, 4);
            data.Write(BitConverter.GetBytes(WaveFormat.Basic.classSize), 0, 4);
        }
    }
}
