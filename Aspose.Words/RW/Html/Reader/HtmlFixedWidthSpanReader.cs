// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/01/2024 by Victor Chebotok

using System.Collections.Generic;
using Aspose.Drawing.Fonts;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Handles fixed-width spans and generates padding for them.
    /// </summary>
    internal class HtmlFixedWidthSpanReader
    {
        internal HtmlFixedWidthSpanReader(
            LoadFormat loadFormat,
            DocumentFormatter formatter,
            HtmlBidiTextArranger bidiTextArranger,
            IDrFontProvider fontProvider)
        {
            // For now we preserve span's width only for MHTML files.
            mIsEnabled = loadFormat == LoadFormat.Mhtml;
            mFormatter = formatter;
            mBidiTextArranger = bidiTextArranger;
            mFontProvider = fontProvider;
        }

        internal void HandleSpanStart(HtmlElementNode node)
        {
            if (!IsSupportedFixedWidthSpan())
            {
                return;
            }

            // WORDSNET-17414 IE opens MHTML documents in IE 5 compatibility mode.
            // It looks like in this mode IE processes fixed-width bidi spans separately from surrounding text.
            mBidiTextArranger.RearrangeAndWriteText();

            // Don't write sequence of space characters that pad fixed-width spans if text is not aligned with the start of
            // the span. We only support the case where text is followed by space.
            bool textIsAlignedWithSpanStart = CssTextAlignStyleConverter.TextIsAlignedWithStart(
                mFormatter.ElementDeclarations,
                mFormatter.IsBlockRtl());
            if (!textIsAlignedWithSpanStart)
            {
                return;
            }

            double width = mFormatter.ElementDeclarations.GetLength("width");
            if (width <= 0)
            {
                return;
            }

            // We start to process a new fixed-width span.
            mOpenedSpans.Push(new FixedWidthSpan(node, width));
        }

        internal string HandleSpanEnd(HtmlElementNode node, Font font)
        {
            if (!IsSupportedFixedWidthSpan())
            {
                return string.Empty;
            }

            // WORDSNET-17414 IE opens MHTML documents in IE 5 compatibility mode.
            // It looks like in this mode IE processes fixed-width bidi spans separately from surrounding text.
            mBidiTextArranger.RearrangeAndWriteText();

            // We don't need to do anything if we're not currently processing any fixed-width span or if the specified node
            // is not the fixed-width span that we're currently processing.
            FixedWidthSpan span = mOpenedSpans.Top();
            if ((span == null) || (span.SpanElement != node))
            {
                return string.Empty;
            }

            // Forget the current span. We have finished processing it.
            mOpenedSpans.Pop();

            // Use space characters to pad the trailing empty space (if any) of the span.
            // WORDSNET-27107 If the font size is explicitly set to zero, the calculation result will not be correct.
            if ((span.TrailingEmptySpaceWidth > 0) && !MathUtil.IsZero(font.Size))
            {
                double spaceCharWidth = GetTextWidth(ControlChar.Space, font);
                int paddingCharCount = MathUtil.DoubleToInt(span.TrailingEmptySpaceWidth / spaceCharWidth);
                return new string(ControlChar.SpaceChar, paddingCharCount);
            }

            return string.Empty;
        }

        internal void ProcessText(string text, Font font)
        {
            if (mOpenedSpans.Count > 0)
            {
                float textWidth = GetTextWidth(text, font);
                // Add text's width to all fixed-width spans that enclose this text.
                foreach (FixedWidthSpan span in mOpenedSpans)
                {
                    span.AddTextWidth(textWidth);
                }
            }
        }

        private bool IsSupportedFixedWidthSpan()
        {
            if (!mIsEnabled)
            {
                return false;
            }

            // Normally, "width" has no effect on inline-level elements. However, it can be applied by Internet Explorer when
            // a document is opened in the IE5 compatibility mode, which is used for MHTML documents.
            CssDisplayType displayType = mFormatter.ElementDisplayType;
            if ((displayType != CssDisplayType.Inline) && (displayType != CssDisplayType.InlineBlock))
            {
                return false;
            }

            return mFormatter.ElementDeclarations["width"] != null;
        }

        private float GetTextWidth(string text, Font font)
        {
            DrFont drawingFont = mFontProvider.FetchDrFont(font.Name, (float)font.Size, font.FontStyle);
            return drawingFont.GetTextWidthPoints(text);
        }

        private class FixedWidthSpan
        {
            internal FixedWidthSpan(HtmlElementNode span, double width)
            {
                SpanElement = span;
                TrailingEmptySpaceWidth = width;
            }

            internal void AddTextWidth(double width)
            {
                TrailingEmptySpaceWidth -= width;
            }

            internal HtmlElementNode SpanElement { get; }

            internal double TrailingEmptySpaceWidth { get; private set; }
        }

        private readonly bool mIsEnabled;
        private readonly Stack<FixedWidthSpan> mOpenedSpans = new Stack<FixedWidthSpan>();
        private readonly DocumentFormatter mFormatter;
        private readonly HtmlBidiTextArranger mBidiTextArranger;
        private readonly IDrFontProvider mFontProvider;
    }
}
