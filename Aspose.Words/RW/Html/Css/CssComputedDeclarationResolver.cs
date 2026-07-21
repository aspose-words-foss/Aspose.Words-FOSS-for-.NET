// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/08/2015 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Resolves specified CSS declarations for computing. E.g. 'em' and 'ex' units are computed to absolute lengths.
    /// </summary>
    internal class CssComputedDeclarationResolver
    {
        internal CssComputedDeclarationResolver(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        /// <summary>
        /// Resolves specified CSS declarations for computing. E.g. 'em' and 'ex' units are computed to absolute lengths.
        /// </summary>
        /// <param name="declarations">Specified CSS declarations for computing.</param>
        /// <returns>Returns computed CSS declarations collection.</returns>
        internal CssDeclarationCollection ResolveToComputed(CssDeclarationCollection declarations)
        {
            return ResolveToComputed(declarations, null, null, false);
        }

        /// <summary>
        /// Resolves specified CSS declarations for computing. E.g. 'em' and 'ex' units are computed to absolute lengths.
        /// </summary>
        /// <param name="declarations">Specified CSS declarations for computing.</param>
        /// <param name="parentDeclarations">
        /// Computed declarations of a parent HTML element; can be null, if the element doesn't have a parent.
        /// </param>
        /// <param name="rootDeclarations">
        /// Computed declarations of a root HTML element; can be null, if the element doesn't have a root.
        /// </param>
        /// <param name="isPseudoElement">
        /// Indicates whether the current HTML element is a pseudo-element.
        /// </param>
        /// <returns>Returns computed CSS declarations collection.</returns>
        internal CssDeclarationCollection ResolveToComputed(
            CssDeclarationCollection declarations,
            CssDeclarationCollection parentDeclarations,
            CssDeclarationCollection rootDeclarations,
            bool isPseudoElement)
        {
            CssDeclarationCollection computedDeclarations = ComputeDeclarations(declarations, parentDeclarations,
                rootDeclarations, isPseudoElement);
            return ResolveInterdependency(computedDeclarations);
        }

        /// <summary>
        /// Resolves specified CSS declarations to computed declarations.
        /// </summary>
        private CssDeclarationCollection ComputeDeclarations(
            CssDeclarationCollection declarations,
            CssDeclarationCollection parentDeclarations,
            CssDeclarationCollection rootDeclarations,
            bool isPseudoElement)
        {
            // Skip empty collections since Java throws on them.
            if (declarations.Count == 0)
                return CssDeclarationCollection.Empty;

            // SPEED. It is actually faster to check whether any declarations should be computed and return fast in case no
            // changes are needed.
            List<CssSpecifiedDeclaration> declarationsToCompute = new List<CssSpecifiedDeclaration>();
            List<CssDeclaration> declarationsToRemove = new List<CssDeclaration>();
            foreach (CssDeclaration declaration in declarations)
            {
                CssSpecifiedDeclaration specifiedDeclaration = declaration as CssSpecifiedDeclaration;
                if (specifiedDeclaration == null)
                {
                    // Declaration has been computed already.
                    continue;
                }

                if (specifiedDeclaration.Value.IsInherit)
                {
                    // If a declaration has the 'inherit' value here, it computes to a default value and thus must be skipped,
                    // because all non-default 'inherit' values have already been replaced in the inheritance resolver.
                    declarationsToRemove.Add(declaration);
                    continue;
                }

                declarationsToCompute.Add(specifiedDeclaration);
            }
            if ((declarationsToCompute.Count == 0) && (declarationsToRemove.Count == 0))
            {
                return declarations;
            }

            // Compute this element's font size first, because relative values of some properties (for instance, 'line-heigth')
            // refer to the element's font size.
            double fontSize = CssUtil.DefaultFontSize;

            // According to the CSS specification, the font size should be taken from the HTML element. MS Word, however,
            // always uses a fixed font size value during import altChunk.
            if (!mApplyFormattingAsMsWord)
            {
                CssDeclaration fontSizeDeclaration = declarations["font-size"];
                if (fontSizeDeclaration != null)
                {
                    CssSpecifiedDeclaration specifiedElementFontSize = fontSizeDeclaration as CssSpecifiedDeclaration;

                    CssDeclaration computedElementFontSize;
                    if (specifiedElementFontSize != null)
                    {
                        CssCascadeContext cssContextWithoutFontSize = new CssCascadeContext(
                            rootDeclarations,
                            parentDeclarations,
                            0,
                            isPseudoElement);
                        computedElementFontSize = ComputeDeclaration(specifiedElementFontSize, cssContextWithoutFontSize);
                    }
                    else
                    {
                        computedElementFontSize = fontSizeDeclaration;
                    }

                    fontSize = CssUtil.LengthToPoint(computedElementFontSize.Value.FirstValue);
                    if (MathUtil.IsMinValue(fontSize))
                    {
                        fontSize = CssUtil.DefaultFontSize;
                    }
                }
            }

            CssCascadeContext cssContext = new CssCascadeContext(rootDeclarations, parentDeclarations, fontSize,
                isPseudoElement);

            CssDeclarationCollectionBuilder computedDeclarationBuilder = new CssDeclarationCollectionBuilder(declarations);
            foreach (CssDeclaration declaration in declarationsToRemove)
            {
                computedDeclarationBuilder.Remove(declaration.Property);
            }
            foreach (CssSpecifiedDeclaration specifiedDeclaration in declarationsToCompute)
            {
                computedDeclarationBuilder.Replace(ComputeDeclaration(specifiedDeclaration, cssContext));
            }

            return computedDeclarationBuilder.GetDeclarations();
        }

        /// <summary>
        /// Resolves specified value of the declaration to computed value. E.g. 'em' and 'ex' units are computed to absolute lengths.
        /// </summary>
        /// <remarks>
        /// The computed value is the result of resolving the specified value, generally absolutizing it in preparation for inheritance.
        /// A specified value can be either absolute (i.e., not relative to another value, as in red or 2mm) or relative (i.e., relative to another value, 
        /// as in auto, 2em). Computing a relative value generally absolutizes it:
        ///   - values with relative units (em, ex, vh, vw) must be made absolute by multiplying with the appropriate reference size
        ///   - certain keywords (e.g., smaller, bolder) must be replaced according to their definitions
        ///   - percentages on some properties must be multiplied by a reference value (defined by the property)
        ///   - valid relative URLs must be resolved to become absolute.
        /// </remarks>
        /// <param name="declaration">Specified CSS declarations for computing.</param>
        /// <param name="cssContext">CSS context.</param>
        private static CssComputedDeclaration ComputeDeclaration(CssSpecifiedDeclaration declaration, CssCascadeContext cssContext)
        {
            CssPropertyDef propertyDef = CssPropertyDefFactory.GetPropertyDef(declaration.Property);
            CssPropertyValue computedValue = propertyDef.ToComputedValue(declaration.Value, cssContext);
            return new CssComputedDeclaration(propertyDef, computedValue, declaration.Important, declaration.Value);
        }

        /// <summary>
        /// Resolves certain interdependent CSS declarations to computed declarations.
        /// </summary>
        private CssDeclarationCollection ResolveInterdependency(CssDeclarationCollection declarations)
        {
            // WORDSNET-23496 In common case MS Word doesn't use the 'color' CSS property to
            // specify a border property. We mimic MS Word behavior during import altChunk.
            if (mApplyFormattingAsMsWord)
                return declarations;

            // WORDSNET-14473 If an element's border color is not specified with a border property, 
            // user agents must use the value of the element's 'color' property as the computed value for the border color.
            CssDeclaration color = declarations["color"];
            bool hasAllBordersColor =
                (declarations["border-top-color"] != null) &&
                (declarations["border-right-color"] != null) &&
                (declarations["border-bottom-color"] != null) &&
                (declarations["border-left-color"] != null);

            // The fast path: no changes needed.
            if ((color == null) || hasAllBordersColor)
            {
                return declarations;
            }

            CssDeclarationCollectionBuilder resolvedDeclarationsBuilder = new CssDeclarationCollectionBuilder(declarations);
            CopyColorToBorderColor(resolvedDeclarationsBuilder, color.Value, "border-top-color");
            CopyColorToBorderColor(resolvedDeclarationsBuilder, color.Value, "border-right-color");
            CopyColorToBorderColor(resolvedDeclarationsBuilder, color.Value, "border-bottom-color");
            CopyColorToBorderColor(resolvedDeclarationsBuilder, color.Value, "border-left-color");
            return resolvedDeclarationsBuilder.GetDeclarations();
        }

        /// <summary>
        /// Copies a color value to one of 'border-xxx-color' properties if the border color is missing from the declarations.
        /// </summary>
        private static void CopyColorToBorderColor(
            CssDeclarationCollectionBuilder declarations,
            CssPropertyValue colorValue,
            string borderColorPropertyName)
        {
            if (declarations[borderColorPropertyName] != null)
            {
                return;
            }

            CssPropertyDef borderColorPropertyDef = CssPropertyDefFactory.GetPropertyDef(borderColorPropertyName);
            declarations.Add(new CssComputedDeclaration(borderColorPropertyDef, colorValue, false, colorValue));
            // The declaration we have created is considered a part of the user-agent CSS.
            declarations.MarkUserAgent(borderColorPropertyName);
        }

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;
    }
}
