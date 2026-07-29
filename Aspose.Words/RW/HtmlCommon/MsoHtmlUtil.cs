// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.HtmlCommon
{
    internal static class MsoHtmlUtil
    {
        internal static bool IsElementSupportedByMsWord(string tag)
        {
            return ArrayUtil.BinarySearch(gElementsSupportedByMsWord, tag) >= 0;
        }

        /// <summary>
        /// List of element names supported by MS Word.
        /// </summary>
        /// <remarks>
        /// The list is sorted for binary search to work.
        /// </remarks>
        private static readonly string[] gElementsSupportedByMsWord = new string[]
        {
            "a", "acronym", "address", "applet", "area", "b", "base", "basefont", "bdo", "big", "blockquote", "body", "br",
            "caption", "cite", "code", "dd", "del", "dfn", "dir", "div", "dl", "dt", "em", "font", "form", "frame", "frameset",
            "h1", "h2", "h3", "h4", "h5", "h6", "head", "hr", "html", "i", "iframe", "img", "input", "ins", "isindex", "kbd",
            "label", "legend", "li", "link", "listing", "map", "marquee", "menu", "meta", "nobr", "noframes", "object", "ol",
            "option", "p", "param", "pre", "q", "rb", "rp", "rt", "ruby", "s", "samp", "script", "select", "small", "span",
            "strike", "strong", "style", "sub", "sup", "table", "tbody", "td", "textarea", "tfoot", "th", "thead", "title",
            "tr", "tt", "u", "ul", "var", "wbr", "xml", "xmp"
        };
    }
}
