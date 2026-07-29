// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides method for reading story-level elements, 
    /// i.e any element that can contain paragraphs or tables.
    /// NOTE: Derived classes should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxStoryReaderBase
    {
        /// <summary>
        /// Reads all children of the current element into the specified node. 
        /// </summary>
        /// <param name="reader">Parser used to read the document.</param>
        /// <param name="story">Node where to read the story to.</param>
        [JavaThrows(true)]
        internal abstract void Read(NrxDocumentReaderBase reader, CompositeNode story);

        /// <summary>
        /// Reads all children of the current element into the current container.
        /// The current element is supposed to be a story element such as comment, footnote, header/footer or a shape.
        /// The children are supposed to be block-level elements.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void ReadChildren(NrxDocumentReaderBase reader);

        /// <summary>
        /// Reads one story child element (paragraph, table, bookmark start or end) and appends 
        /// to the current container in the model builder.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void ReadChild(NrxDocumentReaderBase reader);
    }
}