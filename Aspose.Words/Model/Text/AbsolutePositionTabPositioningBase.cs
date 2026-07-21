// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/15/2012 by Denis Darkin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the possible extents which can be used to calculate the absolute positioning of this positional tab character.
    /// </summary>
    internal enum AbsolutePositionTabPositioningBase
    {
        /// <summary>
        /// Specifies that the absolute positioning of the tab shall be relative to the indents.
        /// </summary>
        Indent,

        /// <summary>
        /// Specifies that the absolute positioning of the tab shall be relative to the margins.
        /// </summary>
        Margin,

        /// <summary>
        /// Defaults to <see cref="Indent"/>
        /// </summary>
        /// <remarks>This default is AW specific, it is not defined in OOXML specification.</remarks>
        Default = Indent
    }
}
