// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'stroke-miterlimit' CSS property for SVG.
    /// </summary>
    internal class CssStrokeMiterlimitPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokeMiterlimitPropertyDef()
            : base(
                "stroke-miterlimit",
                true,
                new CssNumberValue(4),
                new StrokeMiterlimitValueFilter())
        {
            // Nothing to do.
        }

        private class StrokeMiterlimitValueFilter : ICssValueFilter
        {
            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                // <miterlimit>
                return (value.ValueType == CssValueType.Number) &&
                   (value.DoubleValue >= 1);
            }
        }
    }
}
