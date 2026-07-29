// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using System;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for MoveToRangeStart and  MoveFromRangeStart nodes of a Word document.
    /// </summary>
    internal abstract class MoveRangeStart : Node, INodeWithAnnotationId, IDisplaceableByCustomXml
    {
        protected MoveRangeStart(
            DocumentBase doc,
            int id,
            string author,
            string name,
            DateTime dateTime,
            DisplacedByType displacedBy)
            : base(doc)
        {
            Id = id;
            Author = author;
            Name = name;
            Date = dateTime;
            DisplacedBy = displacedBy;
        }

        /// <summary>
        /// Specifies the identifier of the move range.
        /// </summary>
        internal int Id { get; set; }

        /// <summary>
        /// Specifies the author for an annotation within a Word document.
        /// </summary>
        internal string Author { get; set; }

        /// <summary>
        /// Specifies the date information for an annotation within a Word document.
        /// </summary>
        internal DateTime Date { get; set; }

        /// <summary>
        /// Specifies the bookmark name.
        /// </summary>
        internal string Name { get; set; }

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

        /// <summary>
        /// Returns original MS Word move range flags that sometimes specify if its a table column move range.
        /// </summary>
        internal int Flags { get; private set; }

        /// <summary>
        /// Returns true if this move range is a table column range.
        /// </summary>
        internal bool IsColumn
        {
            get { return (Flags & 0x8000) == 0x8000; }
        }

        /// <summary>
        /// Gets or sets the index of the first column in move range. Only valid if <see cref="IsColumn"/> is true.
        /// </summary>
        internal int FirstColumn
        {
            get { return Flags & 0x007F; }
            set
            {
                int newValue = Flags;

                newValue &= ~0x007F;
                newValue |= value & 0x007F;

                // Sets the flag indicating this is a table column move range.
                newValue |= 0x8000;
                Flags = newValue;
            }
        }

        /// <summary>
        /// Gets or sets the index of the last column in move range. Only valid if <see cref="IsColumn"/> is true.
        /// </summary>
        internal int LastColumn
        {
            get
            {
                // The flags contains the zero-based index of the first column beyond the end of the table column range
                // associated with the move range.
                return ((Flags & 0x7F00) >> 8) - 1;
            }
            set
            {
                int newValue = Flags;

                newValue &= ~0x7F00;
                // Put value + 1, i.e. index of a column beyond the last column of the column range associated with
                // the move range.
                newValue |= ((value + 1) & 0x007F) << 8;

                // Sets the flag indicating this is a table column move range.
                newValue |= 0x8000;
                Flags = newValue;
            }
        }
    }
}
