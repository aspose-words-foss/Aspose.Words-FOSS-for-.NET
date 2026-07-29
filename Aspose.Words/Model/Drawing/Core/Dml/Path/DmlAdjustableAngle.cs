// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Guides;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents adjustable angle
    /// </summary>
    /// <remarks>
    /// 5.1.12.1 ST_AdjAngle (Adjustable Angle Methods)
    /// This simple type is an adjustable angle, either an absolute angle or a reference to 
    /// a geometry guide. The units for an adjustable angle are 60,000ths of a degree.
    /// </remarks>
    internal class DmlAdjustableAngle
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value of angle in adjustable angle units.</param>
        internal DmlAdjustableAngle(double value)
        {
            mString = FormatterPal.DoubleToStr9Decimals(value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value can be a number or a guide name.</param>
        internal DmlAdjustableAngle(string value)
        {
            if (!StringUtil.HasChars(value))
                mString = "0";
            else
                mString = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <remarks>
        /// An angle in 60,000ths of a degree. Positive angles are clockwise 
        /// (i.e., towards the positive y axis); negative angles are 
        /// counter-clockwise (i.e., towards the negative y axis).
        /// </remarks>
        internal double GetValue(IDmlGuideValueProvider guideValueProvider)
        {
            return DmlFormulaHelper.GetValue(mString, guideValueProvider);
        }

        internal double GetValueInRadians(IDmlGuideValueProvider guideValueProvider)
        {
            return ConvertUtilCore.DmlAnglesToRadians(GetValue(guideValueProvider));
        }

        /// <summary>
        /// Gets a string representation of the angle
        /// </summary>
        internal string String
        {
            get { return mString; }
        }

        private readonly string mString;
    }
}