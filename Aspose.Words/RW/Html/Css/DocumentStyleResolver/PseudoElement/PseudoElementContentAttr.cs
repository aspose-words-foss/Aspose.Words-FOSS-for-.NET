// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the 'attr' CSS function, which is a souce of generated content for pseudo-elements.
    /// </summary>
    internal class PseudoElementContentAttr : PseudoElementContentPart
    {
        internal PseudoElementContentAttr(string attributeName)
        {
            Debug.Assert(StringUtil.HasChars(attributeName));
            mAttributeName = attributeName;
        }

        /// <summary>
        /// Parses parameters of the 'attr' CSS function.
        /// </summary>
        /// <returns>
        /// Parsed content or <c>null</c> if parameters are malformed.
        /// </returns>
        internal static PseudoElementContentAttr Parse(CssFunctionValue value)
        {
            if ((value.Name != "attr") || (value.Arguments.Count != 1))
            {
                // Error. Either function name or function syntax is invalid.
                return null;
            }

            CssValue firstParameter = value.Arguments[0];
            if (firstParameter.ValueType != CssValueType.Identifier)
            {
                // Error. Attribute name must be an identifier.
                return null;
            }

            string attributeName = ((CssIdentifierValue)firstParameter).Value;
            Debug.Assert(StringUtil.HasChars(attributeName));

            // In HTML, attribute names are compared in a case-insensitive manner. Note, however, that the specifications
            // doesn't define an algorithm for comparison of attribute names. Seems that both Crome and Firefox convert 
            // parameters of attr() functions to lowercase upon reading, and we stick to this behavior. However, this may lead
            // to unexpected results, because the HTML converts case of arguments in a different manner. It converts all ASCII
            // letters to lowercase and leave all other characters intact. For example, attribute name 'DÂTÄ' will be converted
            // to 'dÂtÄ' in HTML and it will be converted to 'dâtä' in CSS. As a result, attr(DÂTÄ) will not match the attribute
            // DÂTÄ="..." both in Chrome and Firefox but it will match the attribute in IE, which uses different casing rules
            // for attributes in CSS.
            attributeName = attributeName.ToLowerInvariant();

            return new PseudoElementContentAttr(attributeName);
        }

        internal override void Accept(IPseudoElementContentPartVisitor visitor)
        {
            visitor.VisitAttr(mAttributeName);
        }

#if DEBUG
        public override string ToString()
        {
            return "attr(" + mAttributeName + ")";
        }
#endif

        private readonly string mAttributeName;
    }
}
