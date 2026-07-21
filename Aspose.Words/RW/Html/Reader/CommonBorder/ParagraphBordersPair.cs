// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2015 by Anton Savko

namespace Aspose.Words.RW.Html.Reader.CommonBorder
{
    /// <summary>
    /// Stores Paragraph and BordersInfo pair.
    /// </summary>
    internal class ParagraphBordersPair
    {
        internal ParagraphBordersPair(Paragraph paragraph, BordersInfo borders)
        {
            Debug.Assert(paragraph != null);
            Debug.Assert(borders != null);

            Paragraph = paragraph;
            Borders = borders;
        }

        internal readonly Paragraph Paragraph;
        internal readonly BordersInfo Borders;
    }
}
