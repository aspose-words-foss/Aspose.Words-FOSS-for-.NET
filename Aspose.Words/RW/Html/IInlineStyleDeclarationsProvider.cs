// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2015 by Alexey Butalov

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Interface to get inline style of an HTML element as an instance of <see cref="CssDeclarationCollection"/>.
    /// </summary>
    /// <remarks>
    /// There are two parts - Export and Import in HTML Model. 
    /// HTML elements have no parsed inline styles in HTML Import and this should be parsed during CSS resolving.
    /// But the inline styles are already presented as <see cref="CssDeclarationCollection"/> in HTML Export and 
    /// should not be parsed. This interface is used in <see cref="CssResolver"/> class and prevents parsed 
    /// inline declarations during HTML export. This improves the performance of HTML Export.
    /// </remarks>
    internal interface IInlineStyleDeclarationsProvider
    {
        /// <summary>
        /// Gets inline style of an HTML element as an instance of <see cref="CssDeclarationCollection"/>.
        /// </summary>
        /// <returns>
        /// Inline CSS declarations of an HTML element. The result is never <c>null</c>. If the element has got no inline CSS,
        /// an empty collection is returned.
        /// </returns>
        CssDeclarationCollection InlineStyle { get; }
    }
}
