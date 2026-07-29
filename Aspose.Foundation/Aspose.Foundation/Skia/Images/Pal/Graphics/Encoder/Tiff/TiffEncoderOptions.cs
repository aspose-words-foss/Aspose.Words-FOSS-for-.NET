// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2024 by Denis Panov

#if NETSTANDARD
using BitMiracle.LibTiff.Classic;

namespace Aspose.Images.Pal.Graphics.Encoder.Tiff
{
    /// <summary>
    ///  Allows to specify options for Tiff images encoder.
    /// </summary>
    internal class TiffEncoderOptions
    {
        public int XResolution
        {
            get { return mXResolution; }
            set { mXResolution = value; }
        }

        public int YResolution
        {
            get { return mYResolution; }
            set { mYResolution = value; }
        }

        public Compression Compression
        {
            get { return mCompression; }
            set { mCompression = value; }
        }

        public Photometric Photometric
        {
            get { return IsGrayscale ?  Photometric.MINISWHITE : Photometric.RGB; }
        }

        private bool IsGrayscale
        {
            get
            {
                return (mCompression == Compression.CCITTFAX3) || (mCompression == Compression.CCITTFAX4)
                    || (mCompression == Compression.CCITTRLE);
            }
        }

        private int mXResolution = 96;
        private int mYResolution = 96;
        private Compression mCompression = Compression.LZW;
    }
}
#endif
