// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2013 by Alexey Morozov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Provides collection for <see cref="Reference"/> objects and <see cref="IReferenceResolver"/> used to resolve references.
    /// </summary>
    internal class ReferenceCollection : IEnumerable<Reference>
    {
        internal ReferenceCollection(IReferenceResolver resolver)
        {
            mReferenceResolver = resolver;
        }

        internal void Add(Reference reference)
        {
            mItems.Add(reference);
            reference.ReferenceResolver = mReferenceResolver;
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        public IEnumerator<Reference> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly IReferenceResolver mReferenceResolver;
        private readonly List<Reference> mItems = new List<Reference>();
    }
}
