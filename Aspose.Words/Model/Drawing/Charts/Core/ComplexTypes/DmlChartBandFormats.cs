// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Noskov


using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.14 bandFmts (Band Formats) element.
    /// This element contains a collection of formatting bands for a surface chart indexed from low to high.
    /// </summary>
    internal class DmlChartBandFormats
    {
        internal DmlChartBandFormats Clone()
        {
            DmlChartBandFormats lhs = new DmlChartBandFormats();

            IntToObjDictionary<DmlChartBandFormat>.Enumerator enumerator = mFmts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DmlChartBandFormat format = enumerator.CurrentValue;
                lhs.mFmts[enumerator.CurrentKey] = format.Clone();
            }

            return lhs;
        }

        internal void AddFmt(int index, DmlChartSpPr spPr)
        {
            mFmts[index] = new DmlChartBandFormat(index, spPr);
        }

        internal DmlChartSpPr GetFmt(int index)
        {
            DmlChartBandFormat pr = mFmts[index];
            return (pr != null) ? pr.SpPr : new DmlChartSpPr();
        }

        internal IEnumerable<DmlChartBandFormat> Formats
        {
            get { return mFmts.Values; }
        }

        internal int Count
        {
            get { return mFmts.Count; }
        }

        /// <summary>
        /// Dictionary contains formatting. Key is index, value is SpPr.
        /// </summary>
        private readonly IntToObjDictionary<DmlChartBandFormat> mFmts = new IntToObjDictionary<DmlChartBandFormat>();
    }
}
