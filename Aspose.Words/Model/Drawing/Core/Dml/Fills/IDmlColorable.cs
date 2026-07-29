// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// Interface for objects using theme colors.
    /// </summary>
    internal interface IDmlColorable
    {
        /// <summary>
        /// Set style color in color placeholders used
        /// in the object.
        /// </summary>
        void ApplyStyleColor(DmlColor styleColor);
    }
}
