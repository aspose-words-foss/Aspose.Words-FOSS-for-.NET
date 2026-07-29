// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Table"/> nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    [JavaGenericArguments("NodeCollection<Table>")]
    public class TableCollection : NodeCollection
    {
        internal TableCollection(CompositeNode parent) : base(parent, NodeType.Table, false)
        {
        }

        /// <summary>
        /// Retrieves a <see cref="Table"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Table this[int index]
        {
            get { return (Table)base[index]; }
        }

        /// <summary>
        /// Copies all tables from the collection to a new array of tables.
        /// </summary>
        /// <returns>An array of tables.</returns>
        public new Table[] ToArray()
        {
            return ToList<Table>().ToArray();
        }
    }
}
