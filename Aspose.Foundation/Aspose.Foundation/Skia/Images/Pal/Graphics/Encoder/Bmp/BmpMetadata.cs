// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2017 by Alexey Noskov

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Encoder.Bmp
{
    internal class BmpMetadata
    {
        public BmpMetadata(BitmapPal input)
        {
            bmpVersion = BmpConstants.VERSION_3;
            compression = BmpConstants.BI_RGB;

            // width and height
            width = input.Width;
            height = input.Height;

            // Resolution.
            xPixelsPerMeter = ImageUtil.DpiToPpm(input.HorizontalResolution);
            yPixelsPerMeter = ImageUtil.DpiToPpm(input.VerticalResolution);

            // bitsPerPixel
            bitsPerPixel = (short)24;

            // masks
            redMask = 0x00ff0000;
            greenMask = 0x0000ff00;
            blueMask = 0x000000ff;
            alphaMask = 0xff000000;
        }

        // Fields for Image Descriptor
        public string bmpVersion;
        public int width;
        public int height;
        public short bitsPerPixel;
        public int compression;
        public int imageSize;

        // Fields for PixelsPerMeter
        public int xPixelsPerMeter;
        public int yPixelsPerMeter;

        public int colorsUsed;
        public int colorsImportant;

        // Fields for BI_BITFIELDS compression(Mask)
        public uint redMask;
        public uint greenMask;
        public uint blueMask;
        public uint alphaMask;

        public int colorSpace;

        // Fields for CIE XYZ for the LCS_CALIBRATED_RGB color space
        public double redX;
        public double redY;
        public double redZ;
        public double greenX;
        public double greenY;
        public double greenZ;
        public double blueX;
        public double blueY;
        public double blueZ;

        // Fields for Gamma values for the LCS_CALIBRATED_RGB color space
        public int gammaRed;
        public int gammaGreen;
        public int gammaBlue;

        public int intent;

        // Fields for the Palette and Entries
        public byte[] palette = null;
        public int paletteSize;
        public int red;
        public int green;
        public int blue;

    }
}

#endif
