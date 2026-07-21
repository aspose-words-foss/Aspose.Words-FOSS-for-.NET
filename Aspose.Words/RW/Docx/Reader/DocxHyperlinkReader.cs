// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2007 by Roman Korchagin

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads a hyperlink DOCX element.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxHyperlinkReader : NrxHyperlinkReaderBase
    {
        /// <summary>
        /// Reads a hyperlink DOCX element.
        /// RK Hyperlink in DOCX is very different from hlink in WordML.
        /// </summary>
        internal override void Read(NrxDocumentReaderBase reader)
        {
            // Firstly, read hyperlink attributes.
            string bookmark = "";
            string dest = "";
            string targetFrame = "";
            string tooltip = "";
            string docLocation = "";
            bool history = false;

            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "anchor":
                        // Specifies the name of a bookmark.
                        bookmark = xmlReader.Value;
                        break;
                    case "docLocation":
                        docLocation = xmlReader.Value;
                        break;
                    case "history":
                        history = xmlReader.ValueAsBool;
                        break;
                    case "id":
                        // Specifies the ID of the relationship whose target shall be used as the target.
                        dest = reader.GetRelationshipTarget(xmlReader.Value);
                        break;
                    case "tgtFrame":
                        targetFrame = xmlReader.Value;
                        break;
                    case "tooltip":
                        tooltip = xmlReader.Value;
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }

            // Remember the last insertion point so we can insert the HYPERLINK field here if needed.
            Node lastNodeBeforeHyperlink = reader.CurContainer.LastChild;

            // In DOCX we have a hyperlink element, but in the model we either have a
            // HYPERLINK field or shape properties. To figure out what to do, we need
            // to read the contents of the hyperlink element first.
            ReadChildren(reader);

            Node lastChild = reader.CurContainer.LastChild;
            // WORDSNET-25908 Check that shape is only child.
            if ((lastChild is ShapeBase) && (lastChild.PreviousSibling == lastNodeBeforeHyperlink))
            {
                ShapeBase shape = (ShapeBase) lastChild;
                // The hyperlinked node is a shape, the hyperlink is stored as attributes of the shape.
                shape.SetShapeAttrInternal(ShapeAttr.HyperlinkAddress, UriUtil.AppendSubAddress(dest, bookmark));

                if (StringUtil.HasChars(targetFrame))
                    shape.SetShapeAttrInternal(ShapeAttr.HyperlinkTarget, targetFrame);

                if (StringUtil.HasChars(tooltip))
                    shape.SetShapeAttrInternal(ShapeAttr.ScreenTip, tooltip);
            }
            else
            {
                // The hyperlinked nodes are runs etc. The hyperlink must be turned into a field.
                FieldStart fieldStart = new FieldStart(reader.Document, new RunPr(), FieldType.FieldHyperlink);
                reader.InsertChildAfter(reader.CurContainer, fieldStart, lastNodeBeforeHyperlink);

                FieldCodeHyperlink fieldCode = new FieldCodeHyperlink(
                    dest,
                    bookmark,
                    tooltip,
                    targetFrame,
                    docLocation,
                    !history);
                fieldCode.NormalizeAddressAndSubAddress();
                Run fieldCodeRun = new Run(reader.Document, fieldCode.ToFieldCodeString());

                reader.InsertChildAfter(reader.CurContainer, fieldCodeRun, fieldStart);

                FieldSeparator fieldSeparator = new FieldSeparator(reader.Document, new RunPr(), FieldType.FieldHyperlink);
                reader.InsertChildAfter(reader.CurContainer, fieldSeparator, fieldCodeRun);

                FieldEnd fieldEnd = new FieldEnd(reader.Document, new RunPr(), FieldType.FieldHyperlink, true);
                reader.AddChild(fieldEnd);

                // WORDSNET-16965 Mimic MSW, fieldCode run receives runPr.
                if ((lastChild != null) && (lastChild.NodeType == NodeType.Run))
                {
                    RunPr runPr = ((Run)lastChild).RunPr;

                    // WORDSNET-28099 Only Hidden attribute is copied to field code.
                    runPr.ExpandToInclusive(fieldCodeRun.RunPr, FontAttr.Hidden);

                    runPr.ExpandToInclusive(fieldStart.RunPr, FontAttr.Hidden);
                    runPr.ExpandToInclusive(fieldSeparator.RunPr, FontAttr.Hidden);
                    runPr.ExpandToInclusive(fieldEnd.RunPr, FontAttr.Hidden);
                }

            }
        }
    }
}
