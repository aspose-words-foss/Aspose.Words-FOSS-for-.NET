// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2008 by Roman Korchagin
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a collection of custom XML attributes or smart tag properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Items are <see cref="CustomXmlProperty"/> objects.</para>
    /// </remarks>
    public class CustomXmlPropertyCollection : IEnumerable<CustomXmlProperty>
    {
        /// <summary>
        /// Don't need public ctor.
        /// </summary>
        internal CustomXmlPropertyCollection()
        {
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <overloads>Provides access to the collection items.</overloads>
        /// <summary>
        /// Gets a property with the specified name.
        /// </summary>
        /// <param name="name">Case-sensitive name of the property to locate.</param>
        public CustomXmlProperty this[string name]
        {
            get { return mItems.GetValueOrNull(name); }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Gets a property with the specified name.
        /// </summary>
        public CustomXmlProperty GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Gets a property at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the property.</param>
        public CustomXmlProperty this[int index]
        {
            get { return mItems.GetByIndex(index); }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<CustomXmlProperty> GetEnumerator()
        {
            return mItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a property to the collection.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(CustomXmlProperty property)
        {
            mItems.Add(property.Name, property);
        }

        /// <summary>
        /// Adds a property to the collection. Replaces if the property with the given name already exists.
        /// Useful when loading documents.
        /// </summary>
        internal void AddSafe(CustomXmlProperty property)
        {
            mItems[property.Name] = property;
        }

        /// <summary>
        /// Determines whether the collection contains a property with the given name.
        /// </summary>
        /// <param name="name">Case-sensitive name of the property to locate.</param>
        /// <returns><c>true</c> if the item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return mItems.ContainsKey(name);
        }

        /// <summary>
        /// Returns the zero-based index of the specified property in the collection.
        /// </summary>
        /// <param name="name">The case-sensitive name of the property.</param>
        /// <returns>The zero based index. Negative value if not found.</returns>
        public int IndexOfKey(string name)
        {
            return mItems.IndexOfKey(name);
        }

        /// <summary>
        /// Removes a property with the specified name from the collection.
        /// </summary>
        /// <param name="name">The case-sensitive name of the property.</param>
        public void Remove(string name)
        {
            mItems.Remove(name);
        }

        /// <summary>
        /// Removes a property at the specified index.
        /// </summary>
        /// <param name="index">The zero based index.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Makes a deep copy of the collection.
        /// </summary>
        internal CustomXmlPropertyCollection Clone()
        {
            // We don't have derived classes therefore we simply create a new object, don't memberwise clone.
            CustomXmlPropertyCollection lhs = new CustomXmlPropertyCollection();

            foreach (CustomXmlProperty property in this)
                lhs.Add(property.Clone());

            return lhs;
        }

        private readonly SortedStringListGeneric<CustomXmlProperty> mItems = new SortedStringListGeneric<CustomXmlProperty>();
    }
}
