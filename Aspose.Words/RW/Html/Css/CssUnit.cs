// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/09/2006 by Roman Korchagin

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Specifies the type of a CSS dimension property (a number with units).
    /// At the moment fully supports only absolute length units.
    /// Relative units are needed to output relative font size for instance.
    /// And restricted support is added for them.
    /// 
    /// Don't rearrange this as there is a related table with conversion factors.
    /// </summary>
    internal enum CssUnit
    {
        None,
        /// <summary>
        /// Absolute length. Inches.
        /// </summary>
        In,
        /// <summary>
        /// Absolute length. Centimeters.
        /// </summary>
        Cm,
        /// <summary>
        /// Absoute length. Millimeters.
        /// </summary>
        Mm,
        /// <summary>
        /// Absolute length. Points.
        /// </summary>
        Pt,
        /// <summary>
        /// Absolute length. Picas. 1pc = 12pt
        /// </summary>
        Pc,
        /// <summary>
        /// CSS says its relative length, but I treat this as absolute length 96dpi. 
        /// Really it should be recalculated according to screen resolution or any given 
        /// resolution on import. It's another candidate to "LoadOptions".
        /// </summary>
        Px,
        /// <summary>
        /// Relative length based on the current font size.
        /// </summary>
        Em,
        /// <summary>
        /// Relative length based on the so-named 'x-height' of the current font.
        /// </summary>
        Ex,
        /// <summary>
        /// Relative length based on the root element font size. In an HTML document, the root element is the html element.
        /// </summary>
        Rem
    }
}
