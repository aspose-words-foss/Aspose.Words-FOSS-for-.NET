// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2013 by Ivan Lyagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a collection of <see cref="IFieldUpdateDataProvider"/> instances.
    /// </summary>
    internal class FieldUpdateDataProviderCollection : IEnumerable<IFieldUpdateDataProvider>
    {
        /// <summary>
        /// Adds the specified data provider to the collection.
        /// </summary>
        internal void Add(IFieldUpdateDataProvider dataProvider)
        {
            if (!HasDataProviders)
                mDataProviders = new List<IFieldUpdateDataProvider>(); // Create on the first demand.

            mDataProviders.Add(dataProvider);
        }

        /// <summary>
        /// Removes the specified data provider from the collection.
        /// </summary>
        internal void Remove(IFieldUpdateDataProvider dataProvider)
        {
            if (!HasDataProviders)
                return;

            mDataProviders.Remove(dataProvider);

            if (mDataProviders.Count == 0)
                mDataProviders = null;
        }

        /// <summary>
        /// Iterates over collected data providers and returns the first value provided for the specified field.
        /// </summary>
        internal Constant GetValue(Field field)
        {
            if (HasDataProviders)
            {
                foreach (IFieldUpdateDataProvider dataProvider in mDataProviders)
                {
                    Constant value = dataProvider.GetValue(field);
                    if (value != null)
                        return value;
                }
            }

            return null;
        }

        /// <summary>
        /// Iterates over collected data providers and returns the first data object provided for the specified field.
        /// </summary>
        internal object GetData(Field field)
        {
            if (HasDataProviders)
            {
                foreach (IFieldUpdateDataProvider dataProvider in mDataProviders)
                {
                    object data = dataProvider.GetData(field);
                    if (data != null)
                        return data;
                }
            }

            return null;
        }

        public IEnumerator<IFieldUpdateDataProvider> GetEnumerator()
        {
            return HasDataProviders
                ? mDataProviders.GetEnumerator()
                : EmptyEnumerator<IFieldUpdateDataProvider>.GetInstance();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool HasDataProviders
        {
            get { return (mDataProviders != null); }
        }

        private List<IFieldUpdateDataProvider> mDataProviders;
    }
}
