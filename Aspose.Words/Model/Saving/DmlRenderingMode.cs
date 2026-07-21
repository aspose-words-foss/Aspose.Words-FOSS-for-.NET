// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2013 by Alexey Noskov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how DrawingML shapes are rendered to fixed page formats.
    /// </summary>
    public enum DmlRenderingMode
    {
        /// <summary>
        /// If fall-back shape is available for DrawingML, Aspose.Words renders fall-back shape instead of the DrawingML.
        /// </summary>
        /// <remarks>
        /// Please note that after saving a document to a fixed page format with fall-back DML rendering mode,
        /// DML shapes in the AW document model are permanently replaced with their fall-back counterparts.
        /// As a result, saving the same document again will always use fall-back shapes, even if <see cref="DmlRenderingMode"/> is set to <see cref="DrawingML"/>.
        /// </remarks>
        Fallback = 0,
        /// <summary>
        /// Aspose.Words ignores fall-back shape of DrawingML and renders DrawingML itself.
        /// This is the default mode.
        /// </summary>
        DrawingML = 1
    }
}
