// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/02/2013 by Alexey Butalov

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Gets and formats appropriate model styles when CSS formatting is applied to the document model.
    /// </summary>
    internal abstract class StyleFormatter
    {
        protected StyleFormatter(
            Document document,
            CssStyleTracker cssStyleTracker,
            ParagraphFormatter paragraphFormatter,
            FontFormatter fontFormatter)
        {
            Debug.Assert(document != null);
            Debug.Assert(cssStyleTracker != null);
            Debug.Assert(paragraphFormatter != null);
            Debug.Assert(fontFormatter != null);

            Document = document;
            CssStyleTracker = cssStyleTracker;
            ParagraphFormatter = paragraphFormatter;
            FontFormatter = fontFormatter;
        }

        [JavaThrows(true)]
        internal abstract Style GetModelStyle(StyleType styleType);

        [JavaThrows(true)]
        internal abstract void UpdatePredefinedStyles();

        internal virtual void PostUpdateStyles()
        {
            // Do nothing by default.
        }

        internal void HandleSpanStart()
        {
            Style style = GetModelStyle(StyleType.Character);
            if (style == null)
            {
                style = GetParentCharacterStyle();
            }
            mCharacterStyles.Push(style);
        }

        internal void HandleSpanEnd()
        {
            Debug.Assert(mCharacterStyles.Count > 0);
            mCharacterStyles.Pop();
        }

        [JavaThrows(true)]
        internal virtual void HandleParagraphStart()
        {
            // Do nothing.
        }

        internal abstract Style StyleWithoutDeclarations { get; }

        /// <summary>
        /// Gets the character style from the parent element. May be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// In MS Word, character styles are propagated from parent inline elements to their children.
        /// </remarks>
        protected Style GetParentCharacterStyle()
        {
            return mCharacterStyles.Top();
        }

        protected readonly Document Document;
        protected readonly CssStyleTracker CssStyleTracker;
        protected readonly ParagraphFormatter ParagraphFormatter;
        protected readonly FontFormatter FontFormatter;

        /// <summary>
        /// A stack of character styles selected for parent inline-level HTML elements.
        /// Elements are instances of <see cref="Style"/>.
        /// </summary>
        private readonly Stack<Style> mCharacterStyles = new Stack<Style>();
    }
}
