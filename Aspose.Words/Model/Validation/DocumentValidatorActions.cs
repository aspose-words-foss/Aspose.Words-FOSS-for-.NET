// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2009 by Roman Korchagin
using System;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Specifies various actions that must be performed on a document when saving in a particular format.
    /// </summary>
    [Flags]
    internal enum DocumentValidatorActions
    {
        None = 0x0000,

        /// <summary>
        /// The table styles need to be expanded into direct formatting when writing.
        /// </summary>
        ExpandTableStyles = 0x0001,

        /// <summary>
        /// The save format requires that the font info collection in a document is updated before saving.
        /// </summary>
        UpdateFontInfo = 0x0002,

        /// <summary>
        /// Causes legacy list level formatting to be expanded into normal list formatting.
        /// </summary>
        ConvertLegacyListFormatting = 0x008,

        /// <summary>
        /// Causes horizontally merged cells to be converted into normal cells.
        /// </summary>
        NormalizeHorizontalMerge = 0x0020,

        /// <summary>
        /// The save format does not support DrawingML, but when this option is turned on,
        /// we can at least convert DrawingML pictures to model shapes.
        /// </summary>
        ConvertDmlPicturesToVml = 0x0040,

        /// <summary>
        /// The save format requires to update list labels.
        /// </summary>
        UpdateListLabels = 0x0080,

        /// <summary>
        /// The save format requires PreferredWidth.Auto value explicitly set.
        /// </summary>
        ExplicitAutoPreferredWidth = 0x0100,

        /// <summary>
        /// Fixes some issues with shapes that is needed for layout and rendering formats.
        /// </summary>
        PrepareShapesForRendering = 0x0400,

        /// <summary>
        /// Causes validator to check table width to be valid value for MS Word.
        /// </summary>
        /// <remarks>
        /// AM. Seems that we need to add this flag later to other conditions, for example,
        /// row height is checked as well.
        /// </remarks>
        WordTableLimits = 0x0800,

        /// <summary>
        /// Allows long (more than 40 characters length) bookmark name.
        /// </summary>
        LongBookmarkNames = 0x1000,

        NonMSWordFormat = ExpandTableStyles | ConvertLegacyListFormatting |
                            ConvertDmlPicturesToVml | UpdateListLabels,

        Rendering = NonMSWordFormat | NormalizeHorizontalMerge | PrepareShapesForRendering
    }
}
