// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/22/2020 by Artem Ptitsin.

using Aspose.Words.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Provides methods for working with a watermark.
    /// </summary>
    /// <remarks>
    /// It is assumed that work with a watermark will be possible with a section or even with a header in the future.
    /// </remarks>
    internal interface IWatermarkProvider
    {
        /// <summary>
        /// Adds a watermark.
        /// </summary>
        [JavaAttributes.JavaThrows(true)]
        void Add(Shape watermark);

        /// <summary>
        /// Gets the watermark shape or null.
        /// </summary>
        Shape Get();

        /// <summary>
        /// Removes a watermark.
        /// </summary>
        void Remove();
    }
}
