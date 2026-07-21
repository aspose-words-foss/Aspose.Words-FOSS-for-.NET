// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Collapsing of two adjacent CSS box margins performed by AW itself (not by MS Word).
    /// </summary>
    /// <remarks>
    /// In certain situations (for example, around floating tables) MS Word's margin collapsing rules differ from CSS rules,
    /// and AW has to manually collapse margins (recalculate their values) to make sure the result looks as specified by CSS. 
    /// </remarks>
    internal enum CssBoxMarginManualCollapsing
    {
        /// <summary>
        /// Margins do not collapse (are separate from each other).
        /// </summary>
        Separate,
        /// <summary>
        /// Margins do collapse. The resulting margin is equal to the maximum of the two margins.
        /// </summary>
        Collapse,
        /// <summary>
        /// Margins do collapse. The resulting margin is zero.
        /// </summary>
        Zero
    }
}
