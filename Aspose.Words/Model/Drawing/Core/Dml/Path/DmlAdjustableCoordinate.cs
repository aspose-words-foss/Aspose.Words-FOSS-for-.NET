// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Nrx;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents adjustable coordinate
    /// </summary>
    /// <remarks>
    /// 5.1.12.2 ST_AdjCoordinate (Adjustable Coordinate Methods)
    /// This simple type is an adjustable coordinate is either an 
    /// absolute coordinate position or a reference to a geometry guide.
    /// </remarks>
    internal class DmlAdjustableCoordinate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value in path coordinate system.</param>
        internal DmlAdjustableCoordinate(double value)
        {
            mString = FormatterPal.DoubleToStr9Decimals(value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value can be a number or a guide name.</param>
        internal DmlAdjustableCoordinate(string value)
        {
            if (!StringUtil.HasChars(value))
                mString = "0";
            else
                mString = value;
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlAdjustableCoordinate"/>.
        /// </summary>
        internal DmlAdjustableCoordinate Clone()
        {
            return (DmlAdjustableCoordinate)MemberwiseClone();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        internal double GetValue(IDmlGuideValueProvider guideValueProvider)
        {
            double result = FormatterPal.TryParseDoubleInvariant(mString);
            if (!double.IsNaN(result))
                return result;

            result = NrxXmlReader.TryConvertUniversalMeasureToEmus(mString, null);
            if (!double.IsNaN(result))
                return result;

            return DmlFormulaHelper.GetValue(mString, guideValueProvider);
        }

        /// <summary>
        /// Gets a string representation of the coordinate.
        /// </summary>
        internal string String
        {
            get { return mString; }
        }

        private readonly string mString;
    }
}