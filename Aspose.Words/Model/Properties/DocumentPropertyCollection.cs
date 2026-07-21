// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2005 by Roman Korchagin
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Properties
{
    /// <summary>
    /// Base class for <see cref="BuiltInDocumentProperties"/> and <see cref="CustomDocumentProperties"/> collections.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-document-properties/">Work with Document Properties</a> documentation article.</para>
    /// </summary>
    /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.Common"]/*'/>
    /// <seealso cref="BuiltInDocumentProperties"/>
    /// <seealso cref="CustomDocumentProperties"/>
    public abstract class DocumentPropertyCollection : IEnumerable<DocumentProperty>
    {
         /// <summary>
        /// Ctor.
        /// </summary>
        internal DocumentPropertyCollection()
        {
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <overloads>Provides access to the collection items.</overloads>
        /// <summary>
        /// Returns a <see cref="DocumentProperty"/> object by the name of the property.
        /// </summary>
        /// <remarks>
        /// <p>Returns <c>null</c> if a property with the specified name is not found.</p>
        /// </remarks>
        /// <param name="name">The case-insensitive name of the property to retrieve.</param>
        public virtual DocumentProperty this[string name]
        {
            get
            {
                ArgumentUtil.CheckHasChars(name, "name");
                return mItems.GetValueOrNull(name);
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Returns a <see cref="DocumentProperty"/> object by the name of the property.
        /// </summary>
        public DocumentProperty GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Returns a <see cref="DocumentProperty"/> object by index.
        /// </summary>
        /// <java><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.JavaSlow"]/*'/></java>
        /// <param name="index">Zero-based index of the <see cref="DocumentProperty"/> to retrieve.</param>
        public DocumentProperty this[int index]
        {
            get { return mItems.GetByIndex(index); }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<DocumentProperty> GetEnumerator()
        {
            // RK This was originally implemented as iterator over items, not over dictionary entries
            // and has to stay that way to avoid breaking customers' code.
            return mItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds new property to the collection or returns existing property if one with specified name already exists.
        /// Throws if <paramref name="name"/> is empty [OR] <paramref name="value"/> is <c>null</c>.
        /// </summary>
        /// <param name="name">Name of added property.</param>
        /// <param name="value">Value of added property.</param>
        /// <returns>Added property.</returns>
        /// <remarks>
        /// Compare the method with <see cref="Add"/>.
        /// </remarks>
        internal DocumentProperty AddSafe(string name, object value)
        {
            ArgumentUtil.CheckHasChars(name, "name");
            ArgumentUtil.CheckNotNull(value, "value");

            DocumentProperty property = mItems.GetValueOrNull(name);
            return (property != null) ? property : Add(name, value);
        }

        /// <summary>
        /// Adds new property to the collection. Throws if <paramref name="name"/> is empty [OR]
        /// <paramref name="value"/> is <c>null</c> [OR] property with specified name already exists.
        /// </summary>
        /// <param name="name">Name of added property.</param>
        /// <param name="value">Value of added property.</param>
        /// <returns>Added property.</returns>
        /// <remarks>
        /// Compare the method with <see cref="AddSafe"/>.
        /// </remarks>
        internal DocumentProperty Add(string name, object value)
        {
            ArgumentUtil.CheckHasChars(name, "name");
            ArgumentUtil.CheckNotNull(value, "value");

            DocumentProperty property = new DocumentProperty(name, value);
            mItems.Add(name, property);

            return property;
        }

        /// <summary>
        /// Returns <c>true</c> if a property with the specified name exists in the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the property.</param>
        /// <returns><c>true</c> if the property exists in the collection; <c>false</c> otherwise.</returns>
        public bool Contains(string name)
        {
            return mItems.ContainsKey(name);
        }

        /// <summary>
        /// Gets the index of a property by name.
        /// </summary>
        /// <java><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.JavaSlow"]/*'/></java>
        /// <param name="name">The case-insensitive name of the property.</param>
        /// <returns>The zero based index. Negative value if not found.</returns>
        public int IndexOf(string name)
        {
            return mItems.IndexOfKey(name);
        }

        /// <summary>
        /// Removes a property with the specified name from the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the property.</param>
        public void Remove(string name)
        {
            ArgumentUtil.CheckHasChars(name, "name");
            mItems.Remove(name);
        }

        /// <summary>
        /// Removes a property at the specified index.
        /// </summary>
        /// <java><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.JavaSlow"]/*'/></java>
        /// <param name="index">The zero based index.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes all properties from the collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Makes a copy of the object.
        /// </summary>
        internal DocumentPropertyCollection Clone()
        {
            DocumentPropertyCollection lhs = Create();

            foreach (KeyValuePair<string, DocumentProperty> item in mItems)
            {
                DocumentProperty property = item.Value;
                lhs.mItems.Add(item.Key, property.Clone());
            }

            return lhs;
        }

        /// <summary>
        /// Virtual constructor. Creates empty collection of the same type as object whose method was called.
        /// Used as part of <see cref="Clone"/> operation.
        /// </summary>
        /// <remarks>
        /// This method should be internal AND protected but there is no such modifier in C# and it's more important
        /// to hide it from end user than from other classes in the assembly. So it's marked as internal.
        /// </remarks>
        internal abstract DocumentPropertyCollection Create();

        private readonly SortedStringListGeneric<DocumentProperty> mItems = new SortedStringListGeneric<DocumentProperty>(false);
    }
}
