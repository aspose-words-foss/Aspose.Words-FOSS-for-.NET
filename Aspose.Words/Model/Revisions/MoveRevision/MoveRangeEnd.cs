// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for MoveToRangeEnd and  MoveFromRangeEnd nodes of a Word document.
    /// </summary>
    internal abstract class MoveRangeEnd : Node, INodeWithAnnotationId, IDisplaceableByCustomXml
    {
        protected MoveRangeEnd(DocumentBase doc, int id, DisplacedByType displacedBy)
            : base(doc)
        {
            Id = id;
            DisplacedBy = displacedBy;
        }

        /// <summary>
        /// Specifies the identifier of the move range.
        /// </summary>
        internal int Id { get; set; }

        /// <summary>
        /// Specifies that the parent annotation's placement shall be directly linked with the location
        /// of the physical presentation of a custom XML element in the document.
        /// </summary>
        internal DisplacedByType DisplacedBy { get; set; }

        /// <summary>
        /// Specifies that the parent annotation's placement shall be directly linked with the location
        /// of the physical presentation of a custom XML element in the document.
        /// </summary>
        /// <dev>
        /// The two same properties DisplacedBy and IDisplaceableByCustomXml.DisplacedByCustomXml have been implemented
        /// to be similar as in bookmark nodes, in which it is done for Java porter. The porter does not support a case
        /// when a class contains "internal" property and explicitly defined interface property with same name.
        /// </dev>
        DisplacedByType IDisplaceableByCustomXml.DisplacedByCustomXml
        {
            get { return DisplacedBy; }
            set { DisplacedBy = value; }
        }

        int INodeWithAnnotationId.IdInternal
        {
            get { return Id; }
            set { Id = value; }
        }

        int INodeWithAnnotationId.ParentIdInternal
        {
            get { return Comment.NoParent; }
            set { }
        }
    }
}
