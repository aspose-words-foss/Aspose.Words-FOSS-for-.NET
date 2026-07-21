// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2019 by Alexander Sevidov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Represents a collection of <see cref="VbaReference"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-vba-macros/">Working with VBA Macros</a> documentation article.</para>
    /// </summary>
    public sealed class VbaReferenceCollection : IEnumerable<VbaReference>
    {
        /// <summary>
        /// Initializes a new instance of the VbaReferenceCollection class for a specified VbaProject.
        /// </summary>
        internal VbaReferenceCollection(VbaProject vbaProject)
        {
            mVbaProject = vbaProject;
        }

        /// <summary>
        /// Removes the first occurrence of a specified <see cref="VbaReference"/> item from the collection. 
        /// </summary>
        public void Remove(VbaReference item)
        {
            mReferences.Remove(item);
            mVbaProject.MarkModified();
        }

        /// <summary>
        /// Removes the <see cref="VbaReference"/> element at the specified index of the collection.
        /// </summary>
        public void RemoveAt(int index)
        {
            mReferences.RemoveAt(index);
            mVbaProject.MarkModified();
        }

        /// <summary>
        /// Adds a specified VbaReference object to a collection.
        /// </summary>
        /// <param name="vbaReference"></param>
        internal void Add(VbaReference vbaReference)
        {
            mReferences.Add(vbaReference);
        }

        /// <summary>
        /// Makes a copy of the object.
        /// </summary>
        internal VbaReferenceCollection Clone()
        {
            VbaReferenceCollection lhs = new VbaReferenceCollection(mVbaProject);

            foreach (VbaReference reference in mReferences)
                lhs.Add(reference.Clone());

            return lhs;
        }

        IEnumerator<VbaReference> IEnumerable<VbaReference>.GetEnumerator()
        {
            return mReferences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mReferences.GetEnumerator();
        }

        /// <summary>
        /// Returns the number of VBA references in the collection.
        /// </summary>
        public int Count
        {
            get { return mReferences.Count; }
        }

        /// <summary>
        /// Gets <see cref="VbaReference"/> object at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the reference to get.</param>
        public VbaReference this[int index]
        {
            get { return mReferences[index]; }
        }

        private readonly List<VbaReference> mReferences = new List<VbaReference>();
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly VbaProject mVbaProject;
    }
}
