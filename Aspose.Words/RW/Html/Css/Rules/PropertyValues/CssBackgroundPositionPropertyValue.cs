// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS background position property value.
    /// </summary>
    internal class CssBackgroundPositionPropertyValue : CssPropertyValue
    {
        /// <summary>
        /// Ctor. Creates a CSS background position property value.
        /// </summary>
        internal CssBackgroundPositionPropertyValue(CssValue x, CssValue y)
            : base(new CssValueList(x, y))
        {
            Debug.Assert(x != null);
            Debug.Assert(y != null);
            mX = x;
            mY = y;
        }

        internal CssValue X
        {
            get { return mX; }
        }

        internal CssValue Y
        {
            get { return mY; }
        }

        private readonly CssValue mX;
        private readonly CssValue mY;
    }
}
