// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/06/2016 by Victor Chebotok

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Caches results of CSS selector to document style resolution.
    /// </summary>
    /// <remarks>
    /// This class is used to speed up selector to style resolution.
    /// </remarks>
    internal class HtmlSelectorStyleCache
    {
        /// <summary>
        /// Gets a document style that corresponds to the specified selector and style.
        /// </summary>
        /// <param name="selector">A CSS selector.</param>
        /// <param name="styleType">A requested style type.</param>
        /// <returns>
        /// A document style that corresponds to the selector. Can be <c>null</c> if the selector hasn't been resolved
        /// to a style yet. Can be the null substitute style if there is no document style that corresponds to the selector.
        /// </returns>
        internal Style GetStyle(CssSelector selector, StyleType styleType)
        {
            Debug.Assert(selector != null);

            HtmlSelectorStyleCacheKey key = new HtmlSelectorStyleCacheKey(selector, styleType);
            return mCachedStyles.GetValueOrNull(key);
        }

        /// <summary>
        /// Adds a resolution result (a selector and a style) to the cache.
        /// </summary>
        /// <param name="selector">A CSS selector.</param>
        /// <param name="styleType">A requested style type.</param>
        /// <param name="style">A document style that corresponds to the selector. Can be <c>null</c>.</param>
        internal void AddStyle(CssSelector selector, StyleType styleType, Style style)
        {
            Debug.Assert(selector != null);
            Debug.Assert(!IsNullSubstitute(style));

            HtmlSelectorStyleCacheKey key = new HtmlSelectorStyleCacheKey(selector, styleType);
            if (style == null)
            {
                style = gNullStyle;
            }
            mCachedStyles.Add(key, style);
        }

        /// <summary>
        /// Checks whether the specified style is a special instance that substitutes for <c>null</c> values in this cache.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Non-static to provide a consistent class interface where all methods are called in the same way.")]
        internal bool IsNullSubstitute(Style style)
        {
            return style == gNullStyle;
        }

        /// <summary>
        /// A special style instance that substitutes for <c>null</c> styles in this cache.
        /// </summary>
        /// <remarks>
        /// This instance is used to tell apart the situations "a selector hasn't been resolved to a style yet" and "a selector
        /// has been resolved to no style".
        /// </remarks>
        private static readonly Style gNullStyle = Style.Create(StyleType.Character);

        private readonly Dictionary<HtmlSelectorStyleCacheKey, Style> mCachedStyles =
            new Dictionary<HtmlSelectorStyleCacheKey, Style>();
    }
}
