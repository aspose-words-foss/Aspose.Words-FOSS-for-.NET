// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2010 by Roman Korchagin

using System;
using Aspose.Images.Pal;

namespace Aspose.Drawing
{
    /// <summary>
    /// This object holds a lot of memory and should not be held in memory for too long.
    ///
    /// This is a very dumb, but very memory expensive object that contains all image pixels values.
    /// At the moment it contains pixels in a format suitable for saving to PDF.
    /// But I am keeping this class here because I want to put platform-specific image extraction into
    /// <see cref="BitmapPal"/> and I want it to return the parsed pixels in a platform-neutral way.
    /// </summary>
    public class DrPixels
    {
        public DrPixels(
            byte[] colorValues,
            byte[] alphaValues,
            bool hasTransparentPixels,
            ColorModel colorModel,
            int bitsPerComponent,
            DrColor[] colorTable)
        {
            mColorValues = colorValues;
            mAlphaValues = alphaValues;
            mHasTransparentPixels = hasTransparentPixels;
            mColorModel = colorModel;

            if (colorModel == ColorModel.Indexed && bitsPerComponent != 1 && bitsPerComponent != 4 && bitsPerComponent != 8)
                throw new ArgumentException("Wrong bitsPerComponent value with indexed color model");

            mBitsPerComponent = bitsPerComponent;
            mColorTable = colorTable;
        }

        /// <summary>
        /// Returns an array containing either RGB values for RGB bitmaps or color indices for indexed bitmaps.
        /// </summary>
        public byte[] ColorValues
        {
            get { return mColorValues; }
        }

        private readonly byte[] mColorValues;
        private byte[] mAlphaValues;
        private bool mHasTransparentPixels;
        private readonly ColorModel mColorModel;
        private readonly int mBitsPerComponent;
        private readonly DrColor[] mColorTable;
    }
}
