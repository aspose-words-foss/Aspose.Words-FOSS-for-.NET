// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2008 by Roman Korchagin

using System;
using Aspose.Common;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Saving;
using Aspose.Words.Tables;


namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads w:sdt content controls.
    /// </summary>
    internal class DocxSdtReader
    {
        internal DocxSdtReader(DocxInlineReader inlineReader, MarkupLevel level) : this(inlineReader, level, null)
        {
        }

        internal DocxSdtReader(DocxInlineReader inlineReader, MarkupLevel level, TablePr objPr)
        {
            Debug.Assert(inlineReader != null);
            mInlineReader = inlineReader;
            mLevel = level;
            mObjPr = objPr;
        }

        /// <summary>
        /// Reads sdt object of any level.
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader)
        {
            StructuredDocumentTag sdt = new StructuredDocumentTag(reader.Document, mLevel);
            reader.MarkupStart(sdt);

            if (reader.CurrentCustomXmlRevision != null)
                SetRevision(sdt, reader.CurrentCustomXmlRevision);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("sdt"))
            {
                switch (xmlReader.LocalName)
                {
                    case "sdtContent":
                        ReadSdtContent(reader);
                        // WORDSNET-26378 When reading the nested empty SDT (<w:sdtContent/>), PossibleSdtEnd node may be very far from the SDT actual
                        // possible end or even turn out to be the end of the SDT preceding the current one, like in the specified ticket. In this case
                        // reader.CurContainer can point to the SDT actual possible end.
                        if (xmlReader.IsEmptyElement && !NodeUtil.IsMarkupNode(reader.CurContainer) &&
                           ((xmlReader.PossibleSdtEnd == null) || (NodeUtil.AreNodesInSameStory(reader.CurContainer, xmlReader.PossibleSdtEnd) &&
                            (NodeUtil.GetNodeIndex(reader.CurContainer) > NodeUtil.GetNodeIndex(xmlReader.PossibleSdtEnd)))))
                        {
                            xmlReader.PossibleSdtEnd = reader.CurContainer;
                        }
                        break;
                    case "sdtPr":
                        ReadSdtPr(reader, sdt);
                        break;
                    case "sdtEndPr":
                        ReadSdtEndPr(reader, sdt, xmlReader);
                        break;
                    default:
                        // RESILIENCY WORDSNET-21161 The problem occurred because there is a paragraph that is a direct child of 'std' element.
                        // Earlier we ignored everything except 'sdtContent', 'sdtPr' and 'sdtEndPr',
                        // but it seems any block level element can be child of std. So read them to model.
                        if (mLevel == MarkupLevel.Block)
                        {
                            DocxReaderFactory.StoryReader.ReadChild(reader);
                        }
                        // WORDSNET-14295 A 'pict' element is direct child of an inline-level 'sdt' element.
                        else if (mLevel == MarkupLevel.Inline)
                            mInlineReader.ReadChild(reader);
                        break;
                }
            }

            reader.MarkupEnd(sdt, xmlReader.PossibleSdtEnd);
        }

        /// <summary>
        /// Read rPr element inside SdtEndPr (properties of end character of sdt).
        /// </summary>
        private static void ReadSdtEndPr(NrxDocumentReaderBase reader, StructuredDocumentTag sdt, NrxXmlReader xmlReader)
        {
            while (xmlReader.ReadChild("sdtEndPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rPr":
                        DocxReaderFactory.RunPrReader.Read(reader, sdt.EndCharacterRunPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read sdtPr, where almost all the sdt properties reside.
        /// </summary>
        /// <remarks>
        /// The spec does not say explicitly about whether or not one sdt can contain multiple tags or single tag <see cref="SdtType"/>.
        /// Common sense advises, that there should be only one tag per sdt.
        /// So this read routine is designed such a way, that many tags can be read, but only the last tag is associated with sdt.
        /// </remarks>
        private static void ReadSdtPr(NrxDocumentReaderBase reader, StructuredDocumentTag sdt)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            while (xmlReader.ReadChild("sdtPr"))
            {
                // Read Office2010 sdt checkbox.
                if ((xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.W14Markup,
                        complianceInfo.Compliance == OoxmlComplianceCore.IsoStrict)) &&
                    (xmlReader.LocalName == "checkbox"))
                {
                    SdtCheckBox checkbox = new SdtCheckBox();
                    complianceInfo.IsDocxExtensions = true;
                    sdt.ControlProperties = checkbox;
                    ReadCheckbox(reader, checkbox);
                }

                switch (xmlReader.LocalName)
                {
                    case "alias":
                        // WORDSNET-27233 Ignore empty 'alias' tag.
                        string title = xmlReader.ReadVal();
                        if(title != null)
                            sdt.Title = xmlReader.ReadVal();
                        break;
                    case "bibliography":
                        sdt.ControlProperties = new SdtBibliography();
                        break;
                    case "citation":
                        sdt.ControlProperties = new SdtCitation();
                        break;
                    case "comboBox":
                    {
                        SdtDropDownListBase combo = new SdtComboBox();
                        sdt.ControlProperties = combo;
                        combo.ListItems.SetParentSdt(sdt);
                        ReadListBase(reader, combo);
                        break;
                    }
                    case "dataBinding":
                        ReadDataBinding(reader, sdt, xmlReader.NamespaceURI);
                        break;
                    case "date":
                        ReadDate(reader, sdt);
                        break;
                    case "docPartList":
                    {
                        SdtDocPart docPart = new SdtBuildingBlockGallery();
                        sdt.ControlProperties = docPart;
                        ReadDocPart(reader, docPart);
                        break;
                    }
                    case "docPartObj":
                    {
                        SdtDocPart docPart = new SdtDocPartObj();
                        sdt.ControlProperties = docPart;
                        ReadDocPart(reader, docPart);
                        break;
                    }
                    case "dropDownList":
                    {
                        SdtDropDownListBase list = new SdtDropDownList();
                        sdt.ControlProperties = list;
                        list.ListItems.SetParentSdt(sdt);
                        ReadListBase(reader, list);
                        break;
                    }
                    case "equation":
                        sdt.ControlProperties = new SdtEquation();
                        break;
                    case "group":
                        sdt.ControlProperties = new SdtGroup();
                        break;
                    case "id":
                        sdt.SetId(xmlReader.ReadIntVal());
                        break;
                    case "label":
                        // we should check that this label really points to some sdt with ID = label, but we don't do this at the moment.
                        sdt.Label = xmlReader.ReadIntVal();
                        break;
                    case "lock":
                        SetLockProperties(xmlReader, sdt);
                        break;
                    case "picture":
                        sdt.ControlProperties = new SdtPicture();
                        break;
                    case "placeholder":
                        ReadPlaceholder(reader, sdt);
                        break;
                    case "richText":
                        ReadText(reader, sdt, true);
                        break;
                    case "rPr":
                        DocxReaderFactory.RunPrReader.Read(reader, sdt.ContentsRunPr);
                        break;
                    case "showingPlcHdr":
                        sdt.IsShowingPlaceholderText = xmlReader.ReadBoolVal();
                        break;
                    case "tabIndex":
                        sdt.TabIndex = xmlReader.ReadIntVal();
                        break;
                    case "tag":
                        {
                            // Resilience WORDSNET-8694: empty tags are disallowed by spec but should be ignored, not thrown upon.
                            string tag = xmlReader.ReadVal();
                            if (StringUtil.HasChars(tag))
                                sdt.Tag = tag;
                            break;
                        }
                    case "temporary":
                        sdt.IsTemporary = xmlReader.ReadBoolVal();
                        break;
                    case "text":
                        ReadText(reader, sdt, false);
                        break;
                    // Only available in Office2013.
                    // Element that specifies that the parent structured document tag is a container for repeated items.
                    case "repeatingSection": // w15:repeatingSection
                        ReadRepeatingSection(reader, sdt);
                        break;
                    // Only available in Office2013.
                    // Element that specifies that the parent structured document tag is a repeated item.
                    case "repeatingSectionItem": // w15:repeatingSectionItem
                    {
                        complianceInfo.IsDocxExtensions = true;
                        sdt.ControlProperties = new SdtRepeatingSectionItem();
                        break;
                    }
                    // Only available in Office2013.
                    // Element that specifies the color on which to base the visual elements of a structured
                    // document tag.
                    case "color": // w15:color
                        ReadColor(reader, sdt);
                        break;
                    // Only available in Office2013.
                    case "appearance": // w15:appearance
                    {
                        complianceInfo.IsDocxExtensions = true;
                        // WORDSNET-25278 Do not update SDT content while read appearance.
                        sdt.SetAppearanceInternal(DocxEnum.DocxToSdtAppearance(xmlReader.ReadVal()));
                        break;
                    }
                    // Only available in Office2013.
                    case "webExtensionCreated": // w15:webExtensionCreated
                    {
                        complianceInfo.IsDocxExtensions = true;
                        if (xmlReader.ReadBoolVal())
                            sdt.WebExtensionRelationship = SdtWebExtensionRelationship.CreatedByWebExtension;
                        break;
                    }
                    // Only available in Office2013.
                    case "webExtensionLinked": // w15:webExtensionLinked
                    {
                        complianceInfo.IsDocxExtensions = true;
                        if (xmlReader.ReadBoolVal())
                            sdt.WebExtensionRelationship = SdtWebExtensionRelationship.LinkedToWebExtension;
                        break;
                    }
                    // Available in Office 2010 and higher.
                    case "entityPicker": // w14:entityPicker
                    {
                        complianceInfo.IsDocxExtensions = true;
                        sdt.ControlProperties = new SdtEntityPicker();
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            // Resiliency 6902: MS Word 2010 defaults RichText type, so it can be omitted.
            if (sdt.SdtType == SdtType.None)
                sdt.ControlProperties = new SdtText(true);

            // WORDSNET-19609 XPath of RepeatingSection should not contain indexer in last node selector.
            if (sdt.IsRepeatingSection && !sdt.XmlMapping.IsEmpty)
            {
                string xPath = RemoveLastIndexer(sdt.XmlMapping.XPath);
                if (xPath != null)
                {
                    sdt.XmlMapping.SetXPath(xPath);
                    sdt.Document.Warn(WarningType.DataLoss, WarningSource.Docx,
                        WarningStrings.RepeatingSectionXPathLastIndexerRemoved);
                }
            }

            // WORDSNET-24520 Ignore checksum for all SDT types exclude rich textbox.
            XmlMapping mapping = sdt.XmlMapping;
            if ((sdt.SdtType != SdtType.RichText) && !mapping.IsEmpty && StringUtil.HasChars(mapping.StoreItemChecksum))
                mapping.StoreItemChecksum = null;
        }

        /// <summary>
        /// Reads the 'color' Office2013 extension of the 'sdtPr' element.
        /// </summary>
        private static void ReadColor(NrxDocumentReaderBase reader, StructuredDocumentTag sdt)
        {
            reader.ComplianceInfo.IsDocxExtensions = true;
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "val":
                        sdt.BaseColor = NrxXmlUtil.XmlToColor(xmlReader.Value);
                        break;
                    case "themeColor":
                        sdt.ThemeColor = xmlReader.Value;
                        break;
                    case "themeShade":
                        sdt.ThemeShade = xmlReader.Value;
                        break;
                    case "themeTint":
                        sdt.ThemeTint = xmlReader.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Reads Office2013 RepeatingSection properties.
        /// </summary>
        private static void ReadRepeatingSection(NrxDocumentReaderBase reader, StructuredDocumentTag tag)
        {
            reader.ComplianceInfo.IsDocxExtensions = true;
            NrxXmlReader xmlReader = reader.XmlReader;
            SdtRepeatingSection repeatingSection = new SdtRepeatingSection();
            while (xmlReader.ReadChild("repeatingSection"))
            {
                switch (xmlReader.LocalName)
                {
                    case "sectionTitle":
                        repeatingSection.SectionTitle = xmlReader.ReadVal();
                        break;
                    case "doNotAllowInsertDeleteSection":
                        repeatingSection.DoNotAllowInsertDeleteSection = xmlReader.ReadBoolVal();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            tag.ControlProperties = repeatingSection;
        }

        /// <summary>
        /// Reads and converts <see cref="SdtLockType"/> to boolean properties, which are assigned to sdt.
        /// </summary>
        private static void SetLockProperties(NrxXmlReader xmlReader, StructuredDocumentTag sdt)
        {
            SdtLockType lockType = DocxEnum.DocxToSdtLockType(xmlReader.ReadVal());
            sdt.LockContentControl = (lockType == SdtLockType.SdtLocked) || (lockType == SdtLockType.SdtAndContentLocked);
            sdt.LockContents = (lockType == SdtLockType.ContentLocked) || (lockType == SdtLockType.SdtAndContentLocked);
        }

        /// <summary>
        /// Read contents of sdt.
        /// </summary>
        private void ReadSdtContent(NrxDocumentReaderBase reader)
        {
            switch (mLevel)
            {
                case MarkupLevel.Inline:
                    mInlineReader.ReadChildren(reader);
                    break;
                case MarkupLevel.Block:
                    DocxReaderFactory.StoryReader.ReadChildren(reader);
                    break;
                case MarkupLevel.Row:
                    DocxTableReader.ReadChildren(reader, mObjPr);
                    break;
                case MarkupLevel.Cell:
                    DocxRowReader.ReadChildren(reader, mObjPr);
                    break;
                default:
                    throw new InvalidOperationException("Please report exception.");
            }
        }

        /// <summary>
        /// Reads control properties for Office2010 sdt checkbox.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="checkbox"></param>
        private static void ReadCheckbox(NrxDocumentReaderBase reader, SdtCheckBox checkbox)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("checkbox"))
            {
                switch (xmlReader.LocalName)
                {
                    case "checked":
                        checkbox.Checked = xmlReader.ReadIntVal() != 0;
                        break;
                    case "checkedState":
                    case "uncheckedState":
                    {
                        // WORDSNET-24098 Use default attribute values while reading a checkbox properties.
                        string fontName = xmlReader.ReadAttribute("font", "MS Gothic");
                        string charCode = xmlReader.ReadVal();

                        if (xmlReader.LocalName == "checkedState")
                        {
                            // Set read properties as checked state info.
                            checkbox.CheckedStateInfo = new SdtCheckBoxStateInfo(fontName,
                                ParseCharCode(charCode, SdtCheckBoxStateInfo.DefaultCheckedStateInfo.CharacterCode));
                        }
                        else
                        {
                            // Set read properties as unchecked state info.
                            checkbox.UncheckedStateInfo = new SdtCheckBoxStateInfo(fontName,
                                ParseCharCode(charCode, SdtCheckBoxStateInfo.DefaultUncheckedStateInfo.CharacterCode));
                        }
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Parses character code safely, returns default value in case of error.
        /// </summary>
        private static int ParseCharCode(string charCode, int defaultCharCode)
        {
            int result = FormatterPal.TryParseHex(charCode);

            return (result != int.MinValue)
                ? result
                : defaultCharCode;
        }

        /// <summary>
        /// Reads ComboBox and DropDownList attributes and children.
        /// </summary>
        private static void ReadListBase(NrxDocumentReaderBase reader, SdtDropDownListBase listContainer)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // read attributes

            string lastValue = "";
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "lastValue":
                        lastValue = xmlReader.Value;
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }

            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "listItem":
                        ReadListItem(reader, listContainer);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            listContainer.ListItems.LastItemValue = lastValue;
            if (StringUtil.HasChars(lastValue))
                SetLastValue(lastValue, listContainer.ListItems);
        }

        /// <summary>
        /// Once we've read lastValue, now we try to find it among values of list items, if succeeded, then we store it
        /// in the collection, otherwise ignore it.
        /// </summary>
        private static void SetLastValue(string lastValue, SdtListItemCollection listItems)
        {
            int index = listItems.IndexOfItemValue(lastValue);
            if (index >= 0)
                listItems.SetSelectedValueInternal(listItems[index]);
        }

        /// <summary>
        /// Read items from <see cref="SdtComboBox"/> and <see cref="SdtDropDownList"/>
        /// </summary>
        private static void ReadListItem(NrxDocumentReaderBase reader, SdtDropDownListBase listContainer)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string displayText = "";
            string value = "";
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "displayText":
                        displayText = xmlReader.Value;
                        break;
                    case "value":
                        value = xmlReader.Value;
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }

            // WORDSNET-24793 Allow empty value list item during roundtrip.
            SdtListItem listItem = (StringUtil.HasChars(value))
                ? new SdtListItem(displayText, value)
                : new SdtListItem() {DisplayText = displayText};

            listContainer.ListItems.Add(listItem);
        }

        /// <summary>
        /// Read dataBinding info, create dataBinding.
        /// </summary>
        /// <remarks>We do not attempt to bind it to document part, and do not check validity of the binding itself.
        /// The actual custom xml data is read unparsed as <see cref="CustomXmlPartCollection"/>.
        /// This will probably require rework when we decide to support rendering of sdt and public api.</remarks>
        private static void ReadDataBinding(NrxDocumentReaderBase reader, StructuredDocumentTag sdt, string namespaceURI)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            string prefixMappings = "";
            string storeItemId = "";
            string xPath = "";
            string storeItemChecksum = "";
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "prefixMappings":
                        prefixMappings = xmlReader.Value;
                        break;
                    case "storeItemID":
                        storeItemId = xmlReader.Value;
                        break;
                    case "xpath":
                        xPath = xmlReader.Value;
                        break;
                    case "storeItemChecksum":
                        storeItemChecksum = xmlReader.Value;
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }

            // w15:dataBinding conforms to [MS-DOCX]: Word Extensions to the Office Open XML (.docx) File
            if (namespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.W15Markup,
                    complianceInfo.Compliance == OoxmlComplianceCore.IsoStrict))
                complianceInfo.IsDocxExtensions = true;

            if (StringUtil.HasChars(xPath))     // required attribute, if it is empty, then dataBinding is invalid.
                sdt.XmlMapping.SetMapping(storeItemId, xPath, (StringUtil.HasChars(prefixMappings)) ? prefixMappings : "");

            if (StringUtil.HasChars(storeItemChecksum))
                sdt.XmlMapping.StoreItemChecksum = storeItemChecksum;
        }

        /// <summary>
        /// Read date sdt and all its properties.
        /// </summary>
        private static void ReadDate(NrxDocumentReaderBase reader, StructuredDocumentTag sdt)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            SdtDate  date = new SdtDate();
            sdt.ControlProperties = date;

            // read attributes
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "fullDate":
                        date.FullDate = FormatterPal.XmlToDateTime(xmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "calendar":
                        date.CalendarType = DocxEnum.DocxToCaledarType(xmlReader.ReadVal());
                        break;
                    case "dateFormat":
                        date.DateFormat = xmlReader.ReadVal();
                        break;
                    case "lid":
                        string lid = xmlReader.ReadVal();
                        if(StringUtil.HasChars(lid))
                            date.Lid = LocaleConverter.DocxTagToLocale(lid);
                        break;
                    case "storeMappedDataAs":
                        date.StoreMappedDataAs = DocxEnum.DocxToSdtDateStorageFormat(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
            date.NeedToUpdateContent = false;
        }

        /// <summary>
        /// Read <see cref="SdtDocPart"/> and the related props.
        /// </summary>
        private static void ReadDocPart(NrxDocumentReaderBase reader, SdtDocPart docPart)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "docPartCategory":
                        docPart.BuildingBlockCategory = xmlReader.ReadVal();
                        break;
                    case "docPartGallery":
                        docPart.BuildingBlockType = xmlReader.ReadVal();
                        break;
                    case "docPartUnique":
                        docPart.IsUnique = xmlReader.ReadBoolVal();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read placeholder object lookup data in the appropriate docPart.
        /// </summary>
        private static void ReadPlaceholder(NrxDocumentReaderBase reader, StructuredDocumentTag tag)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("placeholder"))
            {
                switch (xmlReader.LocalName)
                {
                    case "docPart":
                        tag.SetPlaceholderNameCore(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read text tag and its attribute.
        /// </summary>
        private static void ReadText(NrxDocumentReaderBase reader, StructuredDocumentTag sdt, bool isRichText)
        {
            SdtText text = new SdtText(isRichText);
            sdt.ControlProperties = text;
            NrxXmlReader xmlReader = reader.XmlReader;

            // read attributes
            while (xmlReader.MoveToNextAttribute())
            {
                if (xmlReader.LocalName == "multiLine")
                    text.IsMultiline = xmlReader.ValueAsBool;
            }
        }

        /// <summary>
        /// Sets specified revision for <paramref name="sdt"/>.
        /// </summary>
        private static void SetRevision(StructuredDocumentTag sdt, RevisionBase revision)
        {
            EditRevision editRevision = revision as EditRevision;
            if (editRevision != null)
            {
                if (editRevision.Type == EditRevisionType.Deletion)
                    ((ITrackableNode)sdt).DeleteRevision = editRevision.Clone();
                else
                    ((ITrackableNode)sdt).InsertRevision = editRevision.Clone();
            }
            else
            {
                MoveRevision moveRevision = revision as MoveRevision;
                if (moveRevision != null)
                {
                    switch (moveRevision.Type)
                    {
                        case MoveRevisionType.MoveFrom:
                            ((ITrackableNode)sdt).MoveFromRevision = moveRevision.Clone();
                            break;
                        case MoveRevisionType.MoveTo:
                            ((ITrackableNode)sdt).MoveToRevision = moveRevision.Clone();
                            break;
                        case MoveRevisionType.None:
                            // Skip.
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes indexer from the last node selector in XPath.
        /// </summary>
        /// <returns>Returns <c>null</c>, if there is no indexer at the last node selector.</returns>
        private static string RemoveLastIndexer(string xPath)
        {
            // Find start of last node selector.
            int start = xPath.LastIndexOf('/');

            // Find indexer opening character.
            start = xPath.IndexOf('[', start + 1);
            if (start == -1)
                return null;

            // Find indexer closing character.
            int end = xPath.IndexOf(']', start + 1);
            if (end == -1)
                return null;

            // Remove indexer.
            return xPath.Remove(start, end - start + 1);
        }

        /// <summary>
        /// The same Pr is used either for storing TablePr or rowPr.
        /// </summary>
        private readonly TablePr mObjPr;

        private readonly DocxInlineReader mInlineReader;
        private readonly MarkupLevel mLevel;
    }
}
