using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileEditor.Manager.Image
{
    class ImageData
    {
        public UInt32 level;
        public UInt32 destZ;
        public UInt32 width;
        public UInt32 height;
        public UInt32 dataSize;
        public byte[] data;

    }

    public enum TextureType: UInt32
    {
        TT_DISABLED,
        TT_2D,
        TT_CUBIC
    }

	public enum TextureFormat: UInt32
	{
		FMT_NONE,

		//------------------------
		// Standard color image formats
		//------------------------

		FMT_RGBA8,          // 32 bpp
		FMT_XRGB8,          // 32 bpp

		//------------------------
		// Alpha channel only
		//------------------------

		// Alpha ends up being the same as L8A8 in our current implementation, because straight
		// alpha gives 0 for color, but we want 1.
		FMT_ALPHA,

		//------------------------
		// Luminance replicates the value across RGB with a constant A of 255
		// Intensity replicates the value across RGBA
		//------------------------

		FMT_L8A8,           // 16 bpp
		FMT_LUM8,           //  8 bpp
		FMT_INT8,           //  8 bpp

		//------------------------
		// Compressed texture formats
		//------------------------

		FMT_DXT1,           // 4 bpp
		FMT_DXT5,           // 8 bpp

		//------------------------
		// Depth buffer formats
		//------------------------

		FMT_DEPTH,          // 24 bpp

		//------------------------
		//
		//------------------------

		FMT_X16,            // 16 bpp
		FMT_Y16_X16,        // 32 bpp
		FMT_RGB565,         // 16 bpp
	}

	public enum tTextureColor: UInt32
	{
		CFM_DEFAULT,            // RGBA
		CFM_NORMAL_DXT5,        // XY format and use the fast DXT5 compressor
		CFM_YCOCG_DXT5,         // convert RGBA to CoCg_Y format
		CFM_GREEN_ALPHA,        // Copy the alpha channel to green
	}
}
