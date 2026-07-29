// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/15/2012 by Denis Darkin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the alignment of an absolutely positioned tab character in a document. 
    /// This alignment value determines the position on the line to which this absolute tab shall advance, 
    /// as well as the alignment of the text after the alignment tab character position.
    /// </summary>
    internal enum AbsolutePositionTabAlignment
    {
        /// <summary>
        /// Specifies that the positional tab should be left aligned on the line relative to the specified base
        /// and that the text at that location shall be left aligned.
        /// </summary>
        Left,

        /// <summary>
        /// Specifies that the positional tab should be center aligned on the line relative to the specified base
        /// and that the text at that location shall be center aligned.
        /// </summary>
        Center,
        /// <summary>
        /// Specifies that the positional tab should be right aligned on the line relative to the specified base
        /// and that the text at that location shall be right aligned.
        /// </summary>
        Right,

        /// <summary>
        /// Defaults to <see cref="Left"/>
        /// </summary>
        /// <remarks>This default is AW specific, it is not defined in OOXML specification.</remarks>
        Default = Left
    }
}
