// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2018 by Dmitry Matveenko

namespace Aspose.Words
{
    /// <summary>
    /// Represents gutter behavior for page margin calculation from the client point of view.
    /// </summary>
    /// <remarks>
    /// There is a tricky condition in MS Word with a side gutter.
    /// MS Word counts it as present in some contexts but disregards in other contexts.
    /// The behavior depends on compatibility mode.
    /// Rather than having multiple conditions to choose the in several places where page margins are used,
    /// the logic is handled by <see cref="PageMarginCalculator"/>. 
    /// This type is used by the caller to specify the required behavior.
    /// </remarks>
    internal enum GutterBehavior
    {
        /// <summary>
        /// Side gutter is counted regardless of the top gutter presence.
        /// </summary>
        /// <remarks>
        /// It is used for page borders, floating table and frames positioning and page metrics.
        /// </remarks>
        Normal,

        /// <summary>
        /// Side gutter is not counted if top gutter is also present.
        /// </summary>
        /// <remarks>
        /// It is used for text width calculation in a section with evenly spaced columns.
        /// </remarks>
        Column,

        /// <summary>
        /// Side gutter is not counted if top gutter is also present in compatibility mode.
        /// </summary>
        /// <remarks>
        /// It is used for header and footer width calculation.
        /// </remarks>
        HeaderFooter,

        /// <summary>
        /// Side gutter is not counted if top gutter is also present in 2013 mode.
        /// </summary>
        /// <remarks>
        /// It is used for floating shape positioning.
        /// </remarks>
        FloatingShapePositioning,
    }
}
