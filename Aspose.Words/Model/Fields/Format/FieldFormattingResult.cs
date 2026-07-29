// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class FieldFormattingResult
    {
        internal FieldFormattingResult(RichString text, bool preserveRichFormatting)
        {
            Text = text;
            PreserveRichFormatting = preserveRichFormatting;
        }

        internal FieldFormattingResult(string text)
            : this(RichString.CreateFromString(text), false)
        {
        }

        internal RichString Text { get; }
        internal bool PreserveRichFormatting { get; }
    }
}
