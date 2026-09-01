// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2017 by Alexey Noskov

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Encoder.Bmp
{
    internal class BmpConstants
    {
        // bmp versions
        internal const string VERSION_2 = "BMP v. 2.x";
        internal const string VERSION_3 = "BMP v. 3.x";
        internal const string VERSION_3_NT = "BMP v. 3.x NT";
        internal const string VERSION_4 = "BMP v. 4.x";
        internal const string VERSION_5 = "BMP v. 5.x";

        // Color space types
        internal const int LCS_CALIBRATED_RGB = 0;
        internal const int LCS_sRGB = 1;
        internal const int LCS_WINDOWS_COLOR_SPACE = 2;
        internal const int PROFILE_LINKED = 3;
        internal const int PROFILE_EMBEDDED = 4;

        // Compression Types
        internal const int BI_RGB = 0;
        internal const int BI_RLE8 = 1;
        internal const int BI_RLE4 = 2;
        internal const int BI_BITFIELDS = 3;
        internal const int BI_JPEG = 4;
        internal const int BI_PNG = 5;

        internal readonly string[] CompressionTypeNames = { "BI_RGB", "BI_RLE8", "BI_RLE4", "BI_BITFIELDS", "BI_JPEG", "BI_PNG" };
    }
}

#endif
