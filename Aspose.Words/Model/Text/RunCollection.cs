// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Run"/> nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    [JavaGenericArguments("NodeCollection<Run>")]
    public class RunCollection : NodeCollection
    {
        internal RunCollection(CompositeNode parent) : base(parent, NodeType.Run, false)
        {
        }

        /// <summary>
        /// Retrieves a <see cref="Run"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Run this[int index]
        {
            get { return (Run)base[index]; }
        }

        /// <summary>
        /// Copies all runs from the collection to a new array of runs.
        /// </summary>
        /// <returns>An array of runs.</returns>
        public new Run[] ToArray()
        {
            return ToList<Run>().ToArray();
        }
    }
}
