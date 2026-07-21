// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2016 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents 'list-style' shorthand CSS property value.
    /// </summary>
    internal class CssListStylePropertyValue: CssPropertyValue
    {
        internal CssListStylePropertyValue(CssValue image, CssValue type, CssValue position) 
            : base(CreateValues(image, type, position))
        {
        }

        private static CssValueList CreateValues(CssValue image, CssValue type, CssValue position)
        {
            CssValueList values = new CssValueList();
            if (image != null)
                values.Add(image);
            if (type != null)
                values.Add(type);
            if (position != null)
                values.Add(position);

            Debug.Assert(values.Count != 0);
            return values;
        }
    }
}