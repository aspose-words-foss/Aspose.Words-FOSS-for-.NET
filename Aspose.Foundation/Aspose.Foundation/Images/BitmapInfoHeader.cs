// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2006 by Roman Korchagin
//08/04/08 by Alexey Linnik
using System.IO;

namespace Aspose.Images
{
    /// <summary>
    /// Represents BITMAPINFOHEADER object.
    /// BITMAPINFOHEADER contains information about the dimensions and color format of a DIB. 
    /// 
    /// See http://msdn.microsoft.com/en-us/library/aa930622.aspx for details.
    /// </summary>
    internal class BitmapInfoHeader : DibHeader
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Size);
            writer.Write(mWidth);
            writer.Write(mHeight);
            writer.Write((short)mPlanes);
            writer.Write((short)mBitCount);
            writer.Write((int)mCompression);
            writer.Write((int)mSizeImage);
            writer.Write(mXPelsPerMeter);
            writer.Write(mYPelsPerMeter);
            writer.Write((int)mClrUsed);
            writer.Write((int)mClrImportant);
        }

        /// <summary>
        /// Gets the size of the color table that follows this header in the DIB.
        /// </summary>
        public override int ColorTableSize
        {
            get { return ImageUtil.GetPaletteSize(mCompression, mBitCount, (int)mClrUsed); }
        }

        /// <summary>
        /// Specifies the number of bytes required by the structure.
        /// </summary>
        public override int Size
        {
            get { return BitmapInfoHeaderSize; }
        }

        /// <summary>
        /// Specifies the width of the bitmap, in pixels. 
        /// </summary>
        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Specifies the height of the bitmap, in pixels. 
        /// 
        /// CAN BE NEGATIVE. 
        /// If negative, is negative, the bitmap is a top-down DIB and its origin is the upper left corner. 
        /// </summary>
        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }


        /// <summary>
        /// Specifies the number of planes for the target device. 
        /// This value must be set to 1.
        /// </summary>
        public ushort Planes
        {
            get { return mPlanes; }
            set { mPlanes = value; }
        }

        /// <summary>
        /// Specifies the number of bits-per-pixel. 
        /// </summary>
        public ushort BitCount
        {
            get { return mBitCount; }
            set { mBitCount = value; }
        }

        /// <summary>
        /// Specifies the type of compression for a compressed bottom-up bitmap.
        /// </summary>
        public BitmapCompression Compression
        {
            get { return mCompression; }
            set { mCompression = value; }
        }

        /// <summary>
        /// Specifies the size, in bytes, of the image. This may be set to zero for BI_RGB bitmaps. 
        /// </summary>
        public uint SizeImage
        {
            get { return mSizeImage; }
            set { mSizeImage = value; }
        }

        /// <summary>
        /// Specifies the horizontal resolution, in pixels-per-meter.
        /// </summary>
        public int XPelsPerMeter
        {
            get { return mXPelsPerMeter; }
            set { mXPelsPerMeter = value; }
        }

        /// <summary>
        /// Specifies the vertical resolution, in pixels-per-meter.
        /// </summary>
        public int YPelsPerMeter
        {
            get { return mYPelsPerMeter; }
            set { mYPelsPerMeter = value; }
        }

        /// <summary>
        /// Specifies the number of color indexes in the color table that are actually used by the bitmap. 
        /// If this value is zero, the bitmap uses the maximum number of colors corresponding to the value
        /// of the biBitCount member for the compression mode specified by biCompression. 
        /// </summary>
        public uint ClrUsed
        {
            get { return mClrUsed; }
            set { mClrUsed = value; }
        }

        /// <summary>
        /// Specifies the number of color indexes that are required for displaying the bitmap. 
        /// If this value is zero, all colors are required.
        /// </summary>
        public uint ClrImportant
        {
            get { return mClrImportant; }
            set { mClrImportant = value; }
        }

        private int mWidth;
        private int mHeight;
        private ushort mPlanes;
        private ushort mBitCount;
        private BitmapCompression mCompression;
        private uint mSizeImage;
        private int mXPelsPerMeter;
        private int mYPelsPerMeter;
        private uint mClrUsed;
        private uint mClrImportant;
    }
}
