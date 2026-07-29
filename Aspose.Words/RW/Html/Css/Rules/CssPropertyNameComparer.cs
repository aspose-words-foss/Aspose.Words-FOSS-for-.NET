// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/09/2012 by Alexey Butalov

using System;
using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to sort CSS declarations in some preferable order.
    /// </summary>
    internal class CssPropertyNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int xOrderIndex = gPropertyOrderList.GetValueOrDefault(x, int.MaxValue);
            int yOrderIndex = gPropertyOrderList.GetValueOrDefault(y, int.MaxValue);

            if ((xOrderIndex != int.MaxValue) || (yOrderIndex != int.MaxValue))
                return xOrderIndex.CompareTo(yOrderIndex);

            bool xStartsWithAWPrefix = x.StartsWith(HtmlConstants.AsposeVendorCssPrefix, StringComparison.Ordinal);
            bool yStartsWithAWPrefix = y.StartsWith(HtmlConstants.AsposeVendorCssPrefix, StringComparison.Ordinal);

            if (xStartsWithAWPrefix == yStartsWithAWPrefix)
            {
#pragma warning disable CA1309 // Use ordinal string comparison
                // We use the InvariantCulture comparison here for historical reasons. This comparison differs from the ordinal
                // comparison in the way it handles hyphen ('-') characters. For example, the ordinal comparison places
                // the string "-webkit-box" before the string "border", while with the invariant culture comparison the order
                // of the strings is reverse: "border" then "-webkit-box". We want to preserve the order of properties,
                // because a lot of gold file will change otherwise.
                return string.Compare(x, y, StringComparison.InvariantCulture);
#pragma warning restore CA1309 // Use ordinal string comparison
            }

            // Our -aw-* CSS properties should be last.
            return xStartsWithAWPrefix ? -1 : 1;
        }

        private static readonly SortedList<string, int> gPropertyOrderList;

        /// <summary>
        /// Here we keep some pleasant order to output CSS properties.
        /// If we miss anything here all remaining attributes will be in the end of declaration.
        /// </summary>
        private static readonly string[] gCssPropertyOrder =
            {
                "size", // for @page rules
                "width",
                "height",

                "margin",
                "margin-top",
                "margin-right",
                "margin-left",
                "margin-bottom",

                "text-indent",
                "text-align",
                "page-break-before",
                "page-break-inside",
                "page-break-after",
                "line-height",
                "widows",
                "orphans",
                "writing-mode",

                "border",
                "border-style",
                "border-width",
                "border-color",
                "border-top",
                "border-top-style",
                "border-top-width",
                "border-top-color",
                "border-right",
                "border-right-style",
                "border-right-width",
                "border-right-color",
                "border-left",
                "border-left-style",
                "border-left-width",
                "border-left-color",
                "border-bottom",
                "border-bottom-style",
                "border-bottom-width",
                "border-bottom-color",

                "padding",
                "padding-top",
                "padding-right",
                "padding-left",
                "padding-bottom",

                "font",
                "font-family",
                "font-size",
                "font-weight",
                "font-style",
                "font-variant",
                "text-decoration",
                "text-transform",
                "letter-spacing",
                "vertical-align",
                "color",
                "background",
                "background-color",
                "display",

                "list-style",
                "list-style-type",
                "list-style-image",
                "list-style-position",

                "src" // for @font-face rules
            };

        static CssPropertyNameComparer()
        {
            gPropertyOrderList = new SortedList<string, int>();
            int orderIndex = 0;
            foreach (string item in gCssPropertyOrder)
                gPropertyOrderList.Add(item, ++orderIndex);
        }
    }
}
