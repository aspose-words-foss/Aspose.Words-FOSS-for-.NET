// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2021 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the "src" property of a @font-face rule.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/css-fonts-3/#src-desc
    /// </remarks>
    internal class CssSrcPropertyDef : CssIndividualPropertyDef
    {
        internal CssSrcPropertyDef()
            : base("src", false, null)
        {
            // Empty constructor.
        }

        internal override CssDeclaration CreateIndividualDeclaration(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues)
        {
            if (startIndex == 0)
            {
                CssPropertyValue propertyValue = CreatePropertyValue(cssValues, isInQuirksMode);
                if (propertyValue != null)
                {
                    affectedValues = cssValues.Count;
                    return new CssSpecifiedDeclaration(Name, propertyValue, important);
                }
            }

            affectedValues = 0;
            return null;
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            List<CssFontSource> fontSources = new List<CssFontSource>();
            int first = 0;
            // Note that we step past the end of list in order to simplify the loop body. The last part is also processed
            // inside the loop.
            for (int last = 0; last < (cssValues.Count + 1); last++)
            {
                bool isDelimeter = (last >= cssValues.Count) || cssValues[last].Equals(CssValue.Comma);
                if (isDelimeter)
                {
                    int count = last - first;
                    if (count > 0)
                    {
                        CssFontSource fontSource = ParseFontSource(cssValues.GetRange(first, count));
                        if (fontSource != null)
                        {
                            fontSources.Add(fontSource);
                        }
                    }
                    first = last + 1;
                }
            }

            return (fontSources.Count > 0)
                ? new CssSrcPropertyValue(fontSources)
                : null;
        }

        private static CssFontSource ParseFontSource(CssValueList cssValues)
        {
            if (cssValues.Count == 0)
            {
                return null;
            }

            CssValue value = cssValues[0];

            if (value.ValueType == CssValueType.Function)
            {
                if (cssValues.Count > 1)
                    return null;

                CssFunctionValue cssFunction = (CssFunctionValue)value;

                if ((cssFunction.Name != "local") || (cssFunction.Arguments.Count < 1))
                    return null;

                string fontName = ParseLocalFontName(cssFunction.Arguments);
                return StringUtil.HasChars(fontName)
                    ? CssFontSource.CreateLocal(fontName)
                    : null;
            }

            if (value.ValueType == CssValueType.Uri)
            {
                CssFontFaceFormat format = CssFontFaceFormat.Unknown;
                string fontUri = ((CssUriValue)value).Value;

                if (!StringUtil.HasChars(fontUri))
                    return null;

                if (cssValues.Count == 2)
                {
                    CssFunctionValue cssFunction = cssValues[1] as CssFunctionValue;
                    if ((cssFunction == null) ||
                        (cssFunction.Name != "format") ||
                        (cssFunction.Arguments.Count != 1))
                    {
                        return null;
                    }

                    string formatArgument = cssFunction.Arguments[0].Value as string;
                    if (!StringUtil.HasChars(formatArgument))
                    {
                        return null;
                    }
                    format = ParseFormatArgument(formatArgument);
                }

                return CssFontSource.CreateExternal(fontUri, format);
            }

            return null;
        }

        private static string ParseLocalFontName(CssValueList cssValues)
        {
            CssValueType firstValueType = cssValues[0].ValueType;

            // A single string or identifier.
            if ((cssValues.Count == 1) &&
                ((firstValueType == CssValueType.String) || (firstValueType == CssValueType.Identifier)))
            {
                return (string)cssValues[0].Value;
            }

            // A series of identifiers separated by whitespace. For example, "src: local(Times New Roman)"
            if (firstValueType == CssValueType.Identifier)
            {
                StringBuilder fontName = new StringBuilder();
                foreach (CssValue cssValue in cssValues)
                {
                    if (cssValue.ValueType != CssValueType.Identifier)
                    {
                        return null;
                    }

                    if (fontName.Length > 0)
                    {
                        fontName.Append(' ');
                    }
                    fontName.Append((string)cssValue.Value);
                }
                return fontName.ToString();
            }

            // Nothing of the above. This is an error.
            return null;
        }

        private static CssFontFaceFormat ParseFormatArgument(string value)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "truetype":
                    return CssFontFaceFormat.TrueType;
                case "woff":
                    return CssFontFaceFormat.Woff;
                case "embedded-opentype":
                    return CssFontFaceFormat.EmbeddedOpentype;
                case "woff2":
                    return CssFontFaceFormat.Woff2;
                case "collection":
                    return CssFontFaceFormat.Collection;
                case "svg":
                    return CssFontFaceFormat.Svg;
                case "opentype":
                    return CssFontFaceFormat.OpenType;
                default:
                    return CssFontFaceFormat.Unknown;
            }
        }
    }
}
