// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2013 by Andrey Noskov

using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides static method for parsing 'w:permStart' and 'w:permEnd' nodes.
    /// These nodes can be met in all elements that are story containers, and also paragraph, 
    /// table and row elements.
    /// </summary>
    internal static class NrxEditableRangesReader
    {
        /// <summary>
        /// Reads 'w:permStart' node.
        /// </summary>
        internal static void ReadEditableRangeStart(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            EditableRangeStart editableRangeStart = new EditableRangeStart(
                reader.Document, annotation.Id, annotation.SingleUser, annotation.EditorGroup);
            editableRangeStart.DisplacedBy = annotation.DisplacedBy;

            if (annotation.ColFirst >= 0 && annotation.ColLast >= 0)
            {
                editableRangeStart.FirstColumn = annotation.ColFirst;
                editableRangeStart.LastColumn = annotation.ColLast;
            }

            reader.AddCrossStructureAnnotation(editableRangeStart);
        }

        /// <summary>
        /// Reads 'w:permEnd' node.
        /// </summary>
        internal static void ReadEditableRangeEnd(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            EditableRangeEnd editableRangeEnd = new EditableRangeEnd(reader.Document, annotation.Id);
            editableRangeEnd.DisplacedBy = annotation.DisplacedBy;

            reader.AddCrossStructureAnnotation(editableRangeEnd);
        }
    }
}
