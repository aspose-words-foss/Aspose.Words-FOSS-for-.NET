// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2021 by Dmitry Burov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how ink (InkML) objects are rendered to fixed page formats.
    /// </summary>
    public enum ImlRenderingMode
    {
        /// <summary>
        /// If fall-back shape is available for ink (InkML) object, Aspose.Words renders fall-back shape instead of the InkML.
        /// </summary>
        /// <remarks>
        /// Please note that after saving a document to a fixed page format with fall-back rendering mode,
        /// InkML objects in the AW document model are permanently replaced with their fall-back counterparts.
        /// As a result, saving the same document again will always use fall-back shapes, even if <see cref="ImlRenderingMode"/> is set to <see cref="InkML"/>.
        /// </remarks>
        Fallback = 0,
        /// <summary>
        /// Aspose.Words ignores fall-back shape of ink (InkML) object and renders InkML itself.
        /// This is the default mode.
        /// </summary>
        InkML = 1
    }
}
