// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using Aspose.Words.RW.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.5 CT_MarkerLayout [MS-ODRAWXML]
    /// The complex type specifies additional properties for data points that have markers.
    /// </summary>
    internal class DmlChartMarkerLayout
    {
        /// <summary>
        /// Clones this <see cref="DmlChartMarkerLayout"/> object.
        /// </summary>
        internal DmlChartMarkerLayout Clone()
        {
            return (DmlChartMarkerLayout)MemberwiseClone();
        }

        /// <summary>
        /// Represents symbol: an ST_MarkerStyle attribute that specifies a style for markers on a chart.
        /// </summary>
        /// <dev>
        /// The string type is used now. 2.8.4.4 ST_MarkerStyle [MS-ODRAWXML] has all values of
        /// Aspose.Words.Drawing.Charts.MarkerSymbol (21.2.3.27 ST_MarkerStyle [ISO/IEC29500-1:2012]) except
        /// the values 'none' and 'picture'.
        /// </dev>
        internal string Symbol
        {
            get { return mSymbol; }
            set { mSymbol = value; }
        }

        /// <summary>
        /// Represents size: an ST_MarkerSize attribute that specifies the size for markers on a chart.
        /// The simple type specifies that its contents contain an integer between 2 and 72, inclusive,
        /// whose contents are a size in points.
        /// </summary>
        internal int Size
        {
            get { return mSize; }
            set { mSize = value; }
        }

        /// <summary>
        /// Gets the marker symbol as a <see cref="MarkerSymbol"/> value.
        /// </summary>
        internal MarkerSymbol MarkerSymbol
        {
            get { return DmlChartsEnum.DmlToMarkerStyle(Symbol); }
        }

        private string mSymbol;
        private int mSize;
    }
}
