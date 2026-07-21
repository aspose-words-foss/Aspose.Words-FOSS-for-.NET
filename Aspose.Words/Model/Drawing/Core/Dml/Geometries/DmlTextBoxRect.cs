// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/28/2014 by Alexey Noskov

using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Nrx;

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class DmlTextBoxRect
    {
        internal DmlTextBoxRect(string l, string t, string r, string b)
        {
            mLeft = l;
            mTop = t;
            mRight = r;
            mBottom = b;
        }

        /// <summary>
        /// Clones this instance of text box rectangle.
        /// </summary>
        internal DmlTextBoxRect Clone()
        {
            return (DmlTextBoxRect)MemberwiseClone();
        }

        /// <summary>
        /// Gets the value. 
        /// </summary>
        /// <param name="value">The input value that may be a number, a universal measure or a guide name.</param>
        /// <param name="guides">Dictionary of guides.</param>
        /// <returns>Numeric value in EMUs.</returns>
        private static float GetValue(string value, DmlGuides guides)
        {
            double result = NrxXmlReader.TryConvertUniversalMeasureToEmus(value, null);
            if (!double.IsNaN(result))
                return (float)result;

            return (float)guides.GetValue(value);
        }

        internal string Left
        {
            get { return mLeft; }
        }

        internal string Top
        {
            get { return mTop; }
        }

        internal string Right
        {
            get { return mRight; }
        }

        internal string Bottom
        {
            get { return mBottom; }
        }

        /// <summary>
        /// Returns textbox rectangle defined by the specified guides.
        /// Guides must be calculated in this stage, otherwise empty rectangle will be returned.
        /// Since guides are not calculated on reading stage, we cannot do this in constructor and have to keep rect as strings.
        /// </summary>
        internal RectangleF GetRectangle(DmlGuides guides, bool isUpArrow)
        {
            try
            {
                float l = GetValue(mLeft, guides);
                float t = GetValue(mTop, guides);
                float r = GetValue(mRight, guides);
                float b = GetValue(mBottom, guides);
                RectangleF result = RectangleF.FromLTRB(l, t, r, b);
                if (isUpArrow)
                {
                    // WORDSNET-15365 "upArrow" shape is an exception, height specifies Y position instead of height.
                    // And height should be calculated as difference between real shape height and Y position.
                    float height = GetValue("h", guides);
                    result = new RectangleF(result.X, result.Height, result.Width, height - result.Height);
                }

                return result;
            }
            catch
            {
                return RectangleF.Empty;
            }

        }

        private readonly string mLeft;
        private readonly string mTop;
        private readonly string mRight;
        private readonly string mBottom;
    }
}
