// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Interface for HTML element.
    /// </summary>
    internal interface IHtmlElementProvider : IElementProvider
    {
        /// <summary>
        /// Returns a value indicating whether this element contains only one child element optionally surrounded by whitespace.
        /// </summary>
        bool HasOnlyOneChildElementWithOptionalWhitespace();

        /// <summary>
        /// Indicates whether the first inner box of this element is an implicit box.
        /// </summary>
        bool StartsWithImplicitBox { get; }

        /// <summary>
        /// Indicates whether the last inner box of this element is an implicit box.
        /// </summary>
        bool EndsWithImplicitBox { get; }

        /// <summary>
        /// Indicates whether this element contains non-implicit child boxes.
        /// </summary>
        bool ContainsChildBoxes { get; }

        /// <summary>
        /// Indicates whether this element or any of its child elements contains text that is not whitespace
        /// and is not ignored by the CSS box model.
        /// </summary>
        bool IsElementContainsText { get; }

        /// <summary>
        /// Indicates whether this element is the last non-implicit child box of its parent element.
        /// </summary>
        bool IsLastChildBox { get; }
    }
}
