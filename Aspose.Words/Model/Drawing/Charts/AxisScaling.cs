// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents the scaling options of the axis.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class AxisScaling : IDmlExtensionListSource
    {
        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        internal AxisScaling Clone()
        {
            AxisScaling lhs = (AxisScaling)MemberwiseClone();
            lhs.mGapWidth = mGapWidth.Clone();
            lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);
            return lhs;
        }

        /// <summary>
        /// Sets the <see cref="LogBase"/> property without checking input value.
        /// <see cref="Type"/> of the scale is changed to <see cref="AxisScaleType.Logarithmic"/>.
        /// </summary>
        internal void SetLogBaseWithoutCheck(double value)
        {
            mLogBase = value;
            mType = AxisScaleType.Logarithmic;
        }

        /// <summary>
        /// Gets or sets scaling type of the axis.
        /// </summary>
        /// <remarks>
        /// The <see cref="AxisScaleType.Linear"/> value is the only that is allowed in MS Office 2016 new charts.
        /// </remarks>
        public AxisScaleType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Gets or sets the logarithmic base for a logarithmic axis.
        /// </summary>
        /// <remarks>
        /// <para>The property is not supported by MS Office 2016 new charts.</para>
        /// <para>Valid range of a floating point value is greater than or equal to 2 and less than or 
        /// equal to 1000. The property has effect only if <see cref="Type"/> is set to 
        /// <see cref="AxisScaleType.Logarithmic"/>.</para>
        /// <para>Setting this property sets the <see cref="Type"/> property to <see cref="AxisScaleType.Logarithmic"/>.
        /// </para>
        /// </remarks>
        public double LogBase
        {
            get { return mLogBase; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 2.0, 1000.0, "value");
                mLogBase = value;
                mType = AxisScaleType.Logarithmic;
            }
        }

        /// <summary>
        /// Gets or sets minimum value of the axis.
        /// </summary>
        /// <remarks>
        /// The default value is "auto".
        /// </remarks>
        public AxisBound Minimum
        {
            get { return mMinimum != null ? mMinimum : AxisBound.Auto; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mMinimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the axis.
        /// </summary>
        /// <remarks>
        /// The default value is "auto".
        /// </remarks>
        public AxisBound Maximum
        {
            get { return mMaximum != null ? mMaximum : AxisBound.Auto; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mMaximum = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether minimum value of the axis is defined explicitly.
        /// </summary>
        internal bool MinimumIsDefined
        {
            get { return mMinimum != null; }
        }

        /// <summary>
        /// Gets a flag indicating whether maximum value of the axis is defined explicitly.
        /// </summary>
        internal bool MaximumIsDefined
        {
            get { return mMaximum != null; }
        }

        /// <summary>
        /// Gets or sets the space between data points as a ratio of gap width over category width.
        /// Valid range of a value is greater than or equal to zero.
        /// </summary>
        /// <dev>
        /// Related to category axis only.
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal DoubleOrAutomatic GapWidth
        {
            get { return mGapWidth; }
            set { mGapWidth = value; }
        }

        internal AxisOrientation Orientation
        {
            get { return mOrientation; }
            set { mOrientation = value; }
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList ([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined. 
        ///  </summary>
        /// <remarks>
        /// Explicit implementation hides this from public.
        /// </remarks>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        private AxisScaleType mType = AxisScaleType.Linear;
        private double mLogBase = 10;
        private AxisBound mMinimum;
        private AxisBound mMaximum;
        private DoubleOrAutomatic mGapWidth = DoubleOrAutomatic.AsNull(0);
        private AxisOrientation mOrientation = AxisOrientation.MinMax;
        private StringToObjDictionary<DmlExtension> mExtensions;
    }
}
