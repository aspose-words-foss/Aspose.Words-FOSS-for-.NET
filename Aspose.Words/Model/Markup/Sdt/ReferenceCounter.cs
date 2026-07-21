// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/19/2011 by Denis Darkin 

using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Counts references of objects <see cref="object"/>, and allows checking if a particular object has been referenced or not.
    /// </summary>
    internal class ReferenceCounter
    {
        /// <summary>
        /// Resets counters for any references.
        /// </summary>
        internal void Reset()
        {
            mStorage.Clear();
        }

        /// <summary>
        /// Increment reference counter for given obj by one.
        /// </summary>
        internal void IncrementReference(object obj)
        {
            int number = mStorage[obj];
            if (!ObjToIntDictionary<object>.IsNullSubstitute(number))
            {
                Debug.Assert(number != int.MaxValue);
                mStorage[obj] = ++number;
            }
            else
            {
                mStorage.Add(obj, 1);
            }
        }

        /// <summary>
        /// Decrement reference counter for a given obj by one.
        /// </summary>
        internal void DecrementReference(object  obj)
        {
            int number = mStorage[obj];
            if (!ObjToIntDictionary<object>.IsNullSubstitute(number))
            {
                if (number > 0)
                    number--;

                if (number == 0)
                    mStorage.Remove(obj);
                else
                    mStorage[obj] = number;
            }
        }

        /// <summary>
        /// Returns true if this object has non-zero reference counter.
        /// </summary>
        internal bool IsReferenced(object obj)
        {
            return mStorage.ContainsKey(obj);
        }

        /// <summary>
        /// Returns a collection of objects containing non-zero reference count.
        /// </summary>
        internal IEnumerable<object> ReferencedObjects
        {
            get { return mStorage.Keys; }
        }

        private readonly ObjToIntDictionary<object> mStorage = new ObjToIntDictionary<object>();
    }
}
