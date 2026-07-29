// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2021 by Ilya Navrotskiy

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Contains a collection of <see cref="GradientStop"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-graphic-elements/">Working with Graphic Elements</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// You do not create instances of this class directly.
    /// Use the <see cref="Fill.GradientStops"/> property to access gradient stops of fill objects.
    /// </remarks>
    public class GradientStopCollection : IEnumerable<GradientStop>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection" /> class.
        /// </summary>
        internal GradientStopCollection(DmlGradientFill parentFill, IThemeProvider themeProvider)
        {
            ArgumentUtil.CheckNotNull(parentFill, "ParentFill");

            mParentFill = parentFill;
            mThemeProvider = themeProvider;
        }

        /// <summary>
        /// Gets or sets a <see cref="GradientStop"/> object in the collection.
        /// </summary>
        public GradientStop this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index", ValueOutOfRange);

                GradientStop gradientStop = GradientStops[index];

                // Keep facade and internal gradient stops synchronized.
                if (!ReferenceEquals(gradientStop.DmlGradientStop, mParentFill.GradientStops[index]))
                    GradientStops[index] = new GradientStop(mParentFill.GradientStops[index], mThemeProvider, this);

                return gradientStop;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "GradientStop");
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index", ValueOutOfRange);

                // If the value we are going to set already belongs to some collection,
                // then we cannot reapply this to another collection or even set to another index in the same collection.
                if (value.ParentCollection != null)
                {
                    if (!ReferenceEquals(value.ParentCollection, this))
                        throw new InvalidOperationException("The gradient stop belongs to another collection already.");

                    if (!ReferenceEquals(GradientStops[index], value))
                        throw new InvalidOperationException("The gradient stop is set to another index already.");

                    return;
                }

                GradientStops[index] = value;
                value.ParentCollection = this;

                // Keep facade and internal gradient stops synchronized.
                mParentFill.GradientStops[index] = value.DmlGradientStop;
            }
        }

        /// <summary>
        /// Inserts a <see cref="GradientStop"/> to the collection at a specified index.
        /// </summary>
        public GradientStop Insert(int index, GradientStop gradientStop)
        {
            ArgumentUtil.CheckNotNull(gradientStop, "GradientStop");

            if ((index < 0) || (index > Count))
                throw new ArgumentOutOfRangeException("index", ValueOutOfRange);

            if (gradientStop.ParentCollection != null)
                throw new InvalidOperationException("The gradient stop belongs to some collection already.");

            // Add to facade.
            GradientStops.Insert(index, gradientStop);
            gradientStop.ParentCollection = this;
            // Add to internal.
            mParentFill.GradientStops.Insert(index, gradientStop.DmlGradientStop);

            return gradientStop;
        }

        /// <summary>
        /// Adds a specified <see cref="GradientStop"/> to a gradient.
        /// </summary>
        public GradientStop Add(GradientStop gradientStop)
        {
            return Insert(Count, gradientStop);
        }

        /// <summary>
        /// Removes a <see cref="GradientStop"/> from the collection at a specified index.
        /// </summary>
        /// <returns>Removed <see cref="GradientStop"/>.</returns>
        public GradientStop RemoveAt(int index)
        {
            if (Count <= 2)
                throw new InvalidOperationException("There can not be less than two gradient stops in gradient fill.");

            if ((index < 0) || (index >= Count))
                throw new ArgumentOutOfRangeException("index", ValueOutOfRange);

            GradientStop gradientStop = GradientStops[index];
            gradientStop.ParentCollection = null;
            // Remove facade.
            GradientStops.RemoveAt(index);
            // Remove from inner collection.
            mParentFill.GradientStops.RemoveAt(index);

            return gradientStop;
        }

        /// <summary>
        /// Removes a specified <see cref="GradientStop"/> from the collection.
        /// </summary>
        /// <returns><c>true</c> if gradient stop was successfully removed, otherwise <c>false</c>.</returns>
        public bool Remove(GradientStop gradientStop)
        {
            if (GradientStops.Remove(gradientStop))
            {
                gradientStop.ParentCollection = null;
                return mParentFill.GradientStops.Remove(gradientStop.DmlGradientStop);
            }

            return false;
        }

        #region IEnumerator implementation
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<GradientStop> GetEnumerator()
        {
            return GradientStops.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Gets an integer value indicating the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return mParentFill.GradientStops.Count; }
        }

        /// <summary>
        /// Gets parent <see cref="DmlGradientFill"/> object.
        /// </summary>
        internal DmlGradientFill ParentFill
        {
            get { return mParentFill; }
        }

        /// <summary>
        /// The collection of facade <see cref="GradientStop"/> objects.
        /// </summary>
        private IList<GradientStop> GradientStops
        {
            get
            {
                if (mGradientStops == null)
                {
                    mGradientStops = new List<GradientStop>(mParentFill.GradientStops.Count);
                    foreach (DmlGradientStop dmlGradientStop in mParentFill.GradientStops)
                        mGradientStops.Add(new GradientStop(dmlGradientStop, mThemeProvider, this));
                }

                return mGradientStops;
            }
        }

        /// <summary>
        /// The collection of facade <see cref="GradientStop"/> objects.
        /// </summary>
        private IList<GradientStop> mGradientStops;
        private readonly IThemeProvider mThemeProvider;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DmlGradientFill mParentFill;
        private const string ValueOutOfRange = "The specified value is out of range.";
    }
}
