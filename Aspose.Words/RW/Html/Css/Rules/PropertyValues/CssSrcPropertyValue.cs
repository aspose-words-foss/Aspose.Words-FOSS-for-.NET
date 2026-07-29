// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2021 by Victor Chebotok

using System.Collections.Generic;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a value of the "src" property in the @font-face rule.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/css-fonts-3/#src-desc
    /// </remarks>
    internal class CssSrcPropertyValue : CssPropertyValue
    {
        internal CssSrcPropertyValue(IList<CssFontSource> fontSources)
            : base(ConvertSourcesToCssValues(fontSources))
        {
            FontSources = fontSources;
        }

        internal IList<CssFontSource> FontSources { get; }

        protected override void ValueToCss(StringBuilder sb)
        {
            const string delimiter = ", ";
            for (int i = 0; i < FontSources.Count; i++)
            {
                if (i != 0)
                    sb.Append(delimiter);
                CssFontSource fontSource = FontSources[i];
                if (fontSource.IsLocal)
                {
                    sb.Append("local('");
                    sb.Append(CssEscape.EscapeSingleQuotedString(fontSource.NameOrUri));
                    sb.Append("')");
                }
                else
                {
                    sb.Append("url('");
                    sb.Append(CssEscape.EscapeSingleQuotedString(fontSource.NameOrUri));
                    sb.Append("')");

                    if (fontSource.IsSupportedFormat)
                    {
                        sb.Append(' ');
                        sb.Append("format(");
                        sb.Append(FormatToString(fontSource.Format));
                        sb.Append(')');
                    }
                }
            }
        }

        private static CssValueList ConvertSourcesToCssValues(IList<CssFontSource> fontSources)
        {
            CssValueList cssValues = new CssValueList();
            foreach (CssFontSource fontSource in fontSources)
            {
                if (fontSource.IsLocal)
                {
                    cssValues.Add(new CssFunctionValue("local", new CssStringValue(fontSource.NameOrUri)));
                }
                else
                {
                    cssValues.Add(new CssUriValue(fontSource.NameOrUri));
                    if (fontSource.IsSupportedFormat)
                    {
                        cssValues.Add(new CssFunctionValue("format",
                            new CssStringValue(FormatToString(fontSource.Format))));
                    }
                }
            }
            return cssValues;
        }

        private static string FormatToString(CssFontFaceFormat format)
        {
            switch (format)
            {
                case CssFontFaceFormat.Collection:
                    return "collection";
                case CssFontFaceFormat.EmbeddedOpentype:
                    return "embedded-opentype";
                case CssFontFaceFormat.OpenType:
                    return "opentype";
                case CssFontFaceFormat.Svg:
                    return "svg";
                case CssFontFaceFormat.TrueType:
                    return "truetype";
                case CssFontFaceFormat.Woff:
                    return "woff";
                case CssFontFaceFormat.Woff2:
                    return "woff2";
                default:
                    return string.Empty;
            }
        }
    }
}
