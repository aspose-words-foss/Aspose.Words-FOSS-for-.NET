// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2006 by Roman Korchagin

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Compares shapes by their z-order. Note, that only top level shapes have ZOrder.
    /// </summary>
    internal class ShapeZOrderSorter<T> : IComparer<T>
        where T : IShape
    {
        [JavaGenericParameter("T extends IShape")]
        internal static void Sort(List<T> zOrderList)
        {
            List<IShape> originalOrderClone = new List<IShape>();
            foreach (T shape in zOrderList)
                originalOrderClone.Add(shape);
            zOrderList.Sort(new ShapeZOrderSorter<T>(originalOrderClone));
        }

        private ShapeZOrderSorter(List<IShape> originalOrder)
        {
            mOriginalOrder = originalOrder;
        }

        [JavaConvertCheckedExceptions]
        public int Compare(T x, T y)
        {
            // WORDSCPP-2248 C++ error (cannot access ZOrder_IShape property)
#if CPLUSPLUS
            IShape xShape = (IShape)x;
            IShape yShape = (IShape)y;

            int zorder1 = xShape.ZOrder_IShape;
            int zorder2 = yShape.ZOrder_IShape;
#else
            int zorder1 = x.ZOrder_IShape;
            int zorder2 = y.ZOrder_IShape;
            #endif

            int result = zorder1.CompareTo(zorder2);

            // RK In .NET 1.1 (and also in Java) the Array.Sort algorithm places objects with equal keys differently.
            // This is not good for gold files.

            // In case of equal z-order shapes should be sorted by their appearance.
            // AM. I think best way is compare shape indexes in the original array because
            // they are added in order by appearance.
            if (result == 0)
            {
                int index1 = mOriginalOrder.IndexOf(x);
                int index2 = mOriginalOrder.IndexOf(y);

                result = index1.CompareTo(index2);
            }

            return result;
        }

        private readonly List<IShape> mOriginalOrder;

    }
}
