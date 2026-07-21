// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// Represents method for modifying BIDI paragraph character data which can be applied to any stage
    /// of the paragraph's text processing. Although it is not "written in standard", it might be
    /// useful when dealing with text to insert in MS Word document which comes from outer source.
    /// </summary>
    public interface IBidiParagraphCharDataModifier
    {
        /// <summary>
        /// Modifies text data for the given BIDI paragraph.
        /// </summary>
        void Modify(BidiParagraph paragraph, BidiCharData[] textData);
    }
}
