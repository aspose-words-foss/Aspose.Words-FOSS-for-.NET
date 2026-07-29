// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.145 pivotSource (Pivot Source) element.
    /// This element specifies the source pivot table for a pivot chart.
    /// </summary>
    internal class DmlChartPivotSource : DmlExtensionListSource
    {
        internal DmlChartPivotSource Clone()
        {
            DmlChartPivotSource lhs = (DmlChartPivotSource)MemberwiseClone();
            lhs.Extensions = CloneExtensions();
            return lhs;
        }

        internal int FmtId
        {
            get { return mFmtId; }
            set { mFmtId = value; }
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        private int mFmtId;
        private string mName;
    }
}
