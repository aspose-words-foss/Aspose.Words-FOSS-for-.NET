// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2009 by Alexey Noskov

using System.IO;
using Aspose.IO;

namespace Aspose.Images
{
    /// <summary>
    /// Class is used to read data from TIFF image file directory (IFD).
    /// It helps to read TIFF resolution and check TIFF conformance.
    /// </summary>
    public class TiffDataReader
    {
        public TiffDataReader(byte[] imageBytes) :
            this(new MemoryStream(imageBytes))
        {
        }

        public TiffDataReader(Stream imageStream) :
            this(new BigEndianBinaryReader(imageStream))
        {
        }

        public TiffDataReader(BigEndianBinaryReader reader)
        {
            mTiffDataOffset = reader.BaseStream.Position;
            ReadTiffFields(reader);
        }

        private void ReadTiffFields(BigEndianBinaryReader reader)
        {
            // First two bytes of tiff file indicates byte order that is used within a file.
            short byteOrder = reader.ReadInt16();

            // If it is 0x4949 little-endian is used, if 0x4d4d - big-endian.
            bool isBigEndian = (byteOrder == 0x4d4d);

            // Next two bytes always = 42, that further identifies the file as a TIFF file.
            ushort tiffIdentifier = ReadTiffInt16(reader, isBigEndian);

            // If this value is not 42, then file is invalid and we will stop parsing.
            if (tiffIdentifier != 42)
                return;

            // Next 4 bytes are offset of the first image file directory (IFD).
            uint ifdOffset = ReadTiffInt32(reader, isBigEndian);

            // Move reader to the first IFD.
            reader.BaseStream.Position = mTiffDataOffset + ifdOffset;

            // Get number of TIFF tags within IFD.
            // This is the first 2 bytes in IFD.
            ushort tagsCount = ReadTiffInt16(reader, isBigEndian);

            // Every tag takes up exactly 12 bytes and looks like this.
            //  Offset   |  Datatype          |  Value 
            //  0        |  Word              |  Tag identifying code 
            //  2        |  Word              |  Datatype of tag data 
            //  4        |  Unsigned Long     |  Number of values 
            //  8        |  x * Tag data      |  Tag data
            //           |  datatype          |
            //           |  or Unsigned Long  |
            //           |  offset            | 
            // http://www.awaresystems.be/imaging/tiff/faq.html

            long currentOffset = reader.BaseStream.Position;
            // Read tag by tag.
            for (int tagIdx = 0; tagIdx < tagsCount; tagIdx++)
            {
                // Move to the next tag.
                reader.BaseStream.Position = currentOffset;
                currentOffset += 12;

                // Read tag identifier.
                ushort tagType = ReadTiffInt16(reader, isBigEndian);

                // Read type of tag value.
                ushort tagDataType = ReadTiffInt16(reader, isBigEndian);

                // Next 4 bytes is number of values.
                uint numberOfValues = ReadTiffInt32(reader, isBigEndian);

                // WORDSNET-20914 Skip empty tag reading.
                if (numberOfValues == 0)
                    continue;

                // Determine if value of the field fits 4 bytes.
                // If not, value of the field is offset to the position where the actual value is stored.
                if (((tagDataType == 1 || tagDataType == 2) && numberOfValues > 4) ||
                    ((tagDataType == 3) && numberOfValues > 2) ||
                    ((tagDataType == 4) && numberOfValues > 1) ||
                    (tagDataType == 5))
                {
                    long offset = mTiffDataOffset + ReadTiffInt32(reader, isBigEndian);

                    // WORDSNET-26291
                    // In case of invalid offset just stop reading without the exception.
                    // It'd be better to give a warning here: "Bad TIFF image file directory found."
                    // But currently there's no warning service available at this level.
                    if (offset >= reader.BaseStream.Length)
                        return;

                    reader.BaseStream.Position = offset;
                }

                ReadTag(reader, isBigEndian, tagType, tagDataType, numberOfValues);
            }
        }

        private void ReadTag(BigEndianBinaryReader reader, bool isBigEndian, ushort tagType, ushort tagDataType, uint numberOfValues)
        {
            switch (tagType)
            {
                // Read tags, which are needed to determine size and resolution of the TIFF image.
                case 0x0100:
                    mWidth = tagDataType == 3 ? ReadTiffInt16(reader, isBigEndian) : ReadTiffInt32(reader, isBigEndian);
                    break;
                case 0x0101:
                    mHeight = tagDataType == 3 ? ReadTiffInt16(reader, isBigEndian) : ReadTiffInt32(reader, isBigEndian);
                    break;
                case 0x011A:
                    mXResolution = ReadTiffRational(reader, isBigEndian);
                    break;
                case 0x011B:
                    mYResolution = ReadTiffRational(reader, isBigEndian);
                    break;
                // Read tags, which are needed to check TIFF image conformance.
                case 0x0106:
                    mPhotometricInterpretation = (TiffPhotometricInterpretationCore)ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x0128:
                    mResolutionUnit = (TiffResolutionUnitCore)ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x0103:
                    mCompression = (TiffCompressionCore)ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x0102:
                    mBitsPerSample = ReadTiffInt16Array(reader, isBigEndian, numberOfValues);
                    break;
                case 0x011C:
                    mPlanarConfiguration = (TiffPlanarConfigurationCore)ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x0115:
                    mSamplesPerPixel = ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x014C:
                    mInkSet = (TiffInkSetCore)ReadTiffInt16(reader, isBigEndian);
                    break;
                case 0x014E:
                    mNumberOfInks = ReadTiffInt16(reader, isBigEndian);
                    break;
                // Orientation.
                case 0x0112:
                    mOrientation = ReadTiffInt16(reader, isBigEndian);
                    break;
                default:
                    // Ignore unknown tags.
                    break;
            }
        }

        public static bool IsTiff(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsTiff(stream);
        }

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// </summary>
        public static bool IsTiff(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                // The byte order used within the file. We can use it to detect if this is TIFF or not.
                int byteOrder = reader.ReadInt16();
                return (byteOrder == 0x4949) || (byteOrder == 0x4d4d);
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
        }

        /// <summary>
        /// Returns true, if Tiff image format is 16-bit per sample RGB. 
        /// </summary>
        public bool Is16BppRgb()
        {
            return (mPhotometricInterpretation == TiffPhotometricInterpretationCore.Rgb) &&
                   (mBitsPerSample.Length == 3) &&
                   (mBitsPerSample[0] == 16) &&
                   (mBitsPerSample[1] == 16) &&
                   (mBitsPerSample[2] == 16);
        }

        private static ushort ReadTiffInt16(BigEndianBinaryReader reader, bool isBigEndian)
        {
            ushort value = reader.ReadUInt16();
            if (!isBigEndian)
                value = BitUtil.SwapUInt16(value);

            return value;
        }

        private static uint ReadTiffInt32(BigEndianBinaryReader reader, bool isBigEndian)
        {
            uint value = reader.ReadUInt32();
            if (!isBigEndian)
                value = BitUtil.SwapUInt32(value);

            return value;
        }

        private static ushort[] ReadTiffInt16Array(BigEndianBinaryReader reader, bool isBigEndian, uint numberOfItems)
        {
            ushort[] values = new ushort[(int)numberOfItems];   // Casting to int to allow autoporting to Java.
            for (int i = 0; i < values.Length; i++)
                values[i] = ReadTiffInt16(reader, isBigEndian);
            return values;
        }

        private static double ReadTiffRational(BigEndianBinaryReader reader, bool isBigEndian)
        {
            // Two LONGs: the first represents the numerator of a fraction; the second, the denominator.
            uint numerator = ReadTiffInt32(reader, isBigEndian);
            uint denominator = ReadTiffInt32(reader, isBigEndian);
            // WORDSNET-11932 The problem occurred because denominator in the image is zero, so DivideByZeroException exception occurs.
            // Fixed by adding a condition to avoid dividing by zero.
            return denominator != 0 ? numerator / denominator : 0;
        }

        /// <summary>
        /// Returns Tiff image width. If width is 0, returns default value (100).
        /// </summary>
        public int ImageWidth
        {
            get { return mWidth != 0 ? (int)mWidth : 100; }
        }

        /// <summary>
        /// Returns Tiff image height. If height is 0, returns default value (100).
        /// </summary>
        public int ImageHeight
        {
            get { return mHeight != 0 ? (int)mHeight : 100; }
        }

        /// <summary>
        /// Returns Tiff image orientation. If value is missing, returns default value (Horizontal).
        /// </summary>
        public ExifOrientation Orientation
        {
            get { return mOrientation != 0 ? (ExifOrientation)mOrientation : ExifOrientation.Horizontal; }
        }

        /// <summary>
        /// Returns Tiff image X resolution.
        /// </summary>
        public double ImageXResolution
        {
            // There is tiff with 1 resolution value and ImageResolutionUnit none. It Seems standard resolution must be used in this case.
            // See TestTiffLibTiffPic("Images/Tiff/libtiffpic/strike.tif")
            get { return (ImageResolutionUnit == TiffResolutionUnitCore.None) ? ImageConstants.StandardResolution : mXResolution; }
        }

        /// <summary>
        /// Returns Tiff image Y resolution.
        /// </summary>
        public double ImageYResolution
        {
            get { return (ImageResolutionUnit == TiffResolutionUnitCore.None) ? ImageConstants.StandardResolution : mYResolution; }
        }

        /// <summary>
        /// Returns Tiff photometric tag value
        /// </summary>
        public TiffPhotometricInterpretationCore PhotometricInterpretation
        {
            get { return mPhotometricInterpretation; }
        }

        /// <summary>
        /// Returns Tiff compression tag value
        /// </summary>
        public TiffCompressionCore Compression
        {
            get { return mCompression; }
        }

        /// <summary>
        /// Returns true if TIFF image conforms XPS specification and can be inserted into XPS document as is.
        /// </summary>
        public bool IsConformXpsSpecification
        {
            get
            {
                if (mResolutionUnit != TiffResolutionUnitCore.None &&
                    mResolutionUnit != TiffResolutionUnitCore.Inch &&
                    mResolutionUnit != TiffResolutionUnitCore.Centimeter)
                    return false;

                switch (PhotometricInterpretation)
                {
                    case TiffPhotometricInterpretationCore.BlackIsZero:
                    case TiffPhotometricInterpretationCore.WhiteIsZero:
                        return (IsValidBilevelTiff || IsValidGrayscaleTiff);
                    case TiffPhotometricInterpretationCore.RgbPalette:
                        return IsValidPaletteColorTiff;
                    case TiffPhotometricInterpretationCore.Rgb:
                        return IsValidRgbTiff;
                    case TiffPhotometricInterpretationCore.Cmyk:
                        return IsValidCmykTiff;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns Tiff image resolution unit.
        /// </summary>
        internal TiffResolutionUnitCore ImageResolutionUnit
        {
            get { return mResolutionUnit; }
        }

        /// <summary>
        /// Returns true if image is valid Bilevel Tiff
        /// </summary>
        private bool IsValidBilevelTiff
        {
            get
            {
                // If BitsPerSample is not defined, i.e is 0, this is Bilevel image.
                return mBitsPerSample[0] == 0 && IsCompressionAllowBiLevel;
            }
        }

        /// <summary>
        /// Returns true if image is valid Grayscale Tiff
        /// </summary>
        private bool IsValidGrayscaleTiff
        {
            get
            {
                // In case of Grayscale image, BitsPerSample (258) should be 4, 8 or 16.
                bool isValidBitsPerSample = mBitsPerSample[0] == 4 || mBitsPerSample[0] == 8 || mBitsPerSample[0] == 16;
                return isValidBitsPerSample && IsCompressionAllowGrayscale;
            }
        }

        /// <summary>
        /// Returns true if image is valid Palette Color Tiff
        /// </summary>
        private bool IsValidPaletteColorTiff
        {
            get
            {
                // In case of Palette color image, BitsPerSample (258) should be 1, 4 or 8.
                bool isValidBitsPerSample = mBitsPerSample[0] == 1 || mBitsPerSample[0] == 4 || mBitsPerSample[0] == 8;
                return isValidBitsPerSample && IsCompressionAllowPalette;
            }
        }

        /// <summary>
        /// Returns true if image is valid Rgb Tiff
        /// </summary>
        private bool IsValidRgbTiff
        {
            get
            {
                // PlanarConfiguration (284) should be 1(Chunky).
                if (mPlanarConfiguration != TiffPlanarConfigurationCore.Chunky)
                    return false;

                if (!IsCompressionAllowRgb)
                    return false;

                // In case of RGB image, BitsPerSample (258) should be an array with length 3 or 4.
                // If ExtraSamples (338) is 0 - BitsPerSample should be [8,8,8] or [16,16,16], SamplesPerPixel (277) should be 3.
                // if ExtraSamples is 1 or 2 - BitsPerSample should be [8,8,8,8] or [16,16,16,16], SamplesPerPixel (277) should be 4.
                return IsValidRgbCmykBitsPerSample;
            }
        }

        /// <summary>
        /// Returns true if image is valid CMYK Tiff
        /// </summary>
        private bool IsValidCmykTiff
        {
            get
            {
                // PlanarConfiguration (284) should be 1(Chunky).
                if (mPlanarConfiguration != TiffPlanarConfigurationCore.Chunky)
                    return false;

                if (!IsCompressionAllowCmyk)
                    return false;

                if (!IsValidCmykInks)
                    return false;

                // If ExtraSamples (338) is 0 - BitsPerSample should be [8,8,8,8] or [16,16,16,16], SamplesPerPixel (277) should be 4.
                // if ExtraSamples is 1 or 2 - BitsPerSample should be [8,8,8,8,8] or [16,16,16,16,16], SamplesPerPixel (277) should be 5.
                return IsValidRgbCmykBitsPerSample;
            }
        }

        private bool IsValidCmykInks
        {
            get
            {
                // InkSet (332) should be 1(Cmyk).
                // NumberOfInks (334) should be 4.
                return mInkSet == TiffInkSetCore.Cmyk &&
                       mNumberOfInks == DefaultNumberOfInks;
            }
        }

        /// <summary>
        /// Returns true if all items in bitsPerSample array are equal and is 8 or 16. 
        /// This option has no matter when there is only one item in the BitsPerSample array.
        /// It is used only to check Rgb and Cmyk TIFFs.
        /// </summary>
        private bool IsValidRgbCmykBitsPerSample
        {
            get
            {
                if (mBitsPerSample.Length != mSamplesPerPixel)
                    return false;

                // Check if all items in bitsPerSample array are equal and is 8 or 16.
                // This option has no matter when there is only one item in the BitsPerSample array.
                bool isBitsPerSampleValid = mBitsPerSample[0] == 8 || mBitsPerSample[0] == 16;
                for (int i = 0; i < mBitsPerSample.Length; i++)
                    isBitsPerSampleValid = isBitsPerSampleValid && mBitsPerSample[i] == mBitsPerSample[0];

                return isBitsPerSampleValid;
            }
        }

        private bool IsCompressionAllowBiLevel
        {
            get
            {
                // In case of Bilevel, compression can be 1(None), 2(Rle), 3(Ccitt3), 4(Ccitt4), 5(Lzw) or 32773(PackBits).
                switch (mCompression)
                {
                    case TiffCompressionCore.None:
                    case TiffCompressionCore.Rle:
                    case TiffCompressionCore.Ccitt3:
                    case TiffCompressionCore.Ccitt4:
                    case TiffCompressionCore.Lzw:
                    case TiffCompressionCore.PackBits:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool IsCompressionAllowGrayscale
        {
            get
            {

                // In case of Grayscale image, compression can be 1(None), 2(Rle) or 32773(PackBits).
                switch (mCompression)
                {
                    case TiffCompressionCore.None:
                    case TiffCompressionCore.Rle:
                    case TiffCompressionCore.PackBits:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool IsCompressionAllowPalette
        {
            get
            {
                // For RgbPalette Compression should be 1(None), 5(Lzw) or 32773(PackBits).
                switch (mCompression)
                {
                    case TiffCompressionCore.None:
                    case TiffCompressionCore.Lzw:
                    case TiffCompressionCore.PackBits:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool IsCompressionAllowRgb
        {
            get
            {
                // For RGB compression should be 1(None), 5(Lzw), 6(Jpeg) or 32773(PackBits).
                switch (mCompression)
                {
                    case TiffCompressionCore.None:
                    case TiffCompressionCore.Lzw:
                    case TiffCompressionCore.Jpeg:
                    case TiffCompressionCore.PackBits:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool IsCompressionAllowCmyk
        {
            get
            {
                // For CMYK compression should be 1(None), 5(Lzw), 6(Jpeg) or 32773(PackBits).
                switch (mCompression)
                {
                    case TiffCompressionCore.None:
                    case TiffCompressionCore.Lzw:
                    case TiffCompressionCore.Jpeg:
                    case TiffCompressionCore.PackBits:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// TiffDataReader is also used to read information from EXIF data of JPEG image. 
        /// In this case, Tiff data can be placed not at the beginning of the stream.
        /// </summary>
        private readonly long mTiffDataOffset;

        // Tags, which values are size and resolution of TIFF.
        private uint mWidth; // Tag 0x0100 (decimal 256).
        private uint mHeight; // Tag 0x0101 (decimal 257).
        private ushort mOrientation; // Tag 0x0112 (decimal 274).
        private double mXResolution; // Tag 0x011A (decimal 282).
        private double mYResolution; // Tag 0x011B (decimal 283).

        // Tags, which are needed to check TIFF conformance.
        // If such tags are missed, the default values should be used.
        private TiffPhotometricInterpretationCore mPhotometricInterpretation = TiffPhotometricInterpretationCore.Unspecified; // Tag 0x0106 (decimal 262).
        private TiffResolutionUnitCore mResolutionUnit = TiffResolutionUnitCore.Inch; // Tag 0x0128 (decimal 296).
        private TiffCompressionCore mCompression = TiffCompressionCore.None; // Tag 0x0103 (decimal 259).
        private TiffPlanarConfigurationCore mPlanarConfiguration = TiffPlanarConfigurationCore.Chunky; // Tag 0x011C (decimal 284).
        private TiffInkSetCore mInkSet = TiffInkSetCore.Cmyk; // Tag 0x014C (decimal 332).
        private ushort mSamplesPerPixel = DefaultSamplesPerPixel; // Tag 0x0115 (decimal 277).
        private ushort mNumberOfInks = DefaultNumberOfInks; // Tag 0x014E (decimal 334).
        private ushort[] mBitsPerSample = new ushort[] { 0 }; // Tag 0x0102 (decimal 258).

        // Values used by default.
        private const ushort DefaultSamplesPerPixel = 1;
        private const ushort DefaultNumberOfInks = 4;
    }
}
