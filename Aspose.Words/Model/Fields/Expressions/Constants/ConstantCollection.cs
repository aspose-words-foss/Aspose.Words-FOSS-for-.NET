// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/11/2006 by Dmitry Vorobyev

using System.Collections;
using System.Collections.Generic;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a collection of constants.
    /// </summary>
    internal class ConstantCollection : IEnumerable<Constant>
    {
        /// <summary>
        /// Adds a constant to the collection.
        /// </summary>
        /// <param name="constant">The constant to add.</param>
        internal void Add(Constant constant)
        {
            mItems.Add(constant);
        }

        /// <summary>
        /// Adds a range of constants to the collection.
        /// </summary>
        internal void AddRange(ConstantCollection constantCollection)
        {
            mItems.AddRange(constantCollection.mItems);
        }

        public IEnumerator<Constant> GetEnumerator()
        {
            return new ConstantEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a constant at the specified index.
        /// </summary>
        internal Constant this[int index]
        {
            get
            {
                Debug.Assert((index >= 0) && (index < mItems.Count));
                return mItems[index];
            }
        }

        /// <summary>
        /// Return the total number of constants in this collection.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Enumerates constants in the specified collection. If a constant is null, slips it. If a constant
        /// is aggregate, enumerates all child constants.
        /// </summary>
        private sealed class ConstantEnumerator : IEnumerator<Constant>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal ConstantEnumerator(ConstantCollection constants)
            {
                mConstants = constants;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public bool MoveNext()
            {
                if (IsInAggregate)
                {
                    if (mAggregateEnumerator.MoveNext())
                    {
                        return true;
                    }
                    else
                    {
                        mAggregateEnumerator = null;
                    }
                }

                while (true)    // Break inside.
                {
                    mIndex++;

                    // RK Should this be done using HasNextCore?
                    if (mIndex == mConstants.Count)
                        return false;

                    if (Current.ConstantType == ConstantType.Aggregate)
                    {
                        AggregateConstant aggregate = (AggregateConstant)Current;
                        mAggregateEnumerator = (ConstantEnumerator)aggregate.Values.GetEnumerator();

                        if (!mAggregateEnumerator.MoveNext())
                        {
                            mAggregateEnumerator = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        mAggregateEnumerator = null;
                        return true;
                    }
                }
            }

            public void Reset()
            {
                mIndex = -1;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private bool IsInAggregate
            {
                [CppConstMethod]
                get { return mAggregateEnumerator != null; }
            }

            public Constant Current
            {
                get { return IsInAggregate ? mAggregateEnumerator.Current : mConstants[mIndex]; }
            }

            private readonly ConstantCollection mConstants;
            private int mIndex = -1;
            private ConstantEnumerator mAggregateEnumerator;
        }

        private readonly List<Constant> mItems = new List<Constant>();
    }
}
