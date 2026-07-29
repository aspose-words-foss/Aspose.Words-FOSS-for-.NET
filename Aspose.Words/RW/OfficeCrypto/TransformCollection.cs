// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2013 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements collection for <see cref="Transform"/> objects.
    /// </summary>
    internal class TransformCollection : IEnumerable<Transform>
    {
        // Appends Transform object to collection.
        internal void Add(Transform transform)
        {
            mItems.Add(transform);
        }

        /// <summary>
        /// Reads transform collection from XML.
        /// </summary>
        /// <param name="reader"></param>
        internal void Read(AnyXmlReader reader)
        {
            while (reader.ReadChild("Transforms"))
                if (reader.LocalName == "Transform")
                    Add(Transform.Create(reader));
                else
                    throw new InvalidOperationException("");
        }

        /// <summary>
        /// Writes TransformCollection in XML form.
        /// </summary>
        internal void Write(AnyXmlBuilder writer)
        {
            if (mItems.Count == 0)
                return;

            writer.StartElement("Transforms");

            foreach (Transform transform in mItems)
                transform.Write(writer);

            writer.EndElement("Transforms");
        }

        /// <summary>
        /// Applies transformation to stream and return transformed data.
        /// </summary>
        internal MemoryStream Apply(MemoryStream data)
        {
            foreach (Transform transform in mItems)
                data = transform.Apply(data);

            return data;
        }

        public IEnumerator<Transform> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<Transform> mItems = new List<Transform>();
    }
}
