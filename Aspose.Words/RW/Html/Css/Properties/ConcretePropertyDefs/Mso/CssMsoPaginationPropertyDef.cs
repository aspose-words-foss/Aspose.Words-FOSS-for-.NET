// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoPaginationPropertyDef : CssIndividualPropertyDef
    {
        internal CssMsoPaginationPropertyDef()
            : base("mso-pagination", false, null)
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
            Debug.Assert(false, "This property cannot be a shorthand property part.");
            affectedValues = 0;
            return null;
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            foreach (CssValue value in cssValues)
            {
                if (!IsMsoPaginationValue(value))
                {
                    return null;
                }
            }
            return new CssTextDecorationPropertyValue(cssValues);
        }

        private static bool IsMsoPaginationValue(CssValue value)
        {
            return value.EqualsAny(CssValue.None, CssValue.WidowOrphan, CssValue.LinesTogether, CssValue.NoLineNumbers);
        }
    }
}
