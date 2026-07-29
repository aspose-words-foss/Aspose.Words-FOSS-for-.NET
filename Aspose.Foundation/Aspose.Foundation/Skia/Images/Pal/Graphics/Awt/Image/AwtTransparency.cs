// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal enum AwtTransparency
    {
        /// <summary>
        /// Represents image data that is guaranteed to be completely opaque,
        /// meaning that all pixels have an alpha value of 1.0.
        /// </summary>
        Opaque = 1,

        /// <summary>
        /// Represents image data that is guaranteed to be either completely
        /// opaque, with an alpha value of 1.0, or completely transparent,
        /// with an alpha value of 0.0.
        /// </summary>
        Bitmask = 2,

        /// <summary>
        /// Represents image data that contains or might contain arbitrary
        /// alpha values between and including 0.0 and 1.0.
        /// </summary>
        Translucent = 3
    }
}
#endif
