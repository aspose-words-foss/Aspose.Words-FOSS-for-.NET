// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2017 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// An implementation of <see cref="IFieldResultFormatApplier"/> that contains a collection of appliers
    /// and applies all of them one by one.
    /// </summary>
    internal class CompositeFieldResultFormatApplier : IFieldResultFormatApplier
    {
        void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
        {
            foreach (IFieldResultFormatApplier applier in mAppliers)
                applier.ApplyFormat(result);
        }

        internal void AddApplier(IFieldResultFormatApplier applier)
        {
            mAppliers.Add(applier);
        }

        private readonly List<IFieldResultFormatApplier> mAppliers = new List<IFieldResultFormatApplier>();
    }
}