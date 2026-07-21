// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2009 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.BuildingBlocks
{
    /// <summary>
    /// A collection of <see cref="BuildingBlock"/> objects in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class directly. To access a collection 
    /// of building blocks use the <see cref="GlossaryDocument.BuildingBlocks"/> property.</para>
    /// 
    /// <seealso cref="GlossaryDocument"/>
    /// <seealso cref="GlossaryDocument.BuildingBlocks"/>
    /// <seealso cref="BuildingBlock"/>
    /// </remarks>
    [JavaGenericArguments("NodeCollection<BuildingBlock>")]
    public class BuildingBlockCollection : NodeCollection
    {
        internal BuildingBlockCollection(GlossaryDocument parent) : base(parent, NodeType.BuildingBlock, false)
        {
        }

        /// <summary>
        /// Retrieves a building block at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the list of building blocks.</param>
        public new BuildingBlock this[int index]
        {
            get { return (BuildingBlock)base[index]; }
        }

        /// <summary>
        /// Copies all building blocks from the collection to a new array of building blocks.
        /// </summary>
        /// <returns>An array of building blocks.</returns>
        public new BuildingBlock[] ToArray()
        {
            return ToList<BuildingBlock>().ToArray();
        }
    }
}
