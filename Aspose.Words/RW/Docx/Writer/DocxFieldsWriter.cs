// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2018 by Alexander Sevidov

using System;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes fields.
    /// </summary>
    internal class DocxFieldsWriter : NrxFieldsWriter
    {
        internal DocxFieldsWriter(DocxDocumentWriterBase writer, DocxRunWriter runWriter)
        {
            mWriter = writer;
            mRunWriter = runWriter;
        }

        /// <summary>
        /// Writes hyperlink field start. Currently some hyperlinks except for inline shapes are written as complex fields,
        /// which is ok for MS Word.
        /// </summary>
        protected override void WriteHyperlinkStart(
            string dest,
            string bookmark,
            string target,
            string screenTip,
            string docLocation,
            bool history)
        {
            Builder.StartElement("w:hyperlink");

            if (StringUtil.HasChars(dest))
                Builder.WriteAttribute("r:id", mWriter.AddHyperlinkRelationship(dest));

            Builder.WriteAttribute("w:anchor", bookmark);
            Builder.WriteAttribute("w:tgtFrame", target);
            Builder.WriteAttribute("w:tooltip", screenTip);
            Builder.WriteAttribute("w:docLocation", docLocation);
            Builder.WriteAttributeIfTrue("w:history", history);
        }

        /// <summary>
        /// Closes the 'w:hyperlink' element. Symmetrical to WriteHyperlinkStart.
        /// </summary>
        internal override void WriteHyperlinkEnd()
        {
            mWriter.Builder.EndElement("w:hyperlink");
        }

        /// <summary>
        /// Returns a clone of the shape with the attributes, which stores hyperlink.
        /// </summary>
        internal Shape GetHyperlinkShape(Shape shape)
        {
            Shape hyperlinkShape = (Shape)shape.Clone(true);

            FieldCodeHyperlink fieldCode = FieldCodeHyperlink.Parse(TopFieldChar.GetField().GetFieldCode());

            // The hyperlinked node is a shape, the hyperlink is stored as attributes of the shape.
            hyperlinkShape.SetShapeAttrInternal(ShapeAttr.HyperlinkAddress, UriUtil.AppendSubAddress(fieldCode.Address, fieldCode.SubAddress));

            if (StringUtil.HasChars(fieldCode.Target))
                hyperlinkShape.SetShapeAttrInternal(ShapeAttr.HyperlinkTarget, fieldCode.Target);

            if (StringUtil.HasChars(fieldCode.ScreenTip))
                hyperlinkShape.SetShapeAttrInternal(ShapeAttr.ScreenTip, fieldCode.ScreenTip);

            return hyperlinkShape;
        }

        protected override void WriteField(FieldChar fieldChar, string fldCharName)
        {
            // MS Word does not seem to write rsids for field chars of some field types, so do we.
            bool isWriteRsid = !FieldUtil.IsDead(fieldChar.FieldType);
            mRunWriter.WriteRunStart(fieldChar.RunPr, fieldChar, isWriteRsid);

            Builder.WriteElementWithAttributesStart(
                "w:fldChar",
                "w:fldCharType", fldCharName);

            if (fieldChar.NodeType == NodeType.FieldStart)
            {
                Builder.WriteAttributeIfTrue("w:dirty", fieldChar.IsDirty);
                Builder.WriteAttributeIfTrue("w:fldLock", fieldChar.IsLocked);
            }

            WriteFfDataIfNeeded(fieldChar);
            WriteFieldDataIfNeeded(fieldChar);
            WriteNumberingChangeIfNeeded(fieldChar);

            Builder.EndElement("w:fldChar");
            mRunWriter.WriteRunEnd(fieldChar.RunPr);
        }

        /// <summary>
        /// Word has strange behavior. When more cases will be collected, this solution will be reworked.
        /// </summary>
        /// <remarks>
        /// AM. It seems I got logic, at least part of it. For unknown reason Word does not write hyperlink field
        /// as hyperlink tag if any bookmark node are adjacent to parent paragraph.
        ///
        /// This is the reason why logic misunderstood before, Word writes _GoBack bookmark ar cursor position
        /// in order to preserve cursor position and that's why it looks like Word does not write hyperlink tag
        /// for the first field in document. To see this behavior move cursor to second line and save document.
        ///
        /// I do not understand this behavior reason, it could be some kind of resilience for certain MS Word version.
        /// </remarks>.
        protected override bool CanWriteAsHyperlink(FieldChar fieldChar)
        {
            bool canWriteAsHyperlink = base.CanWriteAsHyperlink(fieldChar);
            Document doc = fieldChar.Document as Document;

            if ((doc == null) || !canWriteAsHyperlink)
            {
                return canWriteAsHyperlink;
            }

            // WORDSNET-23465 Word does not write hyperlink tag in case bookmark node adjacent to parent paragraph.
            Field field = fieldChar.GetField();
            Paragraph parentPara = field.Start.ParentParagraph;
            Node prevSibling = parentPara.PreviousSibling;

            if ((prevSibling != null) && NodeUtil.IsBookmarkNode(prevSibling))
                return false;

            return true;
        }

        /// <summary>
        /// Writes FfData for a form field start node.
        /// Safe to call for any field char any field type.
        /// </summary>
        private void WriteFfDataIfNeeded(FieldChar fieldChar)
        {
            if (fieldChar.NodeType != NodeType.FieldStart)
                return;

            if (!FieldUtil.IsFormField(fieldChar.FieldType))
                return;

            FormField formField = ((FieldStart)fieldChar).FormField;
            if (formField == null)
                return;

            // RK This code is a quick hack to make 6137 pass. In this document there is no field end and the
            // export-import-export gold will not match because when loading our document, the form field will not be created.
            // A proper fix will be to add field validation logic to document validator then this code can be removed.
            if (formField.Field == null)
                return;

            Builder.StartElement("w:ffData");
            Builder.WriteValEvenIfEmpty("w:name", formField.Name);
            Builder.WriteVal("w:enabled", formField.Enabled);
            Builder.WriteVal("w:calcOnExit", formField.CalculateOnExit);
            Builder.WriteVal("w:entryMacro", formField.EntryMacro);
            Builder.WriteVal("w:exitMacro", formField.ExitMacro);
            WriteFormTextProperty("w:helpText", !formField.OwnHelp, formField.HelpText);
            WriteFormTextProperty("w:statusText", !formField.OwnStatus, formField.StatusText);

            switch (fieldChar.FieldType)
            {
                case FieldType.FieldFormTextInput:
                    WriteTextInput(formField.FormFieldPr);
                    break;
                case FieldType.FieldFormCheckBox:
                    WriteCheckBox(formField.FormFieldPr);
                    break;
                case FieldType.FieldFormDropDown:
                    WriteDropDown(formField.FormFieldPr);
                    break;
                default:
                    throw new InvalidOperationException("Unknown form field type.");
            }

            Builder.EndElement("w:ffData");
        }

        private void WriteTextInput(FormFieldPr formFieldPr)
        {
            Builder.StartElement("w:textInput");
            Builder.WriteVal("w:type", DocxEnum.TextFormFieldTypeToDocx(formFieldPr.TextInputType));
            Builder.WriteVal("w:default", formFieldPr.TextInputDefault);

            // WORDSNET-18021 The value may be in the Int32 range.
            int textInputMaxLength = Convert.ToInt32(formFieldPr.TextInputMaxLength);
            textInputMaxLength = (textInputMaxLength > short.MaxValue) ? short.MaxValue : textInputMaxLength;
            Builder.WriteValIfPositive("w:maxLength", textInputMaxLength);

            Builder.WriteVal("w:format", formFieldPr.TextInputFormat);
            Builder.EndElement("w:textInput");
        }

        private void WriteCheckBox(FormFieldPr formFieldPr)
        {
            Builder.StartElement("w:checkBox");

            if (formFieldPr.CheckBoxSizeAuto)
                Builder.WriteEmptyElement("w:sizeAuto");
            else
                Builder.WriteVal("w:size", FormatterPal.IntToXml(formFieldPr.CheckBoxSizeHalfPoints));

            Builder.WriteBoolValExplicit("w:default", formFieldPr.CheckBoxDefault);

            if (formFieldPr.HasCheckBoxChecked)
                Builder.WriteBoolValExplicit("w:checked", formFieldPr.CheckBoxChecked);

            Builder.EndElement("w:checkBox");
        }

        private void WriteDropDown(FormFieldPr formFieldPr)
        {
            Builder.StartElement("w:ddList");

            if (formFieldPr.DropDownDefault != 0)
                Builder.WriteVal("w:default", formFieldPr.DropDownDefault);

            if (formFieldPr.HasDropDownResult)
                Builder.WriteVal("w:result", formFieldPr.DropDownResult);

            foreach (string listEntry in formFieldPr.DropDownItems)
            {
                // andrnosk: WORDSNET-8030 MS Word crashes when empty listEntry value appears.
                if (!StringUtil.HasChars(listEntry))
                    mWriter.Warn(WarningType.MinorFormattingLoss, "Empty value is not allowed inside DropDown ListEntry, and will be ignored");

                Builder.WriteVal("w:listEntry", listEntry);
            }

            Builder.EndElement("w:ddList");
        }

        private void WriteFormTextProperty(string elementName, bool isAuto, string value)
        {
            if (StringUtil.HasChars(value))
            {
                Builder.StartElement(elementName);
                Builder.WriteAttributeString("w:type", isAuto ? "autoText" : "text");
                Builder.WriteAttributeString("w:val", value);
                Builder.EndElement(elementName);
            }
        }

        private DocxBuilder Builder
        {
            get { return mWriter.CurrentBuilder; }
        }

        /// <summary>
        /// Writes <see cref="FieldStart.FieldData"/> for a field start node.
        /// Safe to call for any field char any field type.
        /// </summary>
        private void WriteFieldDataIfNeeded(FieldChar fieldChar)
        {
            if (fieldChar.NodeType != NodeType.FieldStart)
                return;

            byte[] fieldData = ((FieldStart)fieldChar).FieldData;
            if (fieldData == null)
                return;

            // The fldData element was removed in ISO Strict.
            if (mWriter.Compliance == OoxmlComplianceCore.IsoStrict)
            {
                mWriter.Warn(WarningType.DataLoss, string.Format(WarningStrings.NotSupportedByIsoStrict, "fldData"));
                return;
            }

            Builder.StartElement("w:fldData");
            Builder.WriteAttributeString("xml:space", "preserve");
            Builder.WriteBase64(fieldData, 0, fieldData.Length);
            Builder.EndElement(); //w:fldData
        }

        private void WriteNumberingChangeIfNeeded(FieldChar fieldChar)
        {
            if (fieldChar.NodeType != NodeType.FieldEnd)
                return;

            FieldEnd fieldEnd = (FieldEnd)fieldChar;
            if (fieldEnd.NumberRevision == null)
                return;

            Builder.WriteRevision(fieldEnd.NumberRevision, mWriter.GetNextAnnotationId());
        }

        internal bool IsHyperlinkLastChild(Node lastChild)
        {
            FieldChar fieldChar = TopFieldChar;
            if (fieldChar.FieldType != FieldType.FieldHyperlink)
                return false;

            FieldChar fieldEnd = fieldChar.GetField().End;
            return lastChild.NextSibling == fieldEnd;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxDocumentWriterBase mWriter;
        private readonly DocxRunWriter mRunWriter;
    }
}
