// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Collections;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.144 pivotFmts (Pivot Formats) element.
    /// This element contains a collection of formatting bands for a surface chart indexed from low to high.
    /// </summary>
    internal class DmlChartPivotFormats
    {
        internal void AddFmt(DmlChartPivotFormat pivotFormat)
        {
            mFmts[pivotFormat.Index] = pivotFormat;
        }

        internal DmlChartPivotFormats Clone()
        {
            DmlChartPivotFormats lhs = new DmlChartPivotFormats();

            foreach (DmlChartPivotFormat pivotFormat in mFmts.Values)
                lhs.AddFmt(pivotFormat.Clone());

            return lhs;
        }

        internal DmlChartPivotFormat GetFmt(int index)
        {
            return mFmts[index];
        }

        internal IntToObjDictionary<DmlChartPivotFormat> Fmts
        {
            get { return mFmts; }
        }

        /// <summary>
        ///  Dictionary contains formatting. Key is index, value is <see cref="DmlChartPivotFormat"/>.
        /// </summary>
        private readonly IntToObjDictionary<DmlChartPivotFormat> mFmts = new IntToObjDictionary<DmlChartPivotFormat>();
    }
}
