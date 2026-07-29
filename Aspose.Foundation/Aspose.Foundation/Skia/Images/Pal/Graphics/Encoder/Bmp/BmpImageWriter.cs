// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2017 by Alexey Noskov

#if NETSTANDARD

using SkiaSharp;
using System;
using System.Drawing;
using System.IO;

namespace Aspose.Images.Pal.Graphics.Encoder.Bmp
{
    internal class BmpImageWriter
    {
        public void SetOutput(Stream output)
        {
            if (output == null)
                throw new ArgumentException("output == null!");

            mStream = new ImageStreamWriter(false, output);
        }

        public void Write(BitmapPal image)
        {
            if (image == null)
                throw new ArgumentException("Image cannot be null");

            this.mImage = image.GetNativeBitmap();

            if (mImage.ColorType != BitmapPal.DefaultColorType)
                throw new ArgumentException("ARGB_8888 is supported only.");

            // Default is using 24 bits per pixel.
            int bitsPerPixel = 24;
            int numBands = 3; // Despite Android's Bitmap.Config.ARGB_8888
            compressionType = BmpConstants.BI_RGB; // Atm uncompressed BI_RGB is supported ONLY
            int paletteEntries = 0;

            Rectangle sourceRegion = new Rectangle(0, 0, mImage.Width, mImage.Height);

            BmpMetadata bmpImageMetadata = new BmpMetadata(image);

            int scaleX = 1;
            int scaleY = 1;

            int minX = 0, minY = 0;

            w = (sourceRegion.Width + scaleX - 1) / scaleX;
            h = (sourceRegion.Height + scaleY - 1) / scaleY;
            int xOffset = sourceRegion.X % scaleX;
            int yOffset = sourceRegion.Y % scaleY;

            // Number of bytes that a scanline for the image written out will have.
            int destScanlineBytes = w * numBands;
            int xPelsPerMeter = bmpImageMetadata.xPixelsPerMeter;
            int yPelsPerMeter = bmpImageMetadata.yPixelsPerMeter;
            int colorsUsed = bmpImageMetadata.colorsUsed > 0 ? bmpImageMetadata.colorsUsed : paletteEntries;
            int colorsImportant = paletteEntries;

            // Calculate padding for each scanline
            int padding = destScanlineBytes % 4;
            if (padding != 0)
                padding = 4 - padding;

            // FileHeader is 14 bytes, BitmapHeader is 40 bytes,
            // add palette size and that is where the data will begin
            int offset = 54 + paletteEntries * 4;

            int imageSize = (destScanlineBytes + padding) * h;
            // actual writing of image data
            int fileSize = imageSize + offset;
            int headerSize = 40;

            isTopDown = false;

            WriteFileHeader(fileSize, offset);
            WriteInfoHeader(headerSize, bitsPerPixel);

            // compression
            mStream.WriteInt(compressionType);

            // imageSize
            mStream.WriteInt(imageSize);

            // xPelsPerMeter
            mStream.WriteInt(xPelsPerMeter);

            // yPelsPerMeter
            mStream.WriteInt(yPelsPerMeter);

            // Colors Used
            mStream.WriteInt(colorsUsed);

            // Colors Important
            mStream.WriteInt(colorsImportant);

            // Writing of actual image data
            int scanlineBytes = w * numBands;

            // Buffer for up to 8 rows of pixels
            byte[] pixels = new byte[scanlineBytes * scaleX];

            // Also create a buffer to hold one line of the data
            // to be written to the file, so we can use array writes.
            bpixels = new byte[destScanlineBytes];

            for (int i = 0; i < h; i++)
            {
                int row = minY + i;

                if (!isTopDown)
                    row = minY + h - i - 1;

                Rectangle srcRect = new Rectangle(minX * scaleX + xOffset, row * scaleY + yOffset, (w - 1) * scaleX + 1, 1);
                GetPixels(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, pixels);
                WritePixels(0, scanlineBytes, bitsPerPixel, pixels, padding, numBands);
            }

            Flush();
        }

        private void WriteFileHeader(int fileSize, int offset)
        {
            // magic value
            mStream.WriteByte('B');
            mStream.WriteByte('M');

            // File size
            mStream.WriteInt(fileSize);

            // reserved1 and reserved2
            mStream.WriteInt(0);

            // offset to image data
            mStream.WriteInt(offset);
        }

        private void WriteInfoHeader(int headerSize, int bitsPerPixel)
        {
            // size of header
            mStream.WriteInt(headerSize);

            // width
            mStream.WriteInt(w);

            // height
            if (isTopDown)
                mStream.WriteInt(-h);
            else
                mStream.WriteInt(h);

            // number of planes
            mStream.WriteShort(1);

            // Bits Per Pixel
            mStream.WriteShort(bitsPerPixel);
        }

        private void GetPixels(int offsetX, int offsetY, int width, int height, byte[] pixels)
        {
            int dstIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = (y + offsetY) * mImage.Width + (x + offsetX);
                    SKColor argbColor = SrcPixels[srcIndex];
                    pixels[dstIndex++] = argbColor.Red; // r
                    pixels[dstIndex++] = argbColor.Green; // g
                    pixels[dstIndex++] = argbColor.Blue; // b
                }
            }
        }

        private void WritePixels(int l, int scanlineBytes, int bitsPerPixel, byte[] pixels, int padding, int numBands)
        {
            int k = 0;
            switch (bitsPerPixel)
            {
                case 24:

                    // RGB case only
                    if (numBands == 3)
                    {
                        for (int j = 0; j < scanlineBytes; j += 3)
                        {
                            // Since BMP needs BGR format
                            bpixels[k++] = pixels[l + 2];
                            bpixels[k++] = pixels[l + 1];
                            bpixels[k++] = pixels[l];
                            l += 3;
                        }
                        mStream.Write(bpixels, 0, scanlineBytes);
                    }
                    break;
                default:
                    break;
            }

            // Write out the padding
            if (compressionType == BmpConstants.BI_RGB || compressionType == BmpConstants.BI_BITFIELDS)
            {
                for (k = 0; k < padding; k++)
                    mStream.WriteByte(0);
            }
        }

        public void Flush()
        {
            mStream.Flush();
        }

        private SKColor[] SrcPixels
        {
            get
            {
                if (mSrcPixels == null)
                    mSrcPixels = mImage.Pixels;

                return mSrcPixels;
            }
        }

        private SKColor[] mSrcPixels;
        private SKBitmap mImage;
        private ImageStreamWriter mStream;
        private int w;
        private int h;
        private int compressionType;
        private bool isTopDown;
        private byte[] bpixels;
    }
}

#endif
