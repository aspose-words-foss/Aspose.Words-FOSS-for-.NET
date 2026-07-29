// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Cell"/> nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    [JavaGenericArguments("NodeCollection<Cell>")]
    public class CellCollection : NodeCollection
    {
        internal CellCollection(CompositeNode parent) : base(parent, NodeType.Cell, false)
        {
        }

        /// <summary>
        /// Retrieves a <see cref="Cell"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Cell this[int index]
        {
            get { return (Cell)base[index]; }
        }

        /// <summary>
        /// Copies all cells from the collection to a new array of cells.
        /// </summary>
        /// <returns>An array of cells.</returns>
        public new Cell[] ToArray()
        {
            return ToList<Cell>().ToArray();
        }

        /// <summary>
        /// Returns true if any cell has format revision.
        /// </summary>
        internal bool HasFormatRevision
        {
            get
            {
                foreach (Cell cell in this)
                    if(cell.CellPr.HasFormatRevision)
                        return true;

                return false;
            }
        }
    }
}
