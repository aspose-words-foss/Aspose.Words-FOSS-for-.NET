// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2010 by Denis Darkin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Collections;
using Aspose.Images;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Properties;
using Aspose.Xml;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the information that is used to establish a mapping between the parent
    /// structured document tag and an XML element stored within a custom XML data part in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    public class XmlMapping
    {
        /// <summary>
        /// Ctor. Assigns a link to a parent structured document tag.
        /// </summary>
        internal XmlMapping(StructuredDocumentTag sdt)
        {
            mSdt = sdt;
        }

        /// <summary>
        /// Sets a mapping between the parent structured document tag and an XML node of a custom XML data part.
        /// </summary>
        /// <param name="customXmlPart">A custom XML data part to map to.</param>
        /// <param name="xPath">An XPath expression to find the XML node.</param>
        /// <param name="prefixMapping">XML namespace prefix mappings to evaluate the XPath.</param>
        /// <returns>A flag indicating whether the parent structured document tag is successfully mapped to
        /// the XML node.</returns>
        public bool SetMapping(CustomXmlPart customXmlPart, string xPath, string prefixMapping)
        {
            ArgumentUtil.CheckHasChars(xPath, "xPath");
            ArgumentUtil.CheckNotNull(customXmlPart, "customXmlPart");

            // It looks like that the following SDT types cannot be mapped to XML. Let's raise an exception.
            if ((mSdt.SdtType == SdtType.BuildingBlockGallery) ||
                (mSdt.SdtType == SdtType.DocPartObj) ||
                (mSdt.SdtType == SdtType.Citation))
                throw new InvalidOperationException("Cannot map a structured document tag of this type to XML data.");

            SetMapping("", xPath, prefixMapping);
            mCustomXmlPart = customXmlPart;

            return IsMapped;
        }

        /// <summary>
        /// Deletes mapping of the parent structured document to XML data.
        /// </summary>
        public void Delete()
        {
            mPrefixMappings = "";
            mXPath = "";
            mStoreItemId = "";
            mCustomXmlPart = null;
        }

        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal XmlMapping Clone(StructuredDocumentTag parentSdt)
        {
            // On cloning a document, parentSdt has still source document as parent here, so custom XML part has to
            // be resolved later in the ResolveCustomXmlPart method.
            XmlMapping lhs = (XmlMapping)MemberwiseClone();
            lhs.mSdt = parentSdt;

            // On cloning from one document to another, mCustomXmlPart refers to an XML part of another document:
            // need to reassign reference using mStoreItemId to be resolved to correct XML part in CustomXmlPart
            // property getter on accessing.
            if (mCustomXmlPart != null)
            {
                mStoreItemId = mCustomXmlPart.Id;
                mCustomXmlPart = null;
            }

            return lhs;
        }

        /// <summary>
        /// Finds and reassigns a custom XML part. This method is called on cloning a document to set custom XML part
        /// of a cloned document.
        /// </summary>
        internal void ResolveCustomXmlPart()
        {
            mCustomXmlPart = GetCustomXmlPartById(StoreItemId);
        }

        /// <summary>
        /// Sets a mapping between the parent structured document tag and an XML node of a custom XML data part.
        /// </summary>
        /// <param name="storeItemId">Identifier of a store item: either a custom XML data part or a storage of
        /// document properties.</param>
        /// <param name="xPath">An XPath expression to find the XML node.</param>
        /// <param name="prefixMapping">XML namespace prefix mappings to evaluate the XPath.</param>
        internal void SetMapping(string storeItemId, string xPath, string prefixMapping)
        {
            mPrefixMappings = prefixMapping;
            mXPath = xPath;
            mCustomXmlPart = StringUtil.HasChars(storeItemId) ? GetCustomXmlPartById(storeItemId) : null;
            // Store either custom XML part or its ID.
            mStoreItemId = mCustomXmlPart == null ? storeItemId : "";
        }

        /// <summary>
        /// Returns true if this XML mapping is valid.
        /// </summary>
        /// <remarks>
        /// This method only checks <see cref="XPath"/> since <see cref="StoreItemId"/> and <see cref="PrefixMappings"/>
        /// may not be required by MS Word. Use the <see cref="IsMapped"/> property for more strict check.
        /// </remarks>
        internal bool IsValid
        {
            get { return StringUtil.HasChars(XPath); }
        }

        /// <summary>
        /// Sets XPath expression, which is evaluated to find a custom XML node
        /// that is mapped to the parent structured document tag.
        /// </summary>
        /// <dev>
        /// It is separate from the corresponding property since the property is public.
        /// </dev>
        internal void SetXPath(string value)
        {
            mXPath = value;
        }

        /// <summary>
        /// Gets value of an XML node, to which this object is mapped.
        /// </summary>
        internal string GetValue()
        {
            if (!IsEmpty)
            {
                IList<XmlNode> xmlNodes = GetBoundXmlNodes(null);
                if (xmlNodes.Count > 0)
                    return xmlNodes[0].InnerText;
            }
            return null;
        }

        /// <summary>
        /// Sets value of an XML node, to which this object is mapped.
        /// </summary>
        internal void SetValue(string value)
        {
            List<string> values = new List<string>();
            values.Add(value);

            SetValues(values);
        }

        /// <summary>
        /// Sets values of an XML nodes, to which this object is mapped.
        /// </summary>
        internal void SetValues(IList<string> values)
        {
            if (IsEmpty)
                throw new InvalidOperationException("XML mapping is not defined.");

            Document doc = mSdt.FetchDocument();
            if (IsCoreProperties || IsExtendedProperties)
            {
                // MS Word also allows inserting SDTs that are mapped to cover page properties, but they are stored
                // by standard way: in a special custom XML part.
                string propertyName = GetPropertyName();
                if (!StringUtil.HasChars(propertyName))
                    throw new InvalidOperationException("Cannot find document property.");

                doc.BuiltInDocumentProperties[propertyName].Value = values[0];
            }
            else
            {
                CustomXmlPart xmlPart = doc.CustomXmlParts.GetById(StoreItemId);

                if (xmlPart == null)
                {
                    // WORDSNET-28368 Try to find missing part.
                    xmlPart = TryFindMissingPart(doc.CustomXmlParts, XPath, PrefixMappings, StoreItemId);

                    if (xmlPart == null)
                        throw new InvalidOperationException("Custom XML part is not found.");
                }

                IList<XmlNode> xmlNodes = CustomXmlUtil.ExtractXmlNodes(xmlPart.Data, XPath, PrefixMappings, StoreItemId, null);

                // WORDSNET-28668 Create empty mapped XML node if missing.
                if (xmlNodes.Count == 0)
                    xmlNodes = CustomXmlUtil.CreateMappedXmlNode(XPath, PrefixMappings);

                if (xmlNodes.Count != values.Count)
                    throw new InvalidOperationException("Incorrect XML nodes count.");

                if (CustomXmlUtil.IsPartnerControlsNamespace(xmlNodes))
                {
                    for (int i = 0; i < xmlNodes.Count; i++)
                        xmlNodes[i].InnerText = values[i];
                }
                else
                    xmlNodes[0].InnerText = values[0];

                MemoryStream stream = new MemoryStream();
                XmlUtilPal.SaveXml(xmlNodes[0].OwnerDocument, stream);
                xmlPart.Data = stream.ToArray();
            }
        }

        /// <summary>
        /// Updates structured document tag content from data bound XML data.
        /// </summary>
        internal bool UpdateContent()
        {
            return UpdateContent(new XmlMappingContext());
        }

        /// <summary>
        /// Updates structured document tag content from data bound XML data using cache context.
        /// </summary>
        internal bool UpdateContent(XmlMappingContext ctx)
        {
            using (new SuspendMappedCustomXmlUpdateDocument(mSdt.Document))
            {
                return UpdateDataBoundContent(mSdt, ctx);
            }
        }

        internal int MappedNodesCount
        {
            get
            {
                return GetBoundXmlNodes(null).Count;
            }
        }

        /// <summary>
        /// Updates structured document tag content with bound data.
        /// </summary>
        /// <returns>True if content was updated successfully.</returns>
        private static bool UpdateDataBoundContent(StructuredDocumentTag sdt, XmlMappingContext ctx)
        {
            if(!sdt.CanBeUpdated)
                return true;

            // No data binding - treat update as successful.
            if (sdt.XmlMapping.IsEmpty)
                return true;

            if (!sdt.XmlMapping.IsValid)
                return false;

            // WORDSNET-16888 Break cyclic reference for nested SDT.
            if (sdt.ParentNode.NodeType == NodeType.StructuredDocumentTag)
            {
                StructuredDocumentTag parentSdt = (StructuredDocumentTag)sdt.ParentNode;
                if ((sdt.XmlMapping.XPath == parentSdt.XmlMapping.XPath) &&
                    (sdt.XmlMapping.PrefixMappings == parentSdt.XmlMapping.PrefixMappings))
                    return true;
            }

            if (!sdt.IsUpdateable)
                return false;

            if (sdt.IsRepeatingSection)
                return SdtRepeatingSection.UpdateRepeatingSection(sdt);

            IList<XmlNode> xmlNodes = sdt.XmlMapping.GetBoundXmlNodes(ctx);

            if (xmlNodes.Count > 0)
            {
                string newContent = CustomXmlUtil.GetOverallInnerText(xmlNodes);

                // WORDSNET-28846 Update if IsShowingPlaceholder is set, regardless of whether SDT content equals the mapped value.
                if (AreSameVisibleContent(sdt, newContent) && !sdt.IsShowingPlaceholderText)
                    return false;

                if (StringUtil.HasChars(newContent))
                {
                    switch (sdt.SdtType)
                    {
                        case SdtType.PlainText:
                        {
                            // WORDSNET-20331 Seems that Word does not update PlainText content
                            // if bound XmlNode has child XmlNode.
                            if (HasChildElementNode(xmlNodes[0]))
                                return false;

                            return UpdateTextboxSdt(sdt, newContent, ctx);
                        }
                        case SdtType.RichText:
                        {
                            return UpdateTextboxSdt(sdt, newContent, ctx);
                        }
                        case SdtType.ComboBox:
                        case SdtType.DropDownList:
                        {
                            // See ISO 29500, §17.5.2.5, 15 for information of getting display value of SDT bound to XML
                            SdtDropDownListBase dropDownListProps = (SdtDropDownListBase)sdt.ControlProperties;
                            SdtListItemCollection listItems = dropDownListProps.ListItems;

                            // Searching for the value in list items
                            int itemIndex = listItems.IndexOfItemValue(newContent);
                            if (itemIndex >= 0)
                            {
                                SdtContentHelper.SetDisplayText(sdt, listItems[itemIndex].DisplayText);
                                listItems.SelectedValue = listItems[itemIndex];
                                listItems.LastItemValue = null; // if the last value is different, clear it
                                return true;
                            }

                            // If the XML node value equals to the last value, current display text should be retained.
                            bool retainCurDisplayText = (listItems.LastItemValue == newContent) &&
                                                        (sdt.GetChildNodes(NodeType.Any, false).Count > 0);
                            if (!retainCurDisplayText)
                            {
                                // Setting the value as display text
                                SdtContentHelper.SetDisplayText(sdt, newContent);
                            }

                            return true;
                        }
                        case SdtType.Date:
                        {
                            string sdtDateDisplayValue =
                                SdtContentHelper.GetSdtDateDisplayValue(sdt, newContent, ctx);

                            if (sdt.VisibleText != sdtDateDisplayValue)
                                SdtContentHelper.SetDisplayText(sdt, sdtDateDisplayValue);

                            // WORDSNET-24530 Method should return false only in a case of issues while updating.
                            return true;
                        }
                        case SdtType.Picture:
                            return UpdatePicture(sdt, newContent);

                        case SdtType.Checkbox:
                            return UpdateCheckbox(sdt, newContent);

                        default:
                            break;
                    }
                }
                else
                {
                    // WORDSNET-21979 Update SDT with the default content when mapped value is empty.
                    if(!sdt.IsShowingPlaceholderText)
                        SdtContentHelper.InsertDefaultContent(sdt, true);

                    return true;
                }
            }
            else
            {
                if (sdt.XmlMapping.IsCoreProperties)
                {
                    // Content bound to empty core property should be replaced with placeholder.
                    SdtContentHelper.ReplaceContentWithPlaceholder(sdt);

                    // Lets think that it is successful update.
                    return true;
                }
            }

            // Data binding is failed.
            return false;
        }

        /// <summary>
        /// Checks that visible content of SDT equals to new content and therefore should not be updated.
        /// </summary>
        private static bool AreSameVisibleContent(StructuredDocumentTag sdt, string newContent)
        {
            // WORDSNET-21506 We shall not update sdtContent in case the displayed text for XMLMapping
            // is identical to that for sdtContent.
            //
            // WORDSNET-23777 Do not update not empty plain SDT neither with mapped content nor with placeholder
            // when visible content equal to mapped content even if mapped content is empty.
            //
            // WORDSNET-24529 Check for RichText as well.
            // WORDSNET-28344 Do not update SDT content if mapped content is the same as current SDT content for Dropdown control.
            if ((sdt.SdtType != SdtType.PlainText) && (sdt.SdtType != SdtType.RichText) && sdt.SdtType != SdtType.DropDownList)
                return false;

            if (!StringUtil.HasChars(sdt.GetText()))
                return false;

            if (sdt.SdtType == SdtType.PlainText)
            {
                // WORDSNET-28036 Avoid unwanted update on SDT removal.
                // We collect runs to get visible text but PlainText SDT can be at block level.
                newContent = newContent.TrimEnd(ControlChar.ParagraphBreakChar);
            }
            else if (sdt.SdtType == SdtType.DropDownList)
            {
                // WORDSNET-28443 Check DropDownList content by item value.
                int itemIndex = sdt.ListItems.IndexOfItemValue(newContent);

                if (itemIndex >= 0)
                    newContent = sdt.ListItems[itemIndex].DisplayText;
            }

            return (sdt.VisibleText == newContent);
        }

        /// <summary>
        /// Updates SDT having PlainText or RichText types.
        /// </summary>
        private static bool UpdateTextboxSdt(StructuredDocumentTag sdt, string newContent, XmlMappingContext ctx)
        {
            Debug.Assert((sdt.SdtType == SdtType.PlainText) || (sdt.SdtType == SdtType.RichText));

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(newContent));
            LoadFormat lf;

            if (StringUtil.ContainsOnlyWhitespaces(newContent))
            {
                // WORDSNET-20690 If innerText is empty or whitespace only we will treat it as text.
                lf = LoadFormat.Text;
            }
            else if (!newContent.Contains("<pkg:package"))
            {
                // WORDSNET-20828 Seems that there can be FlatOpc document only, do fast check (code taken from our FileFormatDetector).
                lf = LoadFormat.Text;
            }
            else
                lf = FileFormatUtil.DetectFileFormat(stream).LoadFormat;

            bool updated = true;
            Document mappedDoc = null;
            int mappedParaIstd = int.MinValue;

            // WORDSNET-28046 Save first SDT paragraph to restore its properties in case of update from placeholder.
            Paragraph savedSdtPara = null;
            if (sdt.HasChildNodes && (sdt.FirstChild.NodeType == NodeType.Paragraph))
                savedSdtPara = (Paragraph)((Paragraph)sdt.FirstChild).Clone(false);

            // See TestJira7286 for case with "Unknown" load format and plain text SDT.
            if ((lf == LoadFormat.Text) || ((lf == LoadFormat.Unknown) && (sdt.SdtType == SdtType.PlainText)))
            {
                // WORDSNET-15134 Mimic MSW and remove CrLf for plain text SDT.
                // WORDSNET-26419 Word ignores \r as well.
                newContent = (sdt.SdtType == SdtType.RichText) || sdt.Multiline
                    ? newContent
                    : newContent.Replace(ControlChar.CrLf, "").Replace(ControlChar.ParagraphBreak, "");

                // WORDSNET-9830 MSW ignores these symbols.
                newContent = newContent.Replace("_x000d_", "").Replace("_x000D_", "");

                SdtContentHelper.SetDisplayText(sdt, newContent);
            }
            else if (lf != LoadFormat.Unknown)
            {
                // Skip updating mapped SDT which is not on the top level. Check Test22617UpdateNestedSdt for details.
                if ((sdt.ParentNode.NodeType == NodeType.StructuredDocumentTag) &&
                    !((StructuredDocumentTag)sdt.ParentNode).XmlMapping.IsEmpty)
                {
                    return true;
                }

                // WORDSNET-17759 Recognize the content of FlatOpc document, which mapped to the "PlainText" SDT.
                mappedDoc = new Document(stream, null, false);

                // AM. Seems that MS Word applies paragraph style of first paragraph to placeholder.
                //
                // Mapped document might be modified during preparation and first paragraph can be removed at
                // placeholder applying stage. Maybe we need some refactoring here but lets save Istd and apply it later.
                mappedParaIstd = mappedDoc.FirstSection.Body.FirstParagraph.ParaPr.Istd;

                updated = InsertDocument(sdt, mappedDoc);

                // Collect inner SDT loaded with mapped document, they need special processing later.
                foreach (StructuredDocumentTag innerTag in sdt.GetChildNodes(NodeType.StructuredDocumentTag, true))
                    ctx.InnerTags.Add(innerTag);
            }
            else
                return false;

            // If the SDT is removed from the document, it has been converted to a structured document range during
            // update. This means that the SDT is not empty (the inserted contents contains sections), no default
            // contents is needed.
            if (!sdt.IsRemoved && !sdt.HasChildNodes)
            {
                SdtContentHelper.InsertDefaultContent(sdt, true);

                // WORDSNET-26601 Apply paragraph style from mapped document to placeholder.
                if ((mappedDoc != null) && (mappedParaIstd != int.MinValue) && (sdt.FirstChild.NodeType == NodeType.Paragraph))
                {
                    Paragraph firstPara = (Paragraph)sdt.FirstChild;

                    // WORDSNET-28046 Restore paragraph  properties.
                    // Actually I think Word does not restore properties and rather updates from placeholder
                    // leaving original paragraph untouched but I dont see easy way to implemented it now.
                    if (savedSdtPara != null)
                    {
                        savedSdtPara.ParaPr.ExpandTo(firstPara.ParaPr);
                        savedSdtPara.ParagraphBreakRunPr.ExpandTo(firstPara.ParagraphBreakRunPr);
                        firstPara.TextId = savedSdtPara.TextId;
                        firstPara.ParaId = savedSdtPara.ParaId;
                    }

                    Style importedStyle = sdt.Document.Styles.ImportStyle(mappedDoc.Styles.GetByIstd(mappedParaIstd, true));
                    firstPara.ParaPr.Istd = importedStyle.Istd;
                }
            }
            else
            {
                // Reset showing placeholder flag upon successful content update.
                sdt.IsShowingPlaceholderText = false;
            }

            return updated;
        }

        /// <summary>
        /// Inserts document contents into the given SDT and, if necessary, moves the SDT from inline to block level.
        /// </summary>
        private static bool InsertDocument(StructuredDocumentTag sdt, Document doc)
        {
            if ((sdt.Level != MarkupLevel.Inline) ||
                (sdt.ParentNode.NodeType != NodeType.Paragraph) ||
                !NeedMoveCurrentSdtToBlockLevel(doc))
            {
                return SdtContentHelper.InsertContent(sdt, doc, true) != null;
            }

            // WORDSNET-23284 Moved removing last paragraph to here from the place before the call to this method
            // to allow SdtContentHelper.InsertDocument() to check properties of last paragraph properly.
            Paragraph lastPara = doc.LastSection.Body.LastParagraph;
            if (SdtContentHelper.CanRemove(lastPara))
                lastPara.Remove();

            // Insert document contents.
            SdtContentHelper.InsertContent(sdt, doc, true);

            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if the current inline SDT should be moved to block level on inserting the specified
        /// document.
        /// </summary>
        private static bool NeedMoveCurrentSdtToBlockLevel(Document insertedDoc)
        {
            if (insertedDoc.Sections.Count > 1)
                return true;

            Body body = insertedDoc.FirstSection.Body;
            Node first = body.FirstNonAnnotationChild;
            Node last = body.LastNonAnnotationChild;

            // WORDSNET-23284 Actual removing of last empty paragraph is not performed at this point yet,
            // so we need this additional check here.
            if (SdtContentHelper.CanRemove(last as Paragraph))
                last = last.PreviousNonAnnotationSibling;

            if (last == null)
                return false;

            // Current SDT has to be moved to block level when:
            // 1. Document body contains more than two non annotation child nodes.
            // 2. Document body has two child nodes and last child is not empty paragraph.
            return (first != last) &&
                   ((first.NextNonAnnotationSibling != last) ||
                    (last.NodeType != NodeType.Paragraph) ||
                    ((Paragraph)last).HasChildNodes);
        }

        /// <summary>
        /// TODO this code never used.
        /// Updates first Shape or DrawingML of SDT with given image.
        /// </summary>
        /// <param name="sdt"></param>
        /// <param name="imageBytes">Base64 encoded image bytes.</param>
        /// <returns>True if update was successful, false otherwise.</returns>
        private static bool UpdatePicture(StructuredDocumentTag sdt, string imageBytes)
        {
            // WORDSNET-10935 Ensure the string is Base64 encoded and decoded image bytes represents a valid image.
            // WORDSNET-18887 If Base64 is corrupted, return an empty byte array.
            byte[] imageData = StringUtil.ConvertFromBase64Safe(imageBytes);

            if (ImageUtil.GetImageType(imageData) == FileFormat.Unknown)
                return false;

            MemoryStream stream = new MemoryStream(imageData);

            Shape contentShape = (Shape)sdt.GetChild(NodeType.Shape, 0, true);

            if(contentShape == null)
                return false;

            contentShape.ImageData.SetImage(stream);
            // WORDSNET-16472 Update size of the SDT with given image according to original size ratio and actual aspect ratio.
            UpdatePictureSize(contentShape);

            return true;
        }

        /// <summary>
        /// Updates size of the specified picture.
        /// </summary>
        private static void UpdatePictureSize(Shape contentShape)
        {
            double width = contentShape.Width;
            double height = contentShape.Height;

            ImageSizeCore origImgSize = ImageUtil.GetImageSize(contentShape.ImageData.ImageBytes);
            contentShape.SetWidthSafe(origImgSize.WidthPoints);
            contentShape.SetHeightSafe(origImgSize.HeightPoints);

            bool hasAspectRatio = contentShape.AspectRatioLocked;
            contentShape.AspectRatioLocked = true;

            if (!MathUtil.AreEqual(width, height))
            {
                if (width > height)
                    contentShape.Height = height;
                else
                    contentShape.Width = width;
            }
            else
            {
                if (origImgSize.WidthEmus < origImgSize.HeightEmus)
                    contentShape.Height = height;
                else
                    contentShape.Width = width;
            }

            contentShape.AspectRatioLocked = hasAspectRatio;
        }

        /// <summary>
        /// Updates state of data bound check box.
        /// </summary>
        private static bool UpdateCheckbox(StructuredDocumentTag sdt, string value)
        {
            bool newChecked = (value == "1") || (value == "true");

            // WORDSNET-24036 Word updates checkbox content only when value is changed.
            if(sdt.Checked != newChecked)
                sdt.Checked = newChecked;

            return true;
        }

        /// <summary>
        /// Selects data bound XmlNodes from customXmlParts.
        /// </summary>
        internal IList<XmlNode> GetBoundXmlNodes(XmlMappingContext ctx)
        {
            Document doc = mSdt.FetchDocument();

            IList<XmlNode> xmlNodes = new List<XmlNode>();

            if (IsCoreProperties)
            {
                byte[] corePropsXml = CreateCorePropertiesXml(doc.BuiltInDocumentProperties);

                // WORDSNET-25289 Use default prefix mapping for core properties.
                string prefixMapping = StringUtil.HasChars(PrefixMappings)
                    ? PrefixMappings
                    : DefaultCorePropsPrefixMapping;

                xmlNodes = CustomXmlUtil.ExtractXmlNodes(corePropsXml, XPath, prefixMapping, StoreItemId, ctx);
            }
            else if (IsExtendedProperties)
            {
                xmlNodes = CustomXmlUtil.ExtractXmlNodes(CreateExtendedPropertiesXml(doc.BuiltInDocumentProperties),
                    XPath, PrefixMappings, StoreItemId, ctx);
            }
            else
            {
                CustomXmlPart part = doc.CustomXmlParts.GetById(StoreItemId);

                if (part == null)
                {
                    // StoreItemId is specified but corresponding custom XML part is missed.

                    // WARN. Unable to find custom XML part with given ID.

                    // WORDSNET-21591 Seems that Word does not update content of inner SDT in case of missing custom XML part.
                    bool updateMissing = (ctx == null) || !ctx.InnerTags.Contains(mSdt);

                    if (updateMissing)
                    {
                        // AM. Part not found but for TestJira3913.docx Word gets data somehow. I think it tries every part.
                        part = TryFindMissingPart(doc.CustomXmlParts, XPath, PrefixMappings, StoreItemId);
                    }
                }

                if(part != null)
                {
                    // Seems MS Word updates StoreItemId with part identifier if missing but lets postpone.

                    // Part is found, get node.
                    xmlNodes = CustomXmlUtil.ExtractXmlNodes(part.Data, XPath, PrefixMappings, part.Id, ctx);
                }
            }

            return xmlNodes;
        }

        /// <summary>
        /// Gets document property name, to which the parent structured document tag is mapped.
        /// </summary>
        /// <returns></returns>
        internal string GetPropertyName()
        {
            if (!IsCoreProperties && !IsExtendedProperties)
                return null;

            if ((mXPath == null) ||
                (IsCoreProperties && (mXPath.IndexOf("COREPROPERTIES", StringComparison.OrdinalIgnoreCase) == -1)) ||
                (IsExtendedProperties && (mXPath.IndexOf("PROPERTIES", StringComparison.OrdinalIgnoreCase) == -1)))
                return null;

            int beforeIndex = mXPath.LastIndexOf('/');
            if (beforeIndex < 0)
                beforeIndex = mXPath.LastIndexOf('\\');
            if (beforeIndex < 0)
                return null;

            int afterIndex = mXPath.LastIndexOf('[');
            if ((afterIndex < 0) || (afterIndex < beforeIndex))
                afterIndex = mXPath.Length;

            string nodeName = mXPath.Substring(beforeIndex + 1, afterIndex - beforeIndex - 1);

            int prefixIndex = nodeName.IndexOf(':');
            if (prefixIndex >= 0)
                nodeName = nodeName.Substring(prefixIndex + 1);

            if (IsCoreProperties)
            {
                if (gPathToPropertyName.ContainsKey(nodeName))
                    return gPathToPropertyName[nodeName];
                else
                    return null;
            }
            else
            {
                return nodeName;
            }
        }

        /// <summary>
        /// Iterates custom XML part and tries to extract some nodes using given xPath,
        /// </summary>
        /// <returns>
        /// Returns first occured part that contains nodes at given xpath or null if none part contain appropriate nodes.
        /// </returns>
        private static CustomXmlPart TryFindMissingPart(CustomXmlPartCollection customXmlParts,
            string xPath, string prefixMappings, string storeItemId)
        {
            foreach (CustomXmlPart customXmlPart in customXmlParts)
            {
                // Note this operation should not be cached as we actually accessed data incorrectly.
                IList<XmlNode> xmlNodes = CustomXmlUtil.ExtractXmlNodes(
                    customXmlPart.Data, xPath, prefixMappings, storeItemId, null);
                if (xmlNodes.Count > 0)
                    return customXmlPart;
            }

            return null;
        }

        /// <summary>
        /// Finds a custom XML part of a current document by its ID.
        /// </summary>
        private CustomXmlPart GetCustomXmlPartById(string partId)
        {
            Document doc = mSdt.Document as Document;
            return doc != null ? doc.CustomXmlParts.GetById(partId) : null;
        }

        /// <summary>
        /// Checks that given XmlNode has child XmlNode element.
        /// </summary>
        private static bool HasChildElementNode(XmlNode xmlNode)
        {
            XmlNodeList nodeList = xmlNode.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
                if (XmlUtilPal.IsNodeType(nodeList[i], XmlNodeType.Element))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns extended properties stored as XML. Used to update content of structured document tags.
        /// </summary>
        private static byte[] CreateExtendedPropertiesXml(BuiltInDocumentProperties props)
        {
            MemoryStream stream = new MemoryStream();
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(stream, true);

            // This code slightly duplicates DocxExtendedPropertiesWriter.Write,
            // but probably reference to DocxExtendedPropertiesWriter from here is worse.
            xmlBuilder.StartDocument("Properties");

            xmlBuilder.WriteAttributeString("xmlns",
                "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
            xmlBuilder.WriteAttributeString("xmlns:vt",
                "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

            // Only two properties are allowed in MS Word.
            xmlBuilder.WriteOptionalElement("Manager", props.Manager);
            xmlBuilder.WriteElement("Company", props.Company);

            xmlBuilder.EndDocument();

            return stream.ToArray();
        }

        /// <summary>
        /// Returns Builtin properties stored as XML. Used to update structured document tags content.
        /// </summary>
        private static byte[] CreateCorePropertiesXml(BuiltInDocumentProperties props)
        {
            MemoryStream stream = new MemoryStream();
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(stream, true);

            // AM. This code slightly duplicates DocxWriter.WriteCoreProperties
            // but I think links to DocxWriter from here is worse.
            OpcCorePropertiesWriter.Write(
                xmlBuilder,
                props.Title,
                props.Subject,
                props.Author,
                props.Keywords,
                props.Comments,
                props.LastSavedBy,
                props.RevisionNumber.ToString(),
                props.LastPrinted,
                props.CreatedTime,
                props.LastSavedTime,
                props.Category,
                props.ContentStatus, "", "");

            return stream.ToArray();
        }

        /// <summary>
        /// Returns XML namespace prefix mappings to evaluate the <see cref="XPath"/>.
        /// </summary>
        /// <remarks>
        /// Specifies the set of prefix mappings, which shall be used to interpret the XPath expression
        /// when the XPath expression is evaluated against the custom XML data parts in the document.
        /// </remarks>
        public string PrefixMappings
        {
            get { return mPrefixMappings; }
        }

        /// <summary>
        /// Returns the XPath expression, which is evaluated to find the custom XML node
        /// that is mapped to the parent structured document tag.
        /// </summary>
        public string XPath
        {
            get { return mXPath; }
        }

        /// <summary>
        /// Returns the custom XML data part to which the parent structured document tag is mapped.
        /// </summary>
        public CustomXmlPart CustomXmlPart
        {
            get
            {
                if (IsCoreProperties && (mCustomXmlPart == null))
                {
                    // WORDSNET-27328 Create runtime CustomXmlPart for core properties mapping.
                    Document doc = mSdt.Document as Document;

                    if (doc != null)
                    {
                        mCustomXmlPart = new CustomXmlPart();
                        mCustomXmlPart.Id = CorePropertiesStoreItemId;
                        mCustomXmlPart.Data = CreateCorePropertiesXml(doc.BuiltInDocumentProperties);
                        mCustomXmlPart.Schemas.Add("http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
                        mCustomXmlPart.Schemas.Add("http://purl.org/dc/elements/1.1/");
                    }
                }

                if ((mCustomXmlPart == null) && StringUtil.HasChars(mStoreItemId))
                {
                    mCustomXmlPart = GetCustomXmlPartById(mStoreItemId);
                    // Store either custom XML part or store item ID.
                    if (mCustomXmlPart != null)
                        mStoreItemId = null;
                }
                return mCustomXmlPart;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the parent structured document tag is successfully mapped to XML data.
        /// </summary>
        public bool IsMapped
        {
            get
            {
                if (!IsValid)
                    return false;

                return MappedNodesCount > 0;
            }
        }

        /// <summary>
        /// Specifies the custom XML data identifier for the custom XML data part which
        /// shall be used to evaluate the <see cref="XPath"/> expression.
        /// </summary>
        public string StoreItemId
        {
            get
            {
                if (mCustomXmlPart != null)
                    return mCustomXmlPart.Id;
                return StringUtil.HasChars(mStoreItemId) ? mStoreItemId : "";
            }
            internal set
            {
                mCustomXmlPart = GetCustomXmlPartById(value);
                // Store either custom XML part or store item ID.
                mStoreItemId = (mCustomXmlPart == null) ? value : "";
            }
        }

        /// <summary>
        /// Returns true, if this XML Mapping can be achieved only with using extensions defined
        /// in the version 15 of MS Word.
        /// </summary>
        internal bool IsDocx15Extension
        {
            get
            {
                return !IsEmpty &&
                    // It is not completely clear if BuildingBlockGallery, DocPartObj and Citation can be mapped to XML.
                    // MS Word raises an error if they are mapped with the ISO dataBinding element, but does not when
                    // w15:dataBinding is used. Let's include these SDT types into the check below for safety. Mapping
                    // of such SDTs may be only read from file since AW public interface does not allow to do that.
                    ((mSdt.SdtType == SdtType.RichText) || (mSdt.SdtType == SdtType.BuildingBlockGallery) ||
                        (mSdt.SdtType == SdtType.DocPartObj) || (mSdt.SdtType == SdtType.Citation) ||
                        (mSdt.SdtType == SdtType.Checkbox) || mSdt.IsRepeatingSection || mSdt.IsRepeatingSectionItem);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this XML mapping is not defined.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return !StringUtil.HasChars(mXPath) && !StringUtil.HasChars(mPrefixMappings) &&
                    !StringUtil.HasChars(mStoreItemId) && (mCustomXmlPart == null);
            }
        }

        /// <summary>
        /// Specifies the w16sdtdh:storeItemChecksum attribute.
        /// </summary>
        /// <remarks>
        /// Added for compatibility with Word beta version 2102.
        /// Behavior and description can be changed in the future.
        /// </remarks>
        internal string StoreItemChecksum { get; set; }

        /// <summary>
        /// Indicates that stored checksum differs from actual.
        /// </summary>
        internal bool ChecksumNotEqual(XmlMappingContext context)
        {
            if (IsEmpty)
                return false;

            if (!StringUtil.HasChars(StoreItemChecksum))
                return true;

            // WORDSNET-22189 Do not update SDT content when XML part checksum equal to saved in the SDT.
            string actualChecksum = context.GetStoreItemChecksum(this);

            return actualChecksum != StoreItemChecksum;
        }

        /// <summary>
        /// Indicates that this object defines mapping to document core properties.
        /// </summary>
        internal bool IsCoreProperties
        {
            get { return StoreItemId == CorePropertiesStoreItemId; }
        }

        /// <summary>
        /// Indicates that this object defines mapping to document extended properties.
        /// </summary>
        private bool IsExtendedProperties
        {
            get { return StoreItemId == ExtendedPropertiesStoreItemId; }
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static XmlMapping()
        {
            gPathToPropertyName = new StringToStringDictionary(false);
            gPathToPropertyName.Add("title", PropertyName.Title);
            gPathToPropertyName.Add("subject", PropertyName.Subject);
            gPathToPropertyName.Add("creator", PropertyName.Author);
            gPathToPropertyName.Add("keywords", PropertyName.Keywords);
            gPathToPropertyName.Add("description", PropertyName.Comments);
            gPathToPropertyName.Add("category", PropertyName.Category);
            gPathToPropertyName.Add("contentStatus", PropertyName.ContentStatus);
        }

        private string mXPath = "";
        private string mPrefixMappings = "";
        private string mStoreItemId;
        private CustomXmlPart mCustomXmlPart;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private StructuredDocumentTag mSdt;

        private const string CorePropertiesStoreItemId = "{6C3C8BC8-F283-45AE-878A-BAB7291924A1}";
        private const string ExtendedPropertiesStoreItemId = "{6668398D-A668-4E3E-A5EB-62B293D839F1}";

        private const string DefaultCorePropsPrefixMapping =
            "xmlns:ns0='http://purl.org/dc/elements/1.1/' xmlns:ns1='http://schemas.openxmlformats.org/package/2006/metadata/core-properties'";

        private static readonly StringToStringDictionary gPathToPropertyName;
    }
}
