// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2025 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents base class for individual 'mso-para-margin-xxx' CSS properties.
    /// </summary>
    internal abstract class CssMsoParaMarginIndividualPropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssMsoParaMarginIndividualPropertyDefBase(string name)
            : base(
                name,
                true,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.Auto),
                    CssValueFilter.Percentage,
                    CssValueFilter.QuirkyLength))
        {
            // Empty constructor.
        }
    }
}
