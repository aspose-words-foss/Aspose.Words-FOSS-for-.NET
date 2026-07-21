// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2011 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.Settings;
using Aspose.Words.Validation;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class helps to read fldChar from WML or DOCX document.
    /// It is created as a result of refactoring WmlFldCharReader and DocxFldCharReader.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxFldCharReaderBase
    {
        /// <summary>
        /// The problem with reading fields from WordML and DOCX is that we don't know the type of the field
        /// when we encounter the field char nodes. The actual type of the field must be derived from
        /// text stored in runs that occur between field start and field end or separator nodes.
        /// This text forms a field code. The first word in this code represents the type of the field.
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            bool isFieldEnd = false;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "fldCharType":
                    {
                        switch (xmlReader.Value)
                        {
                            case "begin":
                                ReadFieldStart(reader, runPr);
                                break;
                            case "separate":
                                ReadFieldSeparator(reader, runPr);
                                break;
                            case "end":
                                isFieldEnd = true;
                                break;
                            default:
                                // Do nothing.
                                break;
                        }
                        break;
                    }
                    case "dirty":
                    {
                        if (reader.XmlReader.FieldNodesStack.Count > 0)
                        {
                            FieldChar fieldChar = xmlReader.FieldNodesStack.Peek();
                            fieldChar.IsDirty = xmlReader.ValueAsBool;
                        }
                        break;
                    }
                    case "fldLock":
                    {
                        if (reader.XmlReader.FieldNodesStack.Count > 0)
                        {
                            FieldChar fieldChar = xmlReader.FieldNodesStack.Peek();
                            fieldChar.IsLocked = xmlReader.ValueAsBool;
                        }
                        break;
                    }
                    default:
                        // Do nothing.
                        break;
                }
            }

            // WORDSNET-6774 We should create FieldEnd when all properties FieldEnd are read.
            FieldEnd fieldEnd = isFieldEnd
                ? ReadFieldEnd(reader, runPr)
                : null;

            // Read child elements.
            while (xmlReader.ReadChild("fldChar"))
            {
                switch (xmlReader.LocalName)
                {
                    case "fldData": // WordML.
                        string fieldData = xmlReader.ReadString();

                        if (reader.IsDocx)
                        {
                            // Custom Field Data. It should be stored only.
                            // If current field character is not start, field data may be ignored
                            // (§14.9.5 of ISO 29500, Part 4).
                            FieldChar fieldChar = xmlReader.FieldNodesStack.Peek();
                            if (fieldChar.NodeType == NodeType.FieldStart)
                                ((FieldStart)fieldChar).FieldData = Convert.FromBase64String(fieldData);
                        }
                        else
                        {
                            // If 'fldChar' has 'fldData' defined, then we should memorize this data and
                            // use it later, when type of this field will become known to us.
                            xmlReader.FieldData = fieldData;
                        }
                        break;
                    case "numberingChange":
                    case "annotation":
                        if (fieldEnd == null)
                            break;

                        NrxAnnotation annotation = new NrxAnnotation(xmlReader);
                        fieldEnd.NumberRevision = new FieldNumberRevision(
                            annotation.Author,
                            annotation.Date,
                            annotation.Original);

                        break;
                    default:
                        if (!ReadFormatSpecificAttribute(xmlReader.LocalName, reader, runPr))
                            xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        protected abstract bool ReadFormatSpecificAttribute(string attrName, NrxDocumentReaderBase reader, RunPr runPr);

        /// <summary>
        /// Reads field data and stores it in a field start node.
        /// </summary>
        internal static void ReadFieldData(NrxDocumentReaderBase reader)
        {
            Debug.Assert(reader.CurContainer != null);
            FieldStart fieldStart = (FieldStart)reader.CurContainer.GetChild(NodeType.FieldStart, 0, false);
            if (fieldStart != null)
                fieldStart.FieldData = Convert.FromBase64String(reader.XmlReader.ReadString());
        }

        private static void ReadFieldStart(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FieldStart fieldStart = new FieldStart(reader.Document, runPr, FieldType.FieldNone);
            reader.AddChild(fieldStart);

            reader.XmlReader.FieldNodesStack.Push(fieldStart);
        }

        private static void ReadFieldSeparator(NrxDocumentReaderBase reader, RunPr runPr)
        {
            // RESILIENCY 4503 A DOCPROPERTY field contains two separators. Ignore extra separators.
            if (reader.XmlReader.FieldNodesStack.Count > 0 && reader.XmlReader.FieldNodesStack.Peek() is FieldSeparator)
                return;

            FieldSeparator fieldSeparator = new FieldSeparator(reader.Document, runPr, FieldType.FieldNone);
            reader.AddChild(fieldSeparator);

            reader.XmlReader.FieldNodesStack.Push(fieldSeparator);
        }

        private static FieldEnd ReadFieldEnd(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            FieldEnd fieldEnd = new FieldEnd(reader.Document, runPr, FieldType.FieldNone, false);
            reader.AddChild(fieldEnd);

            // RK Guard against doubled field ends.
            if (xmlReader.FieldNodesStack.Count == 0)
                return null;

            // Build the field bundle. Field type and whether the field has a separator are set below.
            FieldBundle field = new FieldBundle();
            field.End = fieldEnd;

            // We might have a field separator.
            if (xmlReader.FieldNodesStack.Peek() is FieldSeparator)
                field.Separator = (FieldSeparator)xmlReader.FieldNodesStack.Pop();

            // WORDSNET-7690 The document contains corrupted field (without FieldStart).
            // We should ignore such fields as MS Word does.
            if (xmlReader.FieldNodesStack.Count == 0)
            {
                field.RemoveFieldNodes();
                return null;
            }
            else
            {
                field.Start = (FieldStart)xmlReader.FieldNodesStack.Pop();
            }


            // Now we can determine and set field type.
            field.DetermineFieldType();

            // Synthesize field separator.
            if ((field.Separator == null) &&
                (FieldUtil.GetSeparatorPresence(field.FieldType) == FieldSeparatorPresence.Always))
            {
                // At the moment the model assumes that whether the field type has or does not have
                // a separator is determined by the field type. However, in MS Word files, fields
                // that are supposed to have a separator, sometimes do not have it.
                // We synthesize a separator for such fields.
                field.Separator = new FieldSeparator(reader.Document, runPr, field.FieldType);
                reader.InsertChildBefore(reader.CurContainer, field.Separator, field.End);
            }

            // Update the HasSeparator status of the field end. Hopefully I can get rid of this later.
            field.End.SetHasSeparator(field.Separator != null);
            field.UpdateDirtyLocked();

            TransformField(reader, field);

            return fieldEnd;
        }

        private static void TransformField(NrxDocumentReaderBase reader, FieldBundle field)
        {
            switch (field.Start.FieldType)
            {
                case FieldType.FieldFormTextInput:
                case FieldType.FieldFormCheckBox:
                case FieldType.FieldFormDropDown:
                    TransformFormField(reader, field);
                    break;
                case FieldType.FieldIncludePicture:
                case FieldType.FieldImport:
                    if (!reader.LoadOptions.PreserveIncludePictureField)
                        TransformIncludePictureField(reader, field);
                    break;
                case FieldType.FieldEquation:
                    RubyConverter.TransformEquation(field);
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private static void TransformFormField(NrxDocumentReaderBase reader, FieldBundle field)
        {
            bool isDocx = reader.IsDocx;
            NrxXmlReader xmlReader = reader.XmlReader;

            // Get current FormFieldPr (the first one from stack).
            FormFieldPr currentFormFieldPr = xmlReader.CurrentFormFieldPr;

            // If there was no FfData, just leave the field in the document without creating a form field.
            if (((currentFormFieldPr == null) && isDocx) || ((xmlReader.FieldData == null) && !isDocx))
                return;

            // RK We are creating a new node and need a set of run properties.
            // Lets get a clone of run properties from the field start.
            // Interesting that there is no run properties for the form field node itself in DOCX,
            // but there are properties in DOC and in the model. So I'm keeping this as is.
            RunPr runPr = field.Start.RunPr.Clone();
            FormFieldPr formFieldPr = currentFormFieldPr;

            if (!isDocx &&
                (field.FieldType == FieldType.FieldFormCheckBox) &&
                (reader.LoadOptions.MswVersion > MsWordVersion.Word2013))
            {
                formFieldPr.CheckBoxDefault &= xmlReader.FieldData.EndsWith("\r\n", StringComparison.Ordinal);
            }

            FormField formField = new FormField(reader.Document, formFieldPr, runPr);
            Node fieldCodeEnd = field.FieldCodeEnd;
            reader.InsertChildBefore(fieldCodeEnd.ParentNode, formField, fieldCodeEnd);

            if (currentFormFieldPr != null)
                xmlReader.PopFormFieldPr();
        }

        private static void TransformIncludePictureField(NrxDocumentReaderBase reader, FieldBundle field)
        {
            Debug.Assert(field.Start.FieldType == FieldType.FieldIncludePicture || field.Start.FieldType == FieldType.FieldImport);

            // The shape must be right after the field separator, but access it with care.
            Shape shape;
            Node nodeAfterSeparator = field.Separator.NextSibling;

            // WORDSNET-19296 Invalid INCLUDEPICTURE field.
            // This case is similar to FieldEnd occurred.
            NodeType nodeType = (nodeAfterSeparator != null)
                ? nodeAfterSeparator.NodeType
                : NodeType.FieldEnd;

            switch (nodeType)
            {
                case NodeType.Shape:
                {
                    // This is what's expected.
                    shape = (Shape)nodeAfterSeparator;
                    break;
                }
                case NodeType.GroupShape:
                {
                    Debug.Assert(nodeAfterSeparator != null);

                    // WORDSNET-1325 We got group shape followed by shape. Strange, but happens.
                    // Actually, used in the document as an image map. Not sure if this is what MS Word does.
                    shape = (Shape)nodeAfterSeparator.NextSibling;
                    break;
                }
                case NodeType.FieldEnd:
                {
                    // RESILIENCY C:\GoogleProof\Doc\arabic\4030.doc
                    // This document has INCLUDEPICTURE field but no shape. We have to synthesize the shape.
                    shape = new Shape(reader.Document, ShapeType.Image);
                    shape.Width = 1;
                    shape.Height = 1;
                    shape.WrapType = WrapType.Inline;
                    reader.InsertChildAfter(field.Separator.ParentNode, shape, field.Separator);
                    break;
                }
                case NodeType.BookmarkStart:
                {
                    Debug.Assert(nodeAfterSeparator != null);

                    // RESILIENCY WORDSNET-3119 Bookmark start is inside the field result, just before the shape node.
                    shape = (Shape)nodeAfterSeparator.NextSiblingOfType(NodeType.Shape);
                    // We also move the bookmark start out of the field to be before the field.
                    BookmarkStart bmkStart = (BookmarkStart)nodeAfterSeparator;
                    reader.InsertChildBefore(field.Start.ParentNode, bmkStart, field.Start);
                    break;
                }
                default:
                {
                    // DV An exception used to be thrown here. However, I don't think this is correct. INCLUDEPICTURE,
                    // like any other field, may have any result content, including an error message. Just leave such
                    // field as is without transforming.
                    return;
                }
            }

            Debug.Assert(shape != null);

            // RESILIENCY WORDSNET-3119 All images included with an INCLUDEPICTURE field are supposed to be inline,
            // but apparently in one document is not like this, we correct it.
            // WORDSNET-11613 Don't do it for shapes behind text.
            if (!shape.BehindText)
                shape.WrapType = WrapType.Inline;

            // WORDSNET-1310 When an image is included inside an INCLUDEPICTURE field, its blip name type in Escher
            // does not have the link flag set, therefore it is treated by the escher reader as non linked.
            // We have to correct it here.
            if (!shape.ImageData.IsLink)
            {
                FieldCodeIncludePicture fieldCode = FieldCodeIncludePicture.Parse(field);
                shape.ImageData.SourceFullName = fieldCode.SourceFullName;
            }

            RemoveShapeField(field);
        }

        /// <summary>
        /// Removes all nodes of a shape or ole field except the shape itself.
        /// </summary>
        private static void RemoveShapeField(FieldBundle field)
        {
            Node curNode = field.Start;
            while (curNode != field.End.NextSibling)
            {
                Node nextNode = curNode.NextSibling;

                switch (curNode.NodeType)
                {
                    case NodeType.Shape:
                    case NodeType.GroupShape:
                        // Do nothing, since the purpose of this loop is to keep the shape node.
                        break;
                    default:
                        curNode.Remove();
                        break;
                }

                curNode = nextNode;
            }
        }
    }
}
