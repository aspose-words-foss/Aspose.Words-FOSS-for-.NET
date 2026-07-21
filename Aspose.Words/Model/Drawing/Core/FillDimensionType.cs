// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// The type of a (length) measurement for fill dimensions type.
    /// </summary>
    internal enum FillDimensionType
    {
        /// <summary>
        /// Default size, ignore the values.
        /// </summary>
        Default = 0,  
        /// <summary>
        /// Values are in EMUs.
        /// </summary>
        Emu = 1,  
        /// <summary>
        /// Values are in pixels.
        /// </summary>
        Pixel = 2,  
        /// <summary>
        /// Values are 16.16 fractions of shape size.
        /// </summary>
        ShapeFraction = 3,  
        /// <summary>
        /// Aspect ratio is fixed.
        /// </summary>
        FixedAspect  = 4,  
        /// <summary>
        /// EMUs, fixed aspect ratio.
        /// </summary>
        EmuFixedRatio= 5,  
        /// <summary>
        /// Pixels, fixed aspect ratio.
        /// </summary>
        PixelFixedRatio = 6,  
        /// <summary>
        /// Proportion of shape, fixed aspect ratio.
        /// </summary>
        ShapeFractionFixedRatio = 7,  
        /// <summary>
        /// Aspect ratio is fixed, favor larger size. The values are Fixed.
        /// </summary>
        FixedRatioEnlarge = 8,  
        /// <summary>
        /// EMUs, fixed aspect ratio.
        /// </summary>
        EmuFixedRatioBig = 9,  
        /// <summary>
        /// Pixels, fixed aspect ratio.
        /// </summary>
        PixelFixedRatioBig = 10, 
        /// <summary>
        /// Proportion of shape, fixed aspect ratio.
        /// </summary>
        ShapeFractionFixedRatioBig = 11
    }
}
