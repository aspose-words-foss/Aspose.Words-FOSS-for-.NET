// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/11/2015 by Alexey Morozov

using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Shape"/> nodes.
    /// </summary>
    [JavaGenericArguments("NodeCollection<Shape>")]
    internal class ShapeCollection : NodeCollection
    {
        internal ShapeCollection(CompositeNode parent)
            : base(parent, NodeType.Shape, true)
        {
        }

        /// <summary>
        /// Retrieves a <b>Shape</b> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Shape this[int index]
        {
            get { return (Shape)base[index]; }
        }

        /// <summary>
        /// Copies all shapes from the collection to a new array of shapes.
        /// </summary>
        /// <returns>An array of shapes.</returns>
        public new Shape[] ToArray()
        {
            return ToList<Shape>().ToArray();
        }
    }
}
