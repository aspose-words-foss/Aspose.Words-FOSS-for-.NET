// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2021 by Victor Chebotok

using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Fonts.TrueType;
using Aspose.Fonts.Woff;
using Aspose.Words.Fonts;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Imports fonts from @font-face rule to document.
    /// </summary>
    internal class CssFontFaceProvider
    {
        internal CssFontFaceProvider(
            IList<CssFontFaceRule> fontFaceRules,
            string baseUri,
            HtmlResourceLoader resourceLoader,
            Document document)
        {
            mFontFaceRules = fontFaceRules;
            mBaseUri = baseUri;
            mResourceLoader = resourceLoader;
            mDocument = document;
        }

        internal bool ImportFontFaceToDocument(string fontFamily)
        {
            if (mProcessedFontFamilies.Contains(fontFamily))
            {
                return true;
            }

            foreach (CssFontFaceRule fontFaceRule in mFontFaceRules)
            {
                if (!fontFaceRule.DeclaresFontFamily(fontFamily))
                {
                    continue;
                }

                IList<CssFontSource> sources = fontFaceRule.GetSources();
                if (sources == null)
                {
                    continue;
                }

                foreach (CssFontSource source in sources)
                {
                    byte[] fontData = null;

                    if (source.IsLocal)
                    {
                        TTFont font = mDocument.FontProvider.GetTTFont(source.NameOrUri, fontFaceRule.GetFontStyle());
                        if (font != null)
                        {
                            if (StringUtil.EqualsIgnoreCase(fontFamily, source.NameOrUri))
                            {
                                return true;
                            }
                            fontData = font.Data.GetFontBytes();
                        }
                    }
                    else
                    {
                        if (!source.IsSupportedFormat)
                        {
                            continue;
                        }

                        fontData = mResourceLoader.LoadFont(mBaseUri, source.NameOrUri);
                        if ((source.Format == CssFontFaceFormat.Woff) && (fontData != null))
                        {
                            fontData = WoffConverter.Convert(new MemoryStream(fontData));
                        }
                    }

                    if (fontData != null)
                    {
                        EmbedFont(
                            fontFamily,
                            source.Format == CssFontFaceFormat.EmbeddedOpentype,
                            fontData,
                            fontFaceRule);
                        mProcessedFontFamilies.Add(fontFamily);
                        return true;
                    }
                }
            }

            return false;
        }

        private void EmbedFont(
            string fontFamily,
            bool isEot,
            byte[] fontData,
            CssFontFaceRule fontFaceRule)
        {
            FontInfo fontInfo = new FontInfo(fontFamily);

            EmbeddedFontFormat embeddedFontFormat = isEot
                ? EmbeddedFontFormat.EmbeddedOpenType
                : EmbeddedFontFormat.OpenType;

            fontInfo.AddEmbeddedFont(
                fontData,
                embeddedFontFormat,
                fontFaceRule.GetEmbeddedFontStyle(),
                false);
            TTFont font = fontInfo.GetEmbeddedFontParsedAnyStyle();
            if ((font != null) && font.IsPrintPreview)
            {
                Warn(WarningType.FontEmbedding,
                    string.Format(@"The loaded font has license restrictions on embedding. '{0}' only has `Print & Preview` permissions.", fontFamily));
            }

            mDocument.FontInfos.Merge(fontInfo);
        }

        private void Warn(WarningType warningType, string description)
        {
            if (mDocument.WarningCallback != null)
                mDocument.WarningCallback.Warning(new WarningInfo(warningType, WarningSource.Html, description));
        }

        private readonly IList<CssFontFaceRule> mFontFaceRules;
        private readonly string mBaseUri;
        private readonly HtmlResourceLoader mResourceLoader;
        private readonly Document mDocument;
        private readonly CaseInsensitiveHashSet mProcessedFontFamilies = new CaseInsensitiveHashSet();
    }
}
