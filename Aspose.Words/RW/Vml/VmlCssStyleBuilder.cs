// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2017 by Nikolay Sezganov

using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Builds CSS properties for VML as single string.
    /// </summary>
    internal class VmlCssStyleBuilder
    {
        internal VmlCssStyleBuilder()
        {
            mSortedList = new SortedList<string, string>(new CssPropertyNameComparer());
        }

        /// <summary>
        /// Sets property with number value.
        /// </summary>
        internal void Add(string propertyName, int value)
        {
            Add(propertyName, FormatterPal.IntToStr(value));
        }

        /// <summary>
        /// Sets property with raw value.
        /// </summary>
        internal void Add(string propertyName, string value)
        {
            if (!StringUtil.HasChars(value))
                return;
            mSortedList.Add(propertyName, value);
        }

        /// <summary>
        /// Sets 'font-family' CSS property.
        /// </summary>
        internal void AddFontFamily(string fontName)
        {
            string value = fontName;
            if (!CssEscape.IsValidIdentifier(fontName))
                value = "'" + fontName + "'";

            Add("font-family", value);
        }

        internal string ToCss()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in mSortedList)
            {
                if (sb.Length > 0)
                {
                    sb.Append(';');
                }
                sb.Append(entry.Key);
                sb.Append(':');
                sb.Append(entry.Value);
            }
            return sb.ToString();
        }

        private readonly SortedList<string, string> mSortedList;
    }
}
