// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2022 by Ilya Egorov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Defines a method that compares two <see cref="DmlChartValue"/> objects.
    /// </summary>
    internal class DmlChartValueComparer : IComparer<DmlChartValue>
    {
        public int Compare(DmlChartValue x, DmlChartValue y)
        {
            if (!MathUtil.IsZero(x.Value.CompareTo(y.Value)))
                return x.Value.CompareTo(y.Value);
            else if (x.Index.CompareTo(y.Index) != 0)
                return x.Index.CompareTo(y.Index);
            else
                return 0;
        }
    }
}
