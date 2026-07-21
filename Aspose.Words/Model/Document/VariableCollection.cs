// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2005 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of document variables.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-document-properties/">Work with Document Properties</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Variable names and values are strings.</p>
    /// <p>Variable names are case-insensitive.</p>
    /// </remarks>
    public class VariableCollection : IEnumerable<KeyValuePair<string, string>>
    {
        internal VariableCollection()
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
        /// Gets or a sets a document variable by the case-insensitive name.
        /// <c>null</c> values are not allowed as a right hand side of the assignment and will be replaced by empty string.
        /// </summary>
        public string this[string name]
        {
            get
            {
                ArgumentUtil.CheckHasChars(name, "name");
                return mItems.GetSafe(name,"");
            }
            set
            {
                ArgumentUtil.CheckHasChars(name, "name");
                // andrnosk: WORDSNET-5100 If value is not null use value otherwise use empty string.
                mItems[name] = StringUtil.HasChars(value) ? value : "";
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Gets or a sets a document variable by the case-insensitive name.
        /// <c>null</c> values are not allowed as a right hand side of the assignment and will be replaced by empty string.
        /// </summary>
        public string GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Gets or sets a document variable at the specified index.
        /// <c>null</c> values are not allowed as a right hand side of the assignment and will be replaced by empty string.
        /// </summary>
        /// <param name="index">Zero-based index of the document variable.</param>
        public string this[int index]
        {
            get
            {
                return mItems.Values[index];
            }
            // andrnosk: WORDSNET-5100 If value is not null use value otherwise use empty string.
            set
            {
                string key = mItems.Keys[index];
                mItems[key] = (StringUtil.HasChars(value))
                    ? value
                    : "";
            }
        }

        /// <summary>
        /// Returns an enumerator object that can be used to iterate over all variable in the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a document variable to the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the variable to add.</param>
        /// <param name="value">The value of the variable. The value cannot be <c>null</c>, if value is null empty string will be used instead.</param>
        public void Add(string name, string value)
        {
            // RESILIENCY 16524 - The problem occurred because there is few variables with the same name in the document. 
            // andrnosk: WORDSNET-5100 If value is not null use value otherwise use empty string.
            mItems[name] = StringUtil.HasChars(value) ? value : "";
        }

        /// <summary>
        /// Determines whether the collection contains a document variable with the given name.
        /// </summary>
        /// <param name="name">Case-insensitive name of the document variable to locate.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return mItems.ContainsKey(name);
        }

        /// <summary>
        /// Returns the zero-based index of the specified document variable in the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the variable.</param>
        /// <returns>The zero based index. Negative value if not found.</returns>
        public int IndexOfKey(string name)
        {
            return mItems.IndexOfKey(name);
        }

        /// <summary>
        /// Removes a document variable with the specified name from the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the variable.</param>
        public void Remove(string name)
        {
            mItems.Remove(name);
        }

        /// <summary>
        /// Removes a document variable at the specified index.
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
        internal VariableCollection Clone()
        {
            // We don't have derived classes therefore we simply create a new object, don't memberwise clone.
            VariableCollection lhs = new VariableCollection();

            foreach (KeyValuePair<string, string> entry in this)
                lhs.Add(entry.Key, entry.Value);

            return lhs;
        }

        private readonly SortedStringListGeneric<string> mItems = new SortedStringListGeneric<string>(false);
    }
}
