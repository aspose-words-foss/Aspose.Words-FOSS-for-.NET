// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/12/2006 by Vladimir Averkin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the layout mode for a section allowing to define the document grid behavior.
    /// </summary>
    public enum SectionLayoutMode
    {
        /// <summary>
        /// Specifies that no document grid shall be applied to the contents of the corresponding section in the document.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Specifies that the corresponding section shall have both the additional line pitch and character pitch
        /// added to each line and character within it in order to maintain a specific number
        /// of lines per page and characters per line.
        /// Characters will not be automatically aligned with gridlines on typing.
        /// </summary>
        Grid = 1,

        /// <summary>
        /// Specifies that the corresponding section shall have additional line pitch added to each line within it
        /// in order to maintain the specified number of lines per page.
        /// </summary>
        LineGrid = 2,

        /// <summary>
        /// Specifies that the corresponding section shall have both the additional line pitch and character pitch
        /// added to each line and character within it in order to maintain a specific number
        /// of lines per page and characters per line. 
        /// Characters will be automatically aligned with gridlines on typing.
        /// </summary>
        SnapToChars = 3,
    }
}
