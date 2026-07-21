// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/06/2021 by Artem Tsetkhalin

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// Converts locale IDs to and from language tags compliant with BCP 47.
    /// </summary>
    /// <remarks>
    /// See https://www.rfc-editor.org/info/bcp47 and WORDSNET-21963
    /// </remarks>
    internal static class HtmlLocaleConverter
    {
        /// <summary>
        /// Converts a locale ID to a language tag.
        /// </summary>
        /// <remarks>
        /// If this method cannot convert the locale ID to a language tag, it returns an empty string.
        /// </remarks>
        internal static string LocaleToTag(int localeId)
        {
            string tag = AdditionalLocaleToTag(localeId);
            if (StringUtil.HasChars(tag))
            {
                return tag;
            }

            tag = LocaleConverter.LocaleToDocxTag(localeId);
            return tag;
        }

        /// <summary>
        /// Converts a language tag to a locale ID.
        /// </summary>
        /// <returns>
        /// If the language tag is unknown, this method returns <see cref="Language.InvariantCulture"/>.
        /// </returns>
        internal static int TagToLocale(string tag)
        {
            // First, check if this is an additional language tag that we support.
            int localeId = AdditionalTagToLocale(tag);
            if (localeId != (int)Language.InvariantCulture)
            {
                return localeId;
            }

            // Try to parse the value as a DOCX language tag.
            localeId = LocaleConverter.DocxTagToLocale(tag);

            // If the value is not a valid DOCX language tag, try parse it as a WML tag. Note that MS Word writes WML language
            // tags to HTML documents it generates and this part allows us to correctly load language info from such documents.
            if (localeId == (int)Language.InvariantCulture)
            {
                localeId = LocaleConverter.WmlTagToLocale(tag.ToUpperInvariant());
            }

            return localeId;
        }

        private static string AdditionalLocaleToTag(int localeId)
        {
            // We normally use DOCX language tags, because in most cases they confirm with BCP 47. However, there are some
            // non-compliant tags in the DOCX set that we have to correct manually.
            switch ((Language)localeId)
            {
                case Language.SpanishSpainTraditionalSort:
                    // We cannot preserve the sorting method in this case.
                    return "es-ES";
                case Language.FrenchWestIndies:
                    return "fr-029";
                default:
                    return string.Empty;
            }
        }

        private static int AdditionalTagToLocale(string tag)
        {
            // This method parses language tags that we support in addition to DOCX and WML language tags.
            Language language;
            // Language tags are case-insensitive, see RFC 5646 section "2.1.1. Formatting of Language Tags".
            switch (tag.ToLowerInvariant())
            {
                case "fr-029":
                    language = Language.FrenchWestIndies;
                    break;
                default:
                    language = Language.InvariantCulture;
                    break;
            }
            return (int)language;
        }
    }
}
