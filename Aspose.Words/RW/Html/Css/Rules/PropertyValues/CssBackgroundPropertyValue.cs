// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2015 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a background shorthand CSS property value.
    /// </summary>
    internal class CssBackgroundPropertyValue : CssPropertyValue
    {
        internal CssBackgroundPropertyValue(CssValue color, CssValue image, CssValue repeat,
                                            CssValue attachment, CssBackgroundPositionPropertyValue position)
            : base(CreateValues(color, image, repeat, attachment, position))
        {
        }

        internal CssBackgroundPropertyValue(CssValueList linearGradientValues)
            : base(new CssFunctionValue("linear-gradient", linearGradientValues))
        {
            Debug.Assert(linearGradientValues != null);
        }

        private static CssValueList CreateValues(CssValue color, CssValue image, CssValue repeat,
            CssValue attachment, CssBackgroundPositionPropertyValue position)
        {
            CssValueList values = new CssValueList();
            if (color != null)
                values.Add(color);
            if (image != null)
                values.Add(image);
            if (repeat != null)
                values.Add(repeat);
            if (attachment != null)
                values.Add(attachment);
            if (position != null)
            {
                values.Add(position.X);
                values.Add(position.Y);
            }

            Debug.Assert(values.Count != 0);
            return values;
        }
    }
}