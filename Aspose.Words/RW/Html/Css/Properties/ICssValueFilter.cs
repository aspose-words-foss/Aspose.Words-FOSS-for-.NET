// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2021 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS value filter.
    /// </summary>
    internal interface ICssValueFilter
    {
        /// <summary>
        /// Indicates whether the filter accepts (passes) the specified CSS value in the specified document mode.
        /// </summary>
        bool Accepts(CssValue value, bool isInQuirksMode);
    }
}
