// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2016 by Alexey Butalov

namespace Aspose
{
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class ImageConstants
    {
        public const double InchesPerMeter = 100 / 2.54;

        /// <summary>
        /// This is the default resolution for images we create.
        /// We should specify it on all bitmaps we create because new bitmaps
        /// default to the current screen resolution.
        /// </summary>
        public const float StandardResolution = 96.0f;

        /// <summary>
        /// Resolution for bitmap images created from metafiles.
        /// </summary>
        public const int PrintResolution = 300;

        /// <summary>
        /// Size of pict header, when pict is read from file.
        /// </summary>
        public const int PictHeaderLength = 512;
    }
}
