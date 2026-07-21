// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Paragraph"/> nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-paragraphs/">Working with Paragraphs</a> documentation article.</para>
    /// </summary>
    [JavaGenericArguments("NodeCollection<Paragraph>")]
    public class ParagraphCollection : NodeCollection
    {
        internal ParagraphCollection(CompositeNode parent) : base(parent, NodeType.Paragraph, false)
        {
        }

        /// <summary>
        /// Retrieves a <see cref="Paragraph"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Paragraph this[int index]
        {
            get { return (Paragraph)base[index]; }
        }

        /// <summary>
        /// Copies all paragraphs from the collection to a new array of paragraphs.
        /// </summary>
        /// <returns>An array of paragraphs.</returns>
        public new Paragraph[] ToArray()
        {
            return ToList<Paragraph>().ToArray();
        }
    }
}
