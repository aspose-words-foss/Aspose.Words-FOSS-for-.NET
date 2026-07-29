// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS text-align style to a model.
    /// </summary>
    internal static class CssTextAlignStyleConverter
    {
        /// <summary>
        /// Applies CSS text-align style to a model paragraph format.
        /// </summary>
        internal static void ToParagraphFormat(CssDeclarationCollection declarations, bool isBlockRtl, ParagraphFormat paragraphFormat)
        {
            CssDeclaration textAlign = declarations["text-align"];
            if (textAlign == null)
            {
                return;
            }

            if (textAlign.Value.Equals(CssValue.AwStart))
            {
                // Keep the Word's default paragraph alignment.
            }
            else if (textAlign.Value.Equals(CssValue.Left) || textAlign.Value.Equals(CssValue.AwLeft))
            {
                paragraphFormat.Alignment = (isBlockRtl)
                                                ? ParagraphAlignment.Right
                                                : ParagraphAlignment.Left;
            }
            else if (textAlign.Value.Equals(CssValue.Right) || textAlign.Value.Equals(CssValue.AwRight))
            {
                paragraphFormat.Alignment = (isBlockRtl)
                                                ? ParagraphAlignment.Left
                                                : ParagraphAlignment.Right;
            }
            else if (textAlign.Value.Equals(CssValue.Center) || textAlign.Value.Equals(CssValue.AwCenter))
            {
                paragraphFormat.Alignment = ParagraphAlignment.Center;
            }
            else if (textAlign.Value.Equals(CssValue.Justify))
            {
                paragraphFormat.Alignment = ParagraphAlignment.Justify;
            }
        }

        internal static bool TextIsAlignedWithStart(CssDeclarationCollection declarations, bool isBlockRtl)
        {
            CssDeclaration textAlign = declarations["text-align"];
            if (textAlign == null)
            {
                return true;
            }

            return textAlign.Value.Equals(CssValue.AwStart) ||
                (!isBlockRtl && (textAlign.Value.Equals(CssValue.Left) || textAlign.Value.Equals(CssValue.AwLeft))) ||
                (isBlockRtl && (textAlign.Value.Equals(CssValue.Right) || textAlign.Value.Equals(CssValue.AwRight)));
        }
    }
}
