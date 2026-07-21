// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'content' CSS property.
    /// See http://www.w3.org/TR/CSS21/generate.html#propdef-content.
    /// </summary>
    internal class CssContentPropertyDef : CssIndividualPropertyDef
    {
        internal CssContentPropertyDef()
            : base("content", false, new CssPropertyValue(new CssValueList(CssValue.Normal)))
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
            affectedValues = 0;
            Debug.Assert(false, "This property isn't part of a shorthand property.");
            return null;
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            return new CssPropertyValue(cssValues);
        }

        protected override CssPropertyValue ToComputedValueCore(CssPropertyValue specifiedValue, CssCascadeContext cssContex)
        {
            if (cssContex.IsPseudoElement)
            {
                if ((specifiedValue.Count == 1) && specifiedValue[0].Equals(CssValue.Normal))
                {
                    return new CssPropertyValue(new CssValueList(CssValue.None));
                }

                // Here we should also replace 'attr()' functions with their values but we leave them as is. This could affect
                // inheritance but in current version of HTML and CSS it is impossible for pseudo-elements to have children so 
                // there are no negative effects. 
                return specifiedValue;
            }
            return new CssPropertyValue(new CssValueList(CssValue.Normal));
        }
    }
}
