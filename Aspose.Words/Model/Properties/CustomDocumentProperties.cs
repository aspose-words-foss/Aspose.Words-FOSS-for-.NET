// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2004 by Roman Korchagin
using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Properties
{
    /// <summary>
    /// A collection of custom document properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-document-properties/">Work with Document Properties</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Each <see cref="DocumentProperty"/> object represents a custom property of a container document.</p>
    /// </remarks>
    /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.Common"]/*'/>
    /// <seealso cref="Document"/>
    /// <seealso cref="Document.BuiltInDocumentProperties"/>
    /// <seealso cref="Document.CustomDocumentProperties"/>
    public class CustomDocumentProperties : DocumentPropertyCollection
    {
        internal CustomDocumentProperties(Document document)
        {
            mDocument = document;
        }

        internal CustomDocumentProperties()
        {
        }

        /// <overloads>Creates a new custom document property.</overloads>
        /// <summary>
        /// Creates a new custom document property of the <see cref="PropertyType.String"/> data type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The newly created property object.</returns>
        public DocumentProperty Add(string name, [CppForceStringParam] string value)
        {
            return base.Add(name, value);
        }

        /// <summary>
        /// Creates a new custom document property of the <see cref="PropertyType.Number"/> data type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The newly created property object.</returns>
        public DocumentProperty Add(string name, int value)
        {
            return base.Add(name, value);
        }

        /// <summary>
        /// Creates a new custom document property of the <see cref="PropertyType.DateTime"/> data type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The newly created property object.</returns>
        public DocumentProperty Add(string name, DateTime value)
        {
            return base.Add(name, value);
        }

        /// <summary>
        /// Creates a new custom document property of the <see cref="PropertyType.Boolean"/> data type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The newly created property object.</returns>
        public DocumentProperty Add(string name, bool value)
        {
            return base.Add(name, value);
        }

        /// <summary>
        /// Creates a new custom document property of the <see cref="PropertyType.Double"/> data type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The newly created property object.</returns>
        public DocumentProperty Add(string name, double value)
        {
            return base.Add(name, value);
        }

        /// <summary>
        /// Creates a new linked to content custom document property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="linkSource">The source of the property.</param>
        /// <returns>The newly created property object or <c>null</c> when the <paramref name="linkSource"/> is invalid.</returns>
        public DocumentProperty AddLinkToContent(string name, string linkSource)
        {
            string value = GetLinkedContent(mDocument, linkSource);

            // MSW allows to create a linked content only for valid bookmarks.
            if (value == null)
                return null;

            DocumentProperty linkedProperty = base.Add(name, value);
            linkedProperty.LinkTarget = linkSource;

            return linkedProperty;
        }

        internal override DocumentPropertyCollection Create()
        {
            return new CustomDocumentProperties(mDocument);
        }

        internal void SetDocument(Document document)
        {
            mDocument = document;
        }

        /// <summary>
        /// Gets a linked content (a bookmark text) from the document.
        /// </summary>
        private static string GetLinkedContent(Document document, string linkTarget)
        {
            Bookmark bookmark = document.Range.Bookmarks[linkTarget];

            return (bookmark != null) ? bookmark.GetText(true) : null;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Document mDocument;
    }
}
