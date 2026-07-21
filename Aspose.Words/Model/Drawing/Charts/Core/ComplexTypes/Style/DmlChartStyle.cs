// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.1.1 chartStyle, 2.8.3.1 CT_ChartStyle [MS-ODRAWXML]
    /// A complex type that specifies visual and text properties for all elements present on a chart.
    /// </summary>
    internal class DmlChartStyle : DmlExtensionListSource
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal DmlChartStyle()
        {
            mStyleEntries = new IntToObjDictionary<DmlChartStyleEntry>();
        }

        /// <summary>
        /// Clones this <see cref="DmlChartStyle"/> object.
        /// </summary>
        internal DmlChartStyle Clone()
        {
            DmlChartStyle lhs = (DmlChartStyle)MemberwiseClone();

            lhs.mStyleEntries = new IntToObjDictionary<DmlChartStyleEntry>();
            foreach (DmlChartStyleItem index in AllStyleItems)
            {
                if (mStyleEntries[(int)index] != null)
                    lhs[index] = this[index].Clone();
            }

            if (mDataPointMarkerLayout != null)
                lhs.mDataPointMarkerLayout = mDataPointMarkerLayout.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified item is a required element of chart style.
        /// </summary>
        private static bool IsRequiredStyleItem(DmlChartStyleItem item)
        {
            return (item != DmlChartStyleItem.DataLabelCallout);
        }

        /// <summary>
        /// Returns a style entry item for the specified index.
        /// </summary>
        internal DmlChartStyleEntry this[DmlChartStyleItem index]
        {
            get
            {
                DmlChartStyleEntry result = mStyleEntries[(int)index];
                if ((result == null) && IsRequiredStyleItem(index))
                {
                    result = new DmlChartStyleEntry();
                    mStyleEntries[(int)index] = result;
                }
                return result;
            }
            set { mStyleEntries[(int)index] = value; }
        }

        /// <summary>
        /// Represents dataPointMarkerLayout: a CT_MarkerLayout element that specifies additional marker properties
        /// not present in dataPointMarker. It is not required element of the CT_ChartStyle type.
        /// </summary>
        internal DmlChartMarkerLayout DataPointMarkerLayout
        {
            get { return mDataPointMarkerLayout; }
            set { mDataPointMarkerLayout = value; }
        }

        /// <summary>
        /// Represents id: an unsignedInt ([XMLSCHEMA2] section 3.3.22) attribute that specifies the identifier
        /// for this CT_ChartStyle.
        /// </summary>
        internal string Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Gets the identifier of the style.
        /// </summary>
        internal int IntId
        {
            get { return FormatterPal.TryParseInt(mId); }
        }

        /// <summary>
        /// Returns all elements of the <see cref="DmlChartStyleItem"/> enum to be used in the foreach operator.
        /// </summary>
        internal static DmlChartStyleItem[] AllStyleItems
        {
            get
            {
                return (DmlChartStyleItem[])Enum.GetValues(typeof(DmlChartStyleItem));
            }
        }

        private IntToObjDictionary<DmlChartStyleEntry> mStyleEntries;
        private DmlChartMarkerLayout mDataPointMarkerLayout;
        private string mId;
    }
}
