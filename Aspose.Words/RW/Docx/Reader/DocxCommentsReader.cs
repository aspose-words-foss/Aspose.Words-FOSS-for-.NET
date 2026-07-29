// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2007 by Vladimir Averkin

using System;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading comments.
    /// </summary>
    internal static class DocxCommentsReader
    {
        /// <summary>
        /// Reads the "Comments" part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.Comments);
            if (xmlReader == null)
                return;

            while (xmlReader.ReadChild("comments"))
            {
                switch (xmlReader.LocalName)
                {
                    case "comment":
                    {
                        NrxAnnotation annotation = new NrxAnnotation(xmlReader);

                        Comment comment = new Comment(reader.Document, new RunPr());
                        // When reading, we use the annotation id from the document for the model.
                        ((INodeWithAnnotationId)comment).IdInternal = annotation.Id; 
                        comment.Author = annotation.Author;
                        comment.Initial = annotation.Initials;

                        if (annotation.Date != DateTime.MinValue)
                        {
                            // WORDSNET-21481 MS Word (and AW) writes comment local time without conversion to UTC
                            // on saving a document. So, on reading, we need to set Local kind of the date.
                            comment.LocalDateTime = DateTime.SpecifyKind(annotation.Date, DateTimeKind.Local);
                        }

                        DocxReaderFactory.StoryReader.Read(reader, comment);
                        
                        reader.AddComment(comment);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads a w:commentRangeStart element.
        /// </summary>
        internal static void ReadCommentRangeStart(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            // We use the annotation id from the document as a comment id for the model.
            string id = xmlReader.ReadId();
            if (!StringUtil.HasChars(id))
                return;

            CommentRangeStart commentRangeStart = new CommentRangeStart(reader.Document, FormatterPal.XmlToInt(id));
            commentRangeStart.DisplacedBy = ReadDisplacedByCustomXmlAttribute(xmlReader);

            reader.AddCrossStructureAnnotation(commentRangeStart);
        }

        /// <summary>
        /// Reads a w:commentRangeEnd element.
        /// </summary>
        internal static void ReadCommentRangeEnd(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string id = xmlReader.ReadId();
            if (!StringUtil.HasChars(id))
                return;

            CommentRangeEnd commentRangeEnd = new CommentRangeEnd(reader.Document, FormatterPal.XmlToInt(id));
            commentRangeEnd.DisplacedBy = ReadDisplacedByCustomXmlAttribute(xmlReader);

            reader.AddCrossStructureAnnotation(commentRangeEnd);
        }

        /// <summary>
        /// Reads the displacedByCustomXml attribute of comment range element.
        /// </summary>
        private static DisplacedByType ReadDisplacedByCustomXmlAttribute(NrxXmlReader xmlReader)
        {
            string value = xmlReader.ReadAttribute("displacedByCustomXml", null);
            return DocxDopEnum.DocxToDisplacedByType(value);
        }
    }
}
