// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2016 by Alexey Butalov

using System.Drawing;
using System.Runtime.InteropServices;
using Aspose.Fonts;
using Aspose.JavaAttributes;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Interface for the font provider.
    /// This interface is extracted from IFontProvider to break circular dependency Aspose.Fonts from Aspose.Drawing.
    /// </summary>
    [ComVisible(false)]
    public interface IDrFontProvider : IFontProvider
    {
        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        [JavaThrows(true)]
        DrFont FetchDrFont(string familyName, float sizePoints, FontStyle style);

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        [JavaThrows(true)]
        DrFont FetchDrFont(string familyName, float sizePoints, FontStyle actualStyle, FontStyle fontFaceStyle,
                           bool isVertical, bool useWord97FontMetrics);
    }
}
