// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Resolves CSS declaration interdependency and converts certain vendor-specific declarations into normal declarations.
    /// This is expected to be the final step in CSS declaration resolution.
    /// </summary>
    internal class CssInterdependencyResolver
    {
        internal CssInterdependencyResolver(CssUserAgentFormatting omittedUserAgentFormatting)
        {
            mOmittedUserAgentFormatting = omittedUserAgentFormatting;
        }

        /// <summary>
        /// Performs in-place resolution of HTML element's interdependent CSS declarations.
        /// </summary>
        internal void Resolve(CssDeclarationCollectionBuilder declarations, IElementProvider element)
        {
            // Here the directionality of the element has already been resolved, so custom direction-specific properties
            // can be replaced with their counterparts.
            ReplaceStartCustomProperty(declarations, "-aw-margin-start", "margin");
            ReplaceStartCustomProperty(declarations, "-aw-padding-start", "padding");

            // Values of 'align' attributes of outer elements should not affect table contents, but values of 'text-align'
            // properties of outer elements should. Here the table's 'text-align' value is resetted to the initial value
            // if its current value comes from an 'align' attribute.
            if (element.ElementName == "table")
            {
                CssDeclaration textAlign = declarations["text-align"];
                if (textAlign != null)
                {
                    if (textAlign.Value.Equals(CssValue.AwLeft) ||
                        textAlign.Value.Equals(CssValue.AwCenter) ||
                        textAlign.Value.Equals(CssValue.AwRight))
                    {
                        declarations.Remove(textAlign.Property);
                        declarations.Add(new CssSpecifiedDeclaration("text-align", CssValue.AwStart));
                        declarations.MarkUserAgent("text-align");
                    }
                }

                CssDeclaration borderCollapse = declarations["border-collapse"];
                if ((borderCollapse != null) && borderCollapse.Value.Equals(CssValue.Collapse))
                {
                    // WORDSNET-9462 According to w3w we should ignore border-spacing if value of 'border-collapse' equals to 'collapse'.
                    declarations.AddOrReplace(new CssSpecifiedDeclaration("border-spacing", CssValue.Zero));
                    declarations.MarkUserAgent("border-spacing");
                }
            }

            // This rule is a part of the user agent style sheet, but it depends on CSS property values of the element's parent,
            // so we have to apply it here.
            if (element.ElementName == "th")
            {
                CssDeclaration textAlingDeclaration = declarations["text-align"];
                if ((textAlingDeclaration == null) || (textAlingDeclaration.Value.Equals(CssValue.AwStart)))
                {
                    declarations.AddOrReplace(new CssSpecifiedDeclaration("text-align", CssValue.Center));
                    declarations.MarkUserAgent("text-align");
                }
            }

            // The following rules affect the font size. They are not applied if it is overridden by the font size specified 
            // in the target document.
            bool applyDefaultFontFormatting = (mOmittedUserAgentFormatting & CssUserAgentFormatting.Font) == 0;
            if (applyDefaultFontFormatting)
            {
                // WORDSNET-9334 In modern browsers, if an element has the "font-family:monospace" declaration, its font size
                // is changed to 10pt (as a result, "font-family:monospace" means "font-family:'Courier New';font-size:10pt").
                // However, the font size is not changed when the 'font-family' declaration contains a list of font families,
                // even if 'monospace' is on that list.
                if (declarations["font-size"] == null)
                {
                    CssDeclaration fontFamilyDeclaration = declarations["font-family"];
                    if ((fontFamilyDeclaration != null) && fontFamilyDeclaration.Value.Equals(CssValue.Monospace))
                    {
                        declarations.Add(new CssSpecifiedDeclaration("font-size", new CssLengthValue(10, CssUnit.Pt)));
                        declarations.MarkUserAgent("font-size");
                    }
                }
            }
        }

        /// <summary>
        /// Replaces custom direction-specific properties with their counterparts depending on the element directionality.
        /// </summary>
        /// <param name="declarations">CSS declarations of an HTML element.</param>
        /// <param name="customPropertyName">The name of the custom direction-specific property to be replaced.</param>
        /// <param name="commonPropertyPrefix">The first part of the counterpart property name. An appropriate suffix
        /// will be appended to this parameter in order to form the full property name.</param>
        private static void ReplaceStartCustomProperty(
            CssDeclarationCollectionBuilder declarations,
            string customPropertyName,
            string commonPropertyPrefix)
        {
            CssDeclaration customPropertyDeclaration = declarations[customPropertyName];
            if (customPropertyDeclaration == null)
            {
                return;
            }

            // Determine element's direction and writing mode.
            CssDeclaration directionDeclaration = declarations["direction"];
            bool isRtl = (directionDeclaration != null) && directionDeclaration.Value.Equals(CssValue.Rtl);

            CssDeclaration writingModeDeclaration = declarations["writing-mode"];
            bool isVertical = (writingModeDeclaration != null) &&
                (writingModeDeclaration.Value.Equals(CssValue.VerticalLr) || writingModeDeclaration.Value.Equals(CssValue.VerticalRl));

            // Determine what 'start' in the custom property name means based on element's direction and writing mode.
            string suffix;
            if (isRtl)
            {
                suffix = (isVertical)
                    ? "bottom"
                    : "right";
            }
            else
            {
                suffix = (isVertical)
                    ? "top"
                    : "left"; 
            }
            string counterpartPropertyName = commonPropertyPrefix + "-" + suffix;

            // Write a common counterpart to the replaced custom property.
            // The custom properties has lower priority than their counterparts, so the replacement is only performed
            // if no counterpart property is applied to the element.
            CssDeclaration counterpartDeclaration = declarations[counterpartPropertyName];
            if (counterpartDeclaration == null)
            {
                declarations.Add(new CssSpecifiedDeclaration(counterpartPropertyName, customPropertyDeclaration.Value));
                declarations.SetFlags(counterpartPropertyName, declarations.GetFlags(customPropertyName));
            }

            // Remove the custom property.
            declarations.Remove(customPropertyDeclaration.Property);
        }

        /// <summary>
        /// Groups of user agent (default) CSS styles that are omitted from style resolution.
        /// </summary>
        private readonly CssUserAgentFormatting mOmittedUserAgentFormatting;
    }
}
