// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="Section"/> objects in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-sections/">Working with Sections</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A Microsoft Word document can contain multiple sections. To create a section in a Microsoft Word,
    /// select the Insert/Break command and select a break type. The break specifies whether section starts
    /// on a new page or on the same page.</p>
    ///
    /// <p>Programmatically inserting and removing sections can be used to customize documents produced
    /// during mail merge. If a document needs to have different content or parts of the
    /// content depending on some criteria, then you can create a "master" document that contains
    /// multiple sections and delete some of the sections before or after mail merge.</p>
    /// </remarks>
    [JavaGenericArguments("NodeCollection<Section>")]
    public class SectionCollection : NodeCollection
    {
        internal SectionCollection(CompositeNode parent) : base(parent, NodeType.Section, false)
        {
        }

        /// <summary>
        /// Retrieves a section at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the list of sections.</param>
        public new Section this[int index]
        {
            get { return (Section)base[index]; }
        }

        /// <summary>
        /// Copies all sections from the collection to a new array of sections.
        /// </summary>
        /// <returns>An array of sections.</returns>
        public new Section[] ToArray()
        {
            return ToList<Section>().ToArray();
        }
    }
}
