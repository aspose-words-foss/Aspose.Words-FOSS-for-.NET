// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'text-align' CSS property.
    /// </summary>
    /// <remarks>CSS text align is applied to a model format with <see cref="CssTextAlignStyleConverter"/></remarks>
    internal class CssTextAlignPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssTextAlignPropertyDef()
            : base(
                "text-align",
                true,
                CssValue.AwStart,
                // This property also accepts custom alignment values ('-aw-start', '-aw-match-parent', '-aw-left', '-aw-right',
                // and '-aw-center'), but they are used internally and are not recognized in document CSS.
                CssValueFilter.Values(
                    CssValue.Left,
                    CssValue.Right,
                    CssValue.Center,
                    CssValue.Justify))
        {
            // Empty constructor.
        }

        protected override CssPropertyValue ToComputedValueCore(CssPropertyValue specifiedValue, CssCascadeContext cssContex)
        {
            CssPropertyValue computedValue = specifiedValue;
            if (specifiedValue.Equals(CssValue.AwMatchParent))
            {
                CssDeclaration parentTextAlignDeclaration = cssContex.ParentElementDeclarations["text-align"];
                if ((parentTextAlignDeclaration == null) || (parentTextAlignDeclaration.Value.Equals(CssValue.AwStart)))
                {
                    CssDirection parentDirection = CssUtil.GetDirection(cssContex.ParentElementDeclarations);
                    computedValue = CssUtil.GetEffectiveAwStartValue(parentDirection);
                }
                else
                {
                    computedValue = parentTextAlignDeclaration.Value;
                }
            }
            return computedValue;
        }
    }
}
