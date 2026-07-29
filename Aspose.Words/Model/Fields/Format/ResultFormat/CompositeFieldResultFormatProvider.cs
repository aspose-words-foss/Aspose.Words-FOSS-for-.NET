// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2017 by Edward Voronov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// An implementation of <see cref="IFieldResultFormatProvider"/> that contains a collection of providers
    /// and uses all of them one by one to build <see cref="CompositeFieldResultFormatApplier"/>.
    /// </summary>
    internal class CompositeFieldResultFormatProvider : IFieldResultFormatProvider
    {
        Inline IFieldResultFormatProvider.GetSourceNode()
        {
            if (IsEmpty)
                return null;

            for (int i = mProviders.Count - 1; i >= 0; i--)
            {
                Inline node = mProviders[i].GetSourceNode();
                if (node != null)
                    return node;
            }

            return null;
        }

        IFieldResultFormatApplier IFieldResultFormatProvider.GetFormatApplier()
        {
            CompositeFieldResultFormatApplier applier = new CompositeFieldResultFormatApplier();

            foreach (IFieldResultFormatProvider provider in mProviders)
                applier.AddApplier(provider.GetFormatApplier());

            return applier;
        }

        /// <summary>
        /// Inserts <paramref name="provider"/> at the beginning.
        /// Does not insert provider if it already exists.
        /// </summary>
        internal void InsertProvider(IFieldResultFormatProvider provider)
        {
            if (HasProvider(provider))
                return;

            mProviders.Insert(0, provider);
        }

        /// <summary>
        /// Appends <paramref name="provider"/> to the end.
        /// Does not append provider if it already exists.
        /// </summary>
        internal void AppendProvider(IFieldResultFormatProvider provider)
        {
            if (HasProvider(provider))
                return;

            mProviders.Add(provider);
        }

        internal bool IsEmpty
        {
            get { return mProviders.Count == 0; }
        }

        private bool HasProvider(IFieldResultFormatProvider provider)
        {
            Type providerType = provider.GetType();

            foreach (IFieldResultFormatProvider existedProvider in mProviders)
                if (existedProvider.GetType() == providerType)
                    return true;

            return false;
        }

        private readonly List<IFieldResultFormatProvider> mProviders = new List<IFieldResultFormatProvider>();
    }
}
