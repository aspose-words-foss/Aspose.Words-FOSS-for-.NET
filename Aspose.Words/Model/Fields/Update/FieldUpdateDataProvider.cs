// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2015 by Edward Voronov

using System;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents access to <see cref="IFieldUpdateDataProvider"/>.
    /// </summary>
    internal class FieldUpdateDataProvider
    {
        internal FieldUpdateDataProvider(FieldUpdater updater, FieldUpdateDataProviderCollection dataProviderCollection)
        {
            mUpdater = updater;
            mDataProviders = dataProviderCollection;
        }

        /// <summary>
        /// Adds the specified data provider.
        /// </summary>
        internal void Add(IFieldUpdateDataProvider dataProvider)
        {
            mDataProviders.Add(dataProvider);
        }

        internal T Ensure<T>(T dataProvider)
            where T: IFieldUpdateDataProvider
        {
            IFieldUpdateDataProvider result = GetOfType(dataProvider.GetType());
            if (result != null)
                return (T)result;

            mDataProviders.Add(dataProvider);

            return dataProvider;
        }

        /// <summary>
        /// Removes the specified data provider.
        /// </summary>
        internal void Remove(IFieldUpdateDataProvider dataProvider)
        {
            mDataProviders.Remove(dataProvider);
        }

        /// <summary>
        /// Returns the first value provided for the specified field.
        /// </summary>
        internal Constant GetValue(Field field)
        {
            // Treat display context as a data provider either.
            if (mUpdater.DisplayContext != null)
            {
                Constant value = mUpdater.DisplayContext.GetFieldValue(field);
                if (value != null)
                    return value;
            }

            return mDataProviders.GetValue(field);
        }

        /// <summary>
        /// Returns the first data object provided for the specified field.
        /// </summary>
        internal object GetData(Field field)
        {
            return mDataProviders.GetData(field);
        }

        internal IFieldUpdateDataProvider GetOfType(Type type)
        {
            IFieldUpdateDataProvider result = null;

            foreach (IFieldUpdateDataProvider provider in mDataProviders)
                if (provider.GetType() == type)
                    result = provider;

            return result;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly FieldUpdater mUpdater;
        private readonly FieldUpdateDataProviderCollection mDataProviders;
    }
}
