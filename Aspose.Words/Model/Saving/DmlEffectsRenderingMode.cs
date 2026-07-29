// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/26/2013 by Alexey Noskov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how DrawingML effects are rendered to fixed page formats.
    /// </summary>
    public enum DmlEffectsRenderingMode
    {
        /// <summary>
        /// Rendering of DrawingML effects are simplified.
        /// </summary>
        Simplified = 0,
        /// <summary>
        /// No DrawingML effects are rendered.
        /// </summary>
        None = 1,
        /// <summary>
        /// DrawingML effects are rendered in fine mode which involves advanced processing.
        /// In this mode rendering of effects gives better results but at a higher performance cost than <see cref="Simplified"/> mode.
        /// </summary>
        Fine = 2
    }
}
