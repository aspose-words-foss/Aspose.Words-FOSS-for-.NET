// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2015 by Dmitry Burov

using System.Runtime.InteropServices;
using Aspose.JavaAttributes;

namespace Aspose.Drawing
{
    /// <summary>
    /// An interface provided by various color effects E.g. grayscale, black&amp;white etc.
    /// </summary>
    [ComVisible(false)]
    public interface IDrColorEffect
    {
        /// <summary>
        /// Applies the effect to the specified color.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <returns>The color with effect applied.</returns>
        DrColor Apply(DrColor color);

        /// <summary>
        /// Applies the effect to the specified image.
        /// </summary>
        /// <param name="imageBytes">Source image bytes.</param>
        /// <returns>Images bytes with effect applied.</returns>
        [JavaThrows(true)]
        byte[] Apply(byte[] imageBytes);
    }
}
