using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager.Audio
{
    sealed class WaveFormat
	{
		public static readonly string id = "fmt ";
        // This is the basic data we'd expect to see in any valid wave file
        public sealed class Basic
		{
			public UInt16 formatTag { get; set; }
			public UInt16 numChannels { get; set; }
			public UInt32 samplesPerSec { get; set; }
			public UInt32 avgBytesPerSec { get; set; }
			public UInt16 blockSize { get; set; }
			public UInt16 bitsPerSample { get; set; }

			public enum FormatTag: UInt16
            {
				FORMAT_UNKNOWN = 0x0000,
				FORMAT_PCM = 0x0001,
				FORMAT_ADPCM = 0x0002,
				FORMAT_FLOAT = 0x0003, //GK:Add support for Float point audio samples
				FORMAT_XMA2 = 0x0166,
				FORMAT_EXTENSIBLE = 0xFFFF,
			}

			public static readonly int classSize = 16;

			public static Basic fromByteArray(byte[] raw)
            {
				Basic basic = new Basic();
				int index = 0;
				basic.formatTag = BitConverter.ToUInt16(raw, index);
				index += 2;
				basic.numChannels = BitConverter.ToUInt16(raw, index);
				index += 2;
				basic.samplesPerSec = BitConverter.ToUInt32(raw, index);
				index += 4;
				basic.avgBytesPerSec = BitConverter.ToUInt32(raw, index);
				index += 4;
				basic.blockSize = BitConverter.ToUInt16(raw, index);
				index += 2;
				basic.bitsPerSample = BitConverter.ToUInt16(raw, index);

				return basic;
            }
		}
		public Basic? basic { get; set; }
		// Some wav file formats have extra data after the basic header
		public UInt16 extraSize { get; set; }
        // We have a few known formats that we handle:
        public sealed class Extra
		{
			// Valid if basic.formatTag == FORMAT_EXTENSIBLE
			public sealed class Extensible
			{
				public UInt16 validBitsPerSample { get; set; }  // Valid bits in each sample container
				public UInt32 channelMask { get; set; }         // Positions of the audio channels
				public sealed class Guid
				{
					public UInt32 data1 { get; set; }
					public UInt16 data2 { get; set; }
					public UInt16 data3 { get; set; }
					public UInt16 data4 { get; set; }
					public byte[]? data5 { get; set; } //size 6

					public static readonly int classSize = 16;
				}           // Format identifier GUID
				public Guid? guid { get; set; }

				public static readonly int classSize = 6 + Guid.classSize;
			}
			public Extensible? extensible { get; set; }
			// Valid if basic.formatTag == FORMAT_ADPCM
			// The microsoft ADPCM struct has a zero-sized array at the end
			// but the array is always 7 entries, so we set it to that size
			// so we can embed it in our format union.  Otherwise, the struct
			// is exactly the same as the one in audiodefs.h
			public sealed class Adpcm
			{
				public UInt16 samplesPerBlock { get; set; }
				public UInt16 numCoef { get; set; }
				public sealed class Adpcmcoef
				{
					public short coef1 { get; set; }
					public short coef2 { get; set; }

					public static readonly int classSize = 4;
				}
				public Adpcmcoef[]? aCoef { get; set; }  // Always 7 coefficient pairs for MS ADPCM

				public static readonly int classSize = 4 + (7 * Adpcmcoef.classSize);

				public static Adpcm fromByteArray(byte[] raw)
				{
					Adpcm adpcm = new Adpcm();
					int index = 0;
					adpcm.samplesPerBlock = BitConverter.ToUInt16(raw, index);
					index += 2;
					adpcm.numCoef = BitConverter.ToUInt16(raw, index);
					index += 2;
					adpcm.aCoef = new Adpcmcoef[adpcm.numCoef];
					for (int i = 0; i < adpcm.numCoef; i++)
                    {
						Adpcmcoef adpcmcoef = new Adpcmcoef();
						adpcmcoef.coef1 = BitConverter.ToInt16(raw, index);
						index += 2;
						adpcmcoef.coef2 = BitConverter.ToInt16(raw, index);
						index += 2;
						adpcm.aCoef[i] = adpcmcoef;
                    }

					return adpcm;
				}
			}
			public Adpcm? adpcm { get; set; }
			// Valid if basic.formatTag == FORMAT_XMA2
			public sealed class Xma2
			{
				public UInt16 numStreams { get; set; }      // Number of audio streams (1 or 2 channels each)
				public UInt32 channelMask { get; set; }     // matches the CHANNEL_MASK enum above
				public UInt32 samplesEncoded { get; set; }  // Total number of PCM samples the file decodes to
				public UInt32 bytesPerBlock { get; set; }   // XMA block size (but the last one may be shorter)
				public UInt32 playBegin { get; set; }       // First valid sample in the decoded audio
				public UInt32 playLength { get; set; }      // Length of the valid part of the decoded audio
				public UInt32 loopBegin { get; set; }       // Beginning of the loop region in decoded sample terms
				public UInt32 loopLength { get; set; }      // Length of the loop region in decoded sample terms
				public byte loopCount { get; set; }    // Number of loop repetitions; 255 = infinite
				public byte encoderVersion { get; set; }    // Version of XMA encoder that generated the file
				public UInt16 blockCount { get; set; }      // XMA blocks in file (and entries in its seek table)
				public static readonly int classSize = 34;

				public static Xma2 fromByteArray(byte[] raw)
				{
					Xma2 xma2 = new Xma2();
					int index = 0;
					xma2.numStreams = BitConverter.ToUInt16(raw, index);
					index += 2;
					xma2.channelMask = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.samplesEncoded = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.bytesPerBlock = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.playBegin = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.playLength = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.loopBegin = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.loopLength = BitConverter.ToUInt32(raw, index);
					index += 4;
					xma2.loopCount = raw[index];
					index += 1;
					xma2.encoderVersion = raw[index];
					index += 1;
					xma2.blockCount = BitConverter.ToUInt16(raw, index);

					return xma2;
				}
			}
			public Xma2? xma2 { get; set; }

			public static readonly int classSize = Extensible.classSize + Adpcm.classSize + Xma2.classSize;
		}
		public Extra? extra { get; set; }
		public static readonly int classSize = Basic.classSize + 2 + Extra.classSize;
	}

    sealed class GeneratedBuffer
    {
		public int bufferSize { get; set; }
		public int numSamples { get; set; }
		public byte[]? buffer { get; set; }
    }
    sealed class AudioModels
    {
    }
}
