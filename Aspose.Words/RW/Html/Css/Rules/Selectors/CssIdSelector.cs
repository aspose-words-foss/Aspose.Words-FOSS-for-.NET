// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS ID selector. For example, '#id'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#id-selectors
    /// </remarks>
    internal class CssIdSelector : CssSimpleSelector
    {
        internal CssIdSelector(string id)
        {
            Debug.Assert(StringUtil.HasChars(id));
            mId = id;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            string idAttributeValue = element.GetAttributeValue("id");
            if (idAttributeValue == null)
            {
                return false;
            }

            if (documentMode == CssDocumentMode.Quirks)
            {
                return StringUtil.EqualsIgnoreCase(mId, idAttributeValue);
            }
            else
            {
                // Case-sensitive ordinal comparison.
                return mId == idAttributeValue;
            }
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            return "#" + CssEscape.EscapeIdentifier(mId);
        }

        internal string Id { get { return mId; } }

        protected override string MakePreferableStyleName()
        {
            return mId;
        }

        private readonly string mId;

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(1, 0, 0);
    }
}
