// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2024 by Vyacheslav Deryushev

using System.Collections.Generic;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents a read-only collection of <see cref="Adjustment"/> adjust values that are applied to the specified shape.
    /// </summary>
    public class AdjustmentCollection
    {
        /// <summary>
        /// Creates instance of <see cref="AdjustmentCollection"/> class.
        /// </summary>
        internal AdjustmentCollection()
        {
        }

        /// <summary>
        /// Adds an <see cref="Adjustment"/> to the end of the list.
        /// </summary>
        internal void Add(Adjustment adjustment)
        {
            mAdjustments.Add(adjustment);
        }

        /// <summary>
        /// Returns an adjustment at the specified index.
        /// </summary>
        /// <param name="index">An index into the collection.</param>
        public Adjustment this[int index]
        {
            get { return mAdjustments[index]; }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mAdjustments.Count; }
        }

        private readonly List<Adjustment> mAdjustments = new List<Adjustment>();
    }
}
