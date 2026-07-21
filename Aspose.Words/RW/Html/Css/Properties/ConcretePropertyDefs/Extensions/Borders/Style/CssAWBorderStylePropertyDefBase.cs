// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2020 by Artem Shabarshin

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for '-aw-border-top-style', '-aw-border-right-style', '-aw-border-bottom-style', 
    /// '-aw-border-left-style' CSS extended border properties.
    /// </summary>
    /// <remarks>
    /// CSS border extension style is applied to a model format with <see cref="CssBorderStyleConverter"/>
    /// </remarks>
    internal abstract class CssAWBorderStylePropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssAWBorderStylePropertyDefBase(string name)
            : base(
                name,
                false,
                CssValue.None,
                // nil |
                // single | thick | double | hairline | dot | dash-large-gap |
                // dot-dash | dot-dot-dash | triple | thin-thick-small-gap | thick-thin-small-gap |
                // thin-thick-thin-small-gap | thin-thick-medium-gap | thick-thin-medium-gap |
                // thin-thick-thin-medium-gap | thin-thick-large-gap | thick-thin-large-gap |
                // thin-thick-thin-large-gap | wave | double-wave | dash-small-gap | dash-dot-stroker |
                // emboss3d | engrave3d | outset
                CssValueFilter.Values(
                    CssValue.Nil,
                    CssValue.Single,
                    CssValue.Thick,
                    CssValue.DoubleId,
                    CssValue.Hairline,
                    CssValue.Dot,
                    CssValue.DashLargeGap,
                    CssValue.DotDash,
                    CssValue.DotDotDash,
                    CssValue.Triple,
                    CssValue.ThinThickSmallGap,
                    CssValue.ThickThinSmallGap,
                    CssValue.ThinThickThinSmallGap,
                    CssValue.ThinThickMediumGap,
                    CssValue.ThickThinMediumGap,
                    CssValue.ThinThickThinMediumGap,
                    CssValue.ThinThickLargeGap,
                    CssValue.ThickThinLargeGap,
                    CssValue.ThinThickThinLargeGap,
                    CssValue.Wave,
                    CssValue.DoubleWave,
                    CssValue.DashSmallGap,
                    CssValue.DashDotStroker,
                    CssValue.Emboss3D,
                    CssValue.Engrave3D,
                    CssValue.Outset,
                    CssValue.Inset))
        {
            // Empty constructor. Everything is set up by the base class.
        }
    }
}
