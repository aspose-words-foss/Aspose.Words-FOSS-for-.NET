// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2021 by Victor Chebotok

using System;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an individual font source information from a @font-face CSS rule.
    /// </summary>
    /// <remarks>
    /// Note that each @font-face rule may specify more than one font source.
    /// See https://www.w3.org/TR/css-fonts-3/#src-desc
    /// </remarks>
    internal class CssFontSource
    {
        private CssFontSource(bool isLocal, string nameOrUri, CssFontFaceFormat format)
        {
            Debug.Assert(StringUtil.HasChars(nameOrUri));

            IsLocal = isLocal;
            NameOrUri = nameOrUri;
            Format = DetectFormat(format);
        }

        internal static CssFontSource CreateLocal(string familyName)
        {
            return new CssFontSource(true, familyName, CssFontFaceFormat.Unknown);
        }

        internal static CssFontSource CreateExternal(string uri, CssFontFaceFormat format)
        {
            return new CssFontSource(false, uri, format);
        }

        internal bool IsLocal { get; }

        internal CssFontFaceFormat Format { get; }

        internal string NameOrUri { get; }

        internal bool IsSupportedFormat
        {
            get
            {
                switch (Format)
                {
                    case CssFontFaceFormat.Woff:
                    case CssFontFaceFormat.EmbeddedOpentype:
                    case CssFontFaceFormat.TrueType:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool HasExtension(string extension)
        {
            string fileExtension = UriUtil.GetExtension("", NameOrUri);
            return string.Equals(extension, fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        private bool HasMediaType(string mediaType)
        {
            DataUrl dataUrl = DataUrl.Parse(NameOrUri);
            return (dataUrl != null) && IsFontWithType(dataUrl.MediaType, mediaType);
        }

        private static bool IsFontWithType(string mediaType, string fontType)
        {
            return string.Equals(mediaType, "font/" + fontType, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mediaType, "application/x-font-" + fontType, StringComparison.OrdinalIgnoreCase);
        }

        private CssFontFaceFormat DetectFormat(CssFontFaceFormat format)
        {
            if ((format == CssFontFaceFormat.EmbeddedOpentype) || HasExtension("eot") || HasMediaType("eot"))
                return CssFontFaceFormat.EmbeddedOpentype;
            if ((format == CssFontFaceFormat.Woff) || HasExtension("woff") || HasMediaType("woff"))
                return CssFontFaceFormat.Woff;
            if ((format == CssFontFaceFormat.TrueType) || HasExtension("ttf") || HasMediaType("ttf"))
                return CssFontFaceFormat.TrueType;
            return CssFontFaceFormat.Unknown;
        }
    }
}
