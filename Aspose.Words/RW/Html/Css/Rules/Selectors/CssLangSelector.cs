// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :lang() pseudo-class selector. For example, ':lang("en")'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#lang-pseudo
    /// </remarks>
    internal class CssLangSelector : CssPseudoClassSelector
    {
        internal CssLangSelector(string language)
        {
            Debug.Assert(StringUtil.HasChars(language));
            mLanguage = language;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            string elementLanguage = element.GetLanguage();
            Debug.Assert(elementLanguage != null);

            if (StringUtil.EqualsIgnoreCase(elementLanguage, mLanguage))
            {
                return true;
            }

            string languagePrefix = mLanguage + '-';
            if (elementLanguage.Length < languagePrefix.Length)
            {
                return false;
            }
            string elementLanguagePrefix = elementLanguage.Substring(0, languagePrefix.Length);

            return StringUtil.EqualsIgnoreCase(elementLanguagePrefix, languagePrefix);
        }

        internal override string ToCss()
        {
            return ":lang(" + CssEscape.EscapeIdentifier(mLanguage) + ")";
        }

        protected override string MakePreferableStyleName()
        {
            return "lang(" + mLanguage + ")";
        }

        private readonly string mLanguage;
    }
}
