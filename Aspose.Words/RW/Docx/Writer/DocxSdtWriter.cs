// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2010 by Denis Darkin
using System;
using Aspose.Common;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Knows how to write <see cref="StructuredDocumentTag"/> instances.
    /// </summary>
    internal static class DocxSdtWriter
    {
        /// <summary>
        /// Begin write of Sdt.
        /// </summary>
        internal static void WriteStart(StructuredDocumentTag sdt, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            int revisionId = WriteRevisionStart(sdt, writer);

            builder.StartElement("w:sdt");

            WriteSdtPr(sdt, writer);
            WriteSdtEndPr(sdt, writer);

            builder.StartElement("w:sdtContent");

            WriteRevisionEnd(sdt, builder, revisionId);
        }

        /// <summary>
        /// end write of sdt.
        /// </summary>
        internal static void WriteEnd(StructuredDocumentTag sdt, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            int revisionId = WriteRevisionStart(sdt, writer);

            builder.EndElement(); // w:sdtContent
            builder.EndElement();   // w:sdt

            WriteRevisionEnd(sdt, builder, revisionId);
        }

        private static void WriteSdtPr(StructuredDocumentTag sdt, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIso29500Up = (writer.Compliance != OoxmlComplianceCore.Ecma376);

            builder.StartElement("w:sdtPr");

            if (sdt.ContentsRunPr.Count != 0)
                DocxRunPrWriter.Instance.WriteForNodes(sdt.ContentsRunPr, null, false, writer);

            builder.WriteVal("w:alias", sdt.Title);
            builder.WriteVal("w:tag", sdt.Tag);
            builder.WriteVal("w:id", sdt.Id);

            SdtLockType lockType = GetLockTypeFromBoolLockValues(sdt.LockContentControl, sdt.LockContents);
            if (lockType != SdtLockType.Unlocked)
                builder.WriteVal("w:lock", DocxEnum.SdtLockTypeToDocx(lockType));

            WriteOptionalPlaceholder(builder, sdt);

            bool forceShowingPlaceholder = (sdt.SdtType == SdtType.PlainText) && writer.SaveInfo.ForceShowingPlaceholder.Contains(sdt);
            if (sdt.IsShowingPlaceholderText || forceShowingPlaceholder)
                builder.WriteEmptyElement("w:showingPlcHdr");

            WriteOptionalDataBinding(builder, sdt, writer.Compliance);

            WriteSdtControlProperties(builder, sdt, isIso29500Up);

            if (sdt.IsLabelDefined)
                builder.WriteVal("w:label", sdt.Label);

            if (sdt.TabIndex != 0)
                builder.WriteVal("tabIndex", sdt.TabIndex);

            if (sdt.IsTemporary)
                builder.WriteEmptyElement("w:temporary");

            if (isIso29500Up)
            {
                WriteColor(builder, sdt);

                if (sdt.Appearance != SdtAppearance.Default)
                {
                    builder.WriteElementWithAttributes("w15:appearance",
                        "w15:val", DocxEnum.SdtAppearanceToDocx(sdt.Appearance));
                }

                WriteWebExtensionRelationship(sdt, builder);
            }

            builder.EndElement();
        }

        /// <summary>
        /// Writes the 2.5.1.12 webExtensionCreated and 2.5.1.13 webExtensionLinked [MS-DOCX] extensions
        /// of the 'sdtPr' element.
        /// </summary>
        private static void WriteWebExtensionRelationship(StructuredDocumentTag sdt, DocxBuilder builder)
        {
            switch (sdt.WebExtensionRelationship)
            {
                case SdtWebExtensionRelationship.CreatedByWebExtension:
                    builder.WriteEmptyElement("w15:webExtensionCreated");
                    break;
                case SdtWebExtensionRelationship.LinkedToWebExtension:
                    builder.WriteEmptyElement("w15:webExtensionLinked");
                    break;
                case SdtWebExtensionRelationship.None:
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected WebExtensionRelationship value {0}.",
                        sdt.WebExtensionRelationship));
            }
        }

        /// <summary>
        /// Writes the 'color' Office2013 extension of the 'sdtPr' element.
        /// </summary>
        private static void WriteColor(DocxBuilder builder, StructuredDocumentTag sdt)
        {
            if ((sdt.BaseColor != null) || (sdt.ThemeColor != null))
                builder.WriteElementWithAttributes(
                    "w15:color",
                    "w:val", (sdt.BaseColor != null) ? NrxXmlUtil.ColorToXml(sdt.BaseColor) : null,
                    "w:themeColor", sdt.ThemeColor,
                    "w:themeShade", sdt.ThemeShade,
                    "w:themeTint", sdt.ThemeTint);
        }

        private static void WriteOptionalPlaceholder(DocxBuilder builder, StructuredDocumentTag sdt)
        {
            if ((sdt.Placeholder != null) && StringUtil.HasChars(sdt.PlaceholderName))
            {
                builder.StartElement("w:placeholder");
                builder.WriteVal("w:docPart", sdt.PlaceholderName);
                builder.EndElement();
            }
        }

        private static void WriteOptionalDataBinding(DocxBuilder builder, StructuredDocumentTag sdt,
            OoxmlComplianceCore compliance)
        {
            if (!sdt.XmlMapping.IsEmpty)
            {
                if (sdt.XmlMapping.IsDocx15Extension)
                {
                    if (compliance != OoxmlComplianceCore.Ecma376)
                    {
                        builder.StartElement("w15:dataBinding");
                    }
                    else
                    {
                        builder.Warn(WarningType.DataLoss, WarningStrings.Ecma376NotSupportedXmlMapping);
                        return;
                    }
                }
                else
                {
                    builder.StartElement("w:dataBinding");
                }

                builder.WriteAttribute("w:prefixMappings", sdt.XmlMapping.PrefixMappings);
                builder.WriteAttribute("w:xpath", sdt.XmlMapping.XPath);
                builder.WriteAttribute("w:storeItemID", sdt.XmlMapping.StoreItemId);
                builder.WriteAttribute("w16sdtdh:storeItemChecksum", sdt.XmlMapping.StoreItemChecksum);
                builder.EndElement();
            }
        }

        private static void WriteSdtRepeatingSection(SdtRepeatingSection sdtRepeatingSection, DocxBuilder builder)
        {
            builder.StartElement("w15:repeatingSection");

            builder.WriteElementWithAttributes("w15:sectionTitle", "w:val", sdtRepeatingSection.SectionTitle);
            if (sdtRepeatingSection.DoNotAllowInsertDeleteSection)
                builder.WriteElementWithAttributes("w15:doNotAllowInsertDeleteSection", "w:val", 1);

            builder.EndElement();
        }

        private static void WriteSdtControlProperties(DocxBuilder builder, StructuredDocumentTag sdt, bool isIso29500Up)
        {
            SdtControlProperties sdtControlProperties = sdt.ControlProperties;

            switch (sdtControlProperties.Type)
            {
                case SdtType.Bibliography:
                    builder.WriteEmptyElement("w:bibliography");
                    break;
                case SdtType.Citation:
                    builder.WriteEmptyElement("w:citation");
                    break;
                case SdtType.Equation:
                    builder.WriteEmptyElement("w:equation");
                    break;
                case SdtType.DropDownList:
                case SdtType.ComboBox:
                    WriteDropDownListBase((sdtControlProperties.Type == SdtType.DropDownList) ? "w:dropDownList" : "w:comboBox",
                        ((SdtDropDownListBase)sdtControlProperties).ListItems, builder);
                    break;
                case SdtType.Date:
                    WriteDate((SdtDate)sdtControlProperties, builder);
                    break;
                case SdtType.BuildingBlockGallery:
                    WriteDocPart("w:docPartList", (SdtDocPart)sdtControlProperties, builder);
                    break;
                case SdtType.DocPartObj:
                    WriteDocPart("w:docPartObj", (SdtDocPart)sdtControlProperties, builder);
                    break;
                case SdtType.Group:
                    builder.WriteEmptyElement("w:group");
                    break;
                case SdtType.Picture:
                    builder.WriteEmptyElement("w:picture");
                    break;
                case SdtType.RichText:
                case SdtType.PlainText:
                {
                    SdtText text = (SdtText)sdtControlProperties;
                    WriteText(builder, text.Type == SdtType.RichText, text.IsMultiline);
                    break;
                }
                case SdtType.Checkbox:
                    WriteCheckbox((SdtCheckBox)sdtControlProperties, builder);
                    break;
                case SdtType.RepeatingSection:
                {
                    // Do not write DOCX extensions elements if OoxmlCompliance.Ecma376_2006 is specified.
                    if ((sdt.ControlProperties.Type == SdtType.RepeatingSection) && isIso29500Up)
                        WriteSdtRepeatingSection((SdtRepeatingSection)sdt.ControlProperties, builder);

                    break;
                }
                case SdtType.RepeatingSectionItem:
                {
                    // Do not write DOCX extensions elements if OoxmlCompliance.Ecma376_2006 is specified.
                    if (isIso29500Up)
                        builder.WriteEmptyElement("w15:repeatingSectionItem");
                    break;
                }
                case SdtType.EntityPicker:
                {
                    // Do not write DOCX extensions elements if OoxmlCompliance.Ecma376_2006 is specified.
                    if (isIso29500Up)
                        builder.WriteEmptyElement("w14:entityPicker");
                    break;
                }
                case SdtType.None:
                    // Do not write anything.sdt type unspecified.
                    break;
                default:
                    throw new InvalidOperationException("Please report exception.");
            }
        }

        /// <summary>
        /// Writes Office2010 checkbox control properties.
        /// </summary>
        private static void WriteCheckbox(SdtCheckBox checkbox, DocxBuilder builder)
        {
            builder.StartElement("w14:checkbox");

            builder.WriteElementWithAttributes("w14:checked", "w14:val", checkbox.Checked ? 1 : 0);
            builder.WriteElementWithAttributes("w14:checkedState",
                "w14:val", String.Format("{0:x4}", checkbox.CheckedStateInfo.CharacterCode),
                "w14:font", checkbox.CheckedStateInfo.FontName);
            builder.WriteElementWithAttributes("w14:uncheckedState",
                "w14:val", String.Format("{0:x4}", checkbox.UncheckedStateInfo.CharacterCode),
                "w14:font", checkbox.UncheckedStateInfo.FontName);

            builder.EndElement();
        }

        private static void WriteText(DocxBuilder builder, bool isRichtext, bool isMultiline)
        {
            builder.StartElement((isRichtext) ? "w:richText" : "w:text");
            builder.WriteAttributeIfTrue("w:multiLine", isMultiline);
            builder.EndElement();
        }

        private static void WriteDropDownListBase(string tagName, SdtListItemCollection li, DocxBuilder builder)
        {
            builder.StartElement(tagName);
            if (li.SelectedValue != null)
                builder.WriteAttribute("w:lastValue", li.SelectedValue.Value);
            else if (StringUtil.HasChars(li.LastItemValue))
                builder.WriteAttribute("w:lastValue", li.LastItemValue);

            for (int itemIdx = 0; itemIdx < li.Count; itemIdx++)
            {
                builder.StartElement("w:listItem");

                SdtListItem item = li[itemIdx];

                builder.WriteAttributeString("w:value", item.Value);

                // WORDSNET-4693 OOXML Spec says that if displayText is omitted, it is equal to w:val,
                // so we used to skip writing of displayText in case if item.DisplayText == item.Value
                //  But Word does not allow editing of such combo/listboxes. So we now always write displayText.
                builder.WriteAttribute("w:displayText", item.DisplayText);

                builder.EndElement();
            }

            builder.EndElement();
        }

        private static void WriteDate(SdtDate date, DocxBuilder builder)
        {
            builder.StartElement("w:date");
            if (date.FullDate != DateTime.MinValue)
                builder.WriteAttribute("w:fullDate", FormatterPal.DateTimeToXmlUtc(date.FullDate));

            builder.WriteVal("w:dateFormat", date.DateFormat);

            if (date.Lid != SdtDate.EmptyLid)
                builder.WriteVal("w:lid", LocaleConverter.LocaleToDocxTag(date.Lid));

            builder.WriteVal("w:storeMappedDataAs", DocxEnum.SdtDateStorageFormatToDocx(date.StoreMappedDataAs));

            // RK Work seems to always write this, even if default. So do we.
            builder.WriteVal("w:calendar", DocxEnum.CalendarTypeToDocx(date.CalendarType));

            builder.EndElement();
        }

        private static void WriteDocPart(string tagName, SdtDocPart docPart, DocxBuilder builder)
        {
            builder.StartElement(tagName);
            builder.WriteVal("w:docPartCategory", docPart.BuildingBlockCategory);
            builder.WriteVal("w:docPartGallery", docPart.BuildingBlockType);

            if (docPart.IsUnique)
                builder.WriteEmptyElement("w:docPartUnique");

            builder.EndElement();
        }

        private static void WriteSdtEndPr(StructuredDocumentTag sdt, DocxDocumentWriterBase writer)
        {
            if (sdt.EndCharacterRunPr.Count != 0)
            {
                DocxBuilder builder = writer.CurrentBuilder;
                builder.StartElement("w:sdtEndPr");
                DocxRunPrWriter.Instance.WriteForNodes(sdt.EndCharacterRunPr, null, false, writer);
                builder.EndElement();
            }
        }

        /// <summary>
        /// Convert bool values from model to OOXML SdtLockType enum.
        /// </summary>
        private static SdtLockType GetLockTypeFromBoolLockValues(bool isLockContentControl, bool isLockContents)
        {
            SdtLockType result;

            if (isLockContentControl && isLockContents)
            {
                result = SdtLockType.SdtAndContentLocked;
            }
            else if (isLockContentControl)
            {
                result = SdtLockType.SdtLocked;
            }
            else if (isLockContents)
            {
                result = SdtLockType.ContentLocked;
            }
            else
            {
                result = SdtLockType.Unlocked;
            }

            return result;
        }

        /// <summary>
        /// Writes edit or move revision start.
        /// </summary>
        /// <returns>
        /// Id of written revision.
        /// If there are no revisions, then returns -1.
        /// </returns>
        private static int WriteRevisionStart(ITrackableNode sdt, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            EditRevision editRevision = (sdt.InsertRevision != null) ? sdt.InsertRevision : sdt.DeleteRevision;
            if (editRevision != null)
            {
                int id = ((INrxWriterContext)writer).GetNextAnnotationId();
                builder.WriteSdtRevisionStart(editRevision, id);
                return id;
            }

            MoveRevision moveRevision = (sdt.MoveFromRevision != null) ? sdt.MoveFromRevision : sdt.MoveToRevision;
            if ((moveRevision != null) && (moveRevision.Type != MoveRevisionType.None))
            {
                int id = ((INrxWriterContext)writer).GetNextAnnotationId();
                builder.WriteSdtRevisionStart(moveRevision, id);
                return id;
            }

            return -1;
        }

        /// <summary>
        /// Writes edit or move revision end.
        /// </summary>
        private static void WriteRevisionEnd(ITrackableNode sdt, DocxBuilder builder, int id)
        {
            EditRevision editRevision = (sdt.InsertRevision != null) ? sdt.InsertRevision : sdt.DeleteRevision;
            if (editRevision != null)
            {
                builder.WriteSdtRevisionEnd(editRevision, id);
                return;
            }

            MoveRevision moveRevision = (sdt.MoveFromRevision != null) ? sdt.MoveFromRevision : sdt.MoveToRevision;
            if ((moveRevision != null) && (moveRevision.Type != MoveRevisionType.None))
                builder.WriteSdtRevisionEnd(moveRevision, id);
        }
    }
}
