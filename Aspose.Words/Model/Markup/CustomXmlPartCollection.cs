// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/05/2010 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Xml;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a collection of Custom XML Parts. The items are <see cref="CustomXmlPart"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not normally need to create instances of this class. You can access custom XML data
    /// stored in a document via the <see cref="Document.CustomXmlParts"/> property.</para>
    ///
    /// <seealso cref="CustomXmlPart"/>
    /// <seealso cref="Document.CustomXmlParts"/>
    /// </remarks>
    public class CustomXmlPartCollection : IEnumerable<CustomXmlPart>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets an item at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the item.</param>
        public CustomXmlPart this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<CustomXmlPart> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="part">The custom XML part to add.</param>
        public void Add(CustomXmlPart part)
        {
            mItems.Add(part);
        }

        /// <summary>
        /// Creates a new XML part with the specified XML and adds it to the collection.
        /// </summary>
        /// <param name="id">Identifier of a new custom XML part.</param>
        /// <param name="xml">XML data of the part.</param>
        /// <returns>Created custom XML part.</returns>
        public CustomXmlPart Add(string id, string xml)
        {
            CustomXmlPart part = new CustomXmlPart();
            part.Id = id;
            part.Data = Encoding.UTF8.GetBytes(xml);

            mItems.Add(part);
            return part;
        }

        /// <summary>
        /// Removes an item at the specified index.
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
            DataStoreRaw = null;
        }

        /// <summary>
        /// Finds and returns a custom XML part by its identifier.
        /// </summary>
        /// <param name="id">Case-sensitive string that identifies the custom XML part.</param>
        /// <returns>Returns <c>null</c> if a custom XML part with the specified identifier is not found.</returns>
        public CustomXmlPart GetById(string id)
        {
            foreach (CustomXmlPart part in mItems)
            {
                if (part.Id == id)
                    return part;
            }

            return null;
        }

        /// <summary>
        /// Makes a deep copy of this collection and its items.
        /// </summary>
        public CustomXmlPartCollection Clone()
        {
            // We don't have derived classes therefore we can simply create a new object, don't memberwise clone.
            CustomXmlPartCollection lhs = new CustomXmlPartCollection();

            foreach (CustomXmlPart item in this)
                lhs.Add(item.Clone());

            if (DataStoreRaw != null)
            {
                lhs.DataStoreRaw = new byte[DataStoreRaw.Length];
                DataStoreRaw.CopyTo(lhs.DataStoreRaw, 0);
            }

            return lhs;
        }

        /// <summary>
        /// Returns <see cref="CustomXmlPart"/> corresponded to a Citation sources.
        /// <seealso cref="SdtCitation"/>
        /// <seealso cref="Fields.FieldCitation"/>
        /// </summary>
        internal CustomXmlPart GetCitationSources()
        {
            foreach (CustomXmlPart xmlPart in mItems)
            {
                // WORDSNET-28059 Resilience against zero length xml part.
                if (!ArrayUtil.HasData(xmlPart.Data))
                    continue;

                using (MemoryStream stream = new MemoryStream(xmlPart.Data))
                {
                    XmlDocument xml = XmlUtilPal.LoadXml(stream, true);
                    if ((xml.DocumentElement != null) && (xml.DocumentElement.Name == CitationSourcesElementName))
                        return xmlPart;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        internal bool Contains(CustomXmlPart item)
        {
            return mItems.Contains(item);
        }

        /// <summary>
        /// Gets/sets raw datastore element for Rtf2Rtf scenario.
        /// </summary>
        /// <remarks>
        /// See remarks for TestJira13324.
        /// </remarks>
        internal byte[] DataStoreRaw { get; set; }

        /// <summary>
        /// The name of Citation sources element in a corresponding <see cref="CustomXmlPart"/>.
        /// </summary>
        internal const string CitationSourcesElementName = "b:Sources";

        private readonly List<CustomXmlPart> mItems = new List<CustomXmlPart>();
    }
}
