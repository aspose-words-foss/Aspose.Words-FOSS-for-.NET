// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2007 by Vladimir Averkin

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods for reading "Numbering" package part.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxNumberingReader
    {
        internal DocxNumberingReader(DocxParaPrReader paraPrReader,
            DocxRunPrReader runPrReader,
            INrxVmlReader vmlReader)
        {
            Debug.Assert(paraPrReader != null);
            Debug.Assert(runPrReader != null);
            mParaPrReader = paraPrReader;
            mRunPrReader = runPrReader;
            mVmlReader = vmlReader;
        }

        /// <summary>
        /// Reads "Numbering" package part.
        /// </summary>
        /// <param name="reader">DocxDocumentBaseReader to read from. Should be positioned to the element start.</param>
        internal void Read(DocxDocumentReaderBase reader)
        {
            if (reader.LoadOptions.SkipFormatting)
                return;

            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.Numbering);
            if (xmlReader == null)
                return;

            while (xmlReader.ReadChild("numbering"))
            {
                switch (xmlReader.LocalName)
                {
                    case "numPicBullet":
                        ReadNumPicBullet(reader);
                        break;
                    case "abstractNum":
                        ReadAbstractNum(reader);
                        break;
                    case "num":
                        ReadNum(reader);
                        break;
                    case "numIdMacAtCleanup":
                        reader.Document.Lists.NumIdMacAtCleanup = xmlReader.ReadIntAttribute("val", 0);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads 'w:numPicBullet' element from 'w:numbering' element children.
        /// </summary>
        private void ReadNumPicBullet(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // RK In DOC and RTF picture bullet ids are simple indexes into an array of picture bullets.
            // In WML and DOCX picture bullet ids can probably be anything and in theory can be
            // out of sequence, with gaps and not sorted. To support this requires extra work.
            // I think we are not likely to get out of sequence picture bullet ids, therefore
            // just throw if we get this. We will support this if required by users later.
            int listPicBulletId = xmlReader.ReadIntAttribute("numPicBulletId", 0);

            reader.PictureBulletTranslation.Add(listPicBulletId);

            List<ShapeBase> shapes = ReadShapes(reader);
            foreach (ShapeBase shape in shapes)
                reader.Document.Lists.AddPictureBullet((Shape)shape);
        }

        /// <summary>
        /// Reads picture bullet shapes.
        /// </summary>
        private List<ShapeBase> ReadShapes(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            List<ShapeBase> shapes = new List<ShapeBase>();

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "pict":
                    {
                        if (mVmlReader == null)
                            xmlReader.IgnoreElement();
                        else
                            shapes.AddRange(mVmlReader.Read(reader));
                        break;
                    }
                    case "AlternateContent":
                        {
                            Paragraph tmpContainer = new Paragraph(reader.Document);
                            reader.AddAndPushContainer(tmpContainer);

                            DocxReaderFactory.RunReader.ReadAlternateContent(reader, new RunPr());

                            reader.PopContainer(NodeType.Paragraph);
                            foreach (Node node in tmpContainer.GetChildNodes(NodeType.Any, false))
                            {
                                Shape shape = node as Shape;
                                if (shape != null)
                                {
                                    node.Remove();
                                    shapes.Add(shape);
                                }
                            }
                            tmpContainer.Remove();
                            break;
                        }
                    case "drawing":
                        {
                            ShapeBase shape = ((INrxDmlReader)reader).ReadDrawing(new RunPr());
                            reader.Document.RemoveChild(shape); // shape is inserting into document during reading
                            shapes.Add(shape);
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return shapes;
        }

        /// <summary>
        /// Reads 'w:listDef' element from 'w:lists' children.
        /// </summary>
        private void ReadAbstractNum(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DocumentBase doc = reader.Document;

            int abstractNumId = 0;
            bool lstidSpecified = false;
            bool restartNumberingAfterBreak = false;
            int lsid = 0;
            // Let's default to multiple level for resiliency (in case list type is not specified),
            // but all nine levels are defined.
            ListType listType = ListType.HybridMultiLevel;
            int templateCode = 0;
            string name = null;
            string sti = null;

            ListDef listDef = null;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "abstractNumId":
                    {
                        abstractNumId = xmlReader.ValueAsTruncatedInt;

                        // WORDSNET-13607 Very strange MS Word behavior.
                        // It ignores list definitions with identical 'abstractNumId',
                        // but only when definition with 'abstractNumId = 0' exists.
                        if (ContainsAbstractNumId(reader, abstractNumId))
                            return;

                        break;
                    }
                    // w15:restartNumberingAfterBreak
                    // Element that specifies that list numbering restarts at each section.
                    case "restartNumberingAfterBreak":
                    {
                        reader.ComplianceInfo.MarkAsHasDocxExtensionsOf(MsWordVersionCore.Word2013);
                        restartNumberingAfterBreak = xmlReader.ValueAsBool;
                        break;
                    }
                    default:
                        // Do nothing.
                        break;
                }
            }

            int lvlOrderNumber = 0;
            while (xmlReader.ReadChild("abstractNum"))
            {
                switch (xmlReader.LocalName)
                {
                    case "nsid":
                        lstidSpecified = true;
                        lsid = NrxXmlUtil.TryHexToInt(xmlReader.ReadVal());

                        if (lsid != Int32.MinValue)
                        {
                            // WORDSNET-25569 Handle w:nsid element located at the end of numbering definition.
                            if (listDef != null)
                                listDef.SetListDefId(lsid);
                        }

                        break;
                    case "multiLevelType":
                        listType = DocxNumberingEnum.DocxToListType(xmlReader.ReadVal());
                        break;
                    case "tmpl":
                        templateCode = NrxXmlUtil.Hex8ToInt(xmlReader.ReadVal());
                        break;
                    case "name":
                        name = xmlReader.ReadVal();
                        break;
                    case "styleLink":
                        sti = xmlReader.ReadVal();
                        break;
                    case "numStyleLink":
                        sti = xmlReader.ReadVal();
                        break;
                    case "lvl":
                        if (listDef == null)
                        {
                            listDef = new ListDef(doc, lsid, listType, templateCode);
                            listDef.Name = name;
                        }

                        int ilvl = xmlReader.ReadIntAttribute("ilvl", 0);
                        // WORDSNET-12762 Mimic MS Word behavior: if type of numbering definition is 'singleLevel'
                        // and numbering XML contains several levels, all levels except a level of index 0 are ignored.
                        if ((listType == ListType.SingleLevel) && (ilvl > 0))
                            break;

                        // andrnosk: WORDSNET-7639 Mimic MS Word behavior, if value equals -1 then clear formatting
                        // of the first level numbering and do not read the formatting of the current level.
                        // We should consider refactoring WmlListReader and DocxNumberingReader there are lots of duplications.
                        if (ilvl == -1 && lvlOrderNumber > 0)
                        {
                            listDef.Levels[0].NumberStyle = NumberStyle.None;
                            listDef.Levels[0].NumberFormat = string.Empty;
                            listDef.Levels[0].ParaPr.Clear();
                            reader.XmlReader.IgnoreElement();
                        }
                        else
                        {
                            // If ilvl of the first is invalid, just ignore and read first NumLevel as usual.
                            if ((ilvl == -1) || (ilvl >= listDef.Levels.Count))
                                ilvl = 0;

                            ReadNumLevel(listDef.Levels[ilvl], reader);
                        }

                        lvlOrderNumber++;
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            // If ListDef with this nsid is duplicated in document, then MS Word reuses it.
            ListDef existingListDef = doc.Lists.GetListDefByListDefId(lsid);
            if (existingListDef != null)
            {
                reader.AbstractNumIdToListDef[abstractNumId] = existingListDef;
            }
            else
            {
                if (listDef == null)
                {
                    listDef = new ListDef(doc, lsid, listType, templateCode);
                    listDef.Name = name;
                }

                // WORDSNET-20828 nsid can be omitted in abstractNum definition.
                // In this case we set default lsid, this caused incorrect numbering in the resulting document.
                // To fix the problem added flag that indicates whether nsid is specified, if not we use abstractNumId as lsid.
                // WORDSNET-4174 MS Word generates random nsids. We should do the same to avoid merging lists with missed nsids.
                // See details in ListCollection.ImportList() comments.
                if (!lstidSpecified)
                {
                    lsid = doc.Lists.MakeUniqueListDefId();
                    listDef.SetListDefId(lsid);
                }

                if (sti != null)
                    listDef.ListStyleIstd = reader.ResolveStyleIdToIstd(sti, StyleIndex.NoList);

                listDef.IsRestartAtEachSection = restartNumberingAfterBreak;

                doc.Lists.AddListDef(listDef);
                reader.AbstractNumIdToListDef[abstractNumId] = listDef;
            }
        }

        /// <summary>
        /// Reads 'w:lvl' element from 'w:listDef' children.
        /// </summary>
        private void ReadNumLevel(ListLevel level, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            bool isTentative = false;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "tplc":
                        // We can extract 'tplc' now without using it as it is not in the model yet.
                        NrxXmlUtil.Hex8ToInt(xmlReader.Value);
                        break;
                    case "tentative":
                        isTentative = xmlReader.ValueAsBool;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
            xmlReader.MoveToElement();

            // TODO 3 w:tplc attribute - not found in DOC
            level.IsTentative = isTentative;
            // We have to set this because default value is 1 in the model, but 0 in the OOXML standard.
            level.StartAt = 0;

            ReadLevelChildren(level, reader);
        }

        private void ReadLevelChildren(ListLevel level, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            string tagName = xmlReader.LocalName;

            mIsLevelStartRead = false;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "start":
                    {
                        level.SetStartAtSafe(xmlReader.ReadIntVal());
                        mIsLevelStartRead = true;
                        break;
                    }
                    case "numFmt":
                        level.NumberStyle = DocxEnum.DocxToNumberStyle(xmlReader.ReadVal());
                        if (level.NumberStyle == NumberStyle.Custom)
                            level.CustomNumberStyle = xmlReader.ReadAttribute("format", "");
                        break;
                    case "lvlRestart":
                        // The value is 1-based in WordML but 0 based in the model.
                        level.SetRestartAfterLevelSafe(xmlReader.ReadIntVal() - 1);
                        break;
                    case "pStyle":
                        level.ParaStyleIstd = reader.ResolveStyleIdToIstd(xmlReader.ReadVal(), StyleIndex.Nil);
                        break;
                    case "isLgl":
                        level.IsLegal = xmlReader.ReadBoolVal();
                        break;
                    case "suff":
                        level.TrailingCharacter = DocxNumberingEnum.DocxToListTrailingCharacter(xmlReader.ReadVal());
                        break;
                    case "lvlText":
                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "val":
                                    level.SetNumberFormatSafe(NrxXmlUtil.XmlToListLevel(xmlReader.Value));
                                    break;
                                case "null":
                                    if (xmlReader.ValueAsBool)
                                        level.SetNumberFormatSafe("");
                                    break;
                                default:
                                    // Do nothing.
                                    break;
                            }
                        }
                        break;
                    case "lvlPicBulletId":
                        level.PictureBulletId = xmlReader.ReadIntVal();

                        // Translate picture bullet ids to zero base.
                        int translatedId = reader.PictureBulletTranslation.IndexOf(level.PictureBulletId);
                        if (translatedId >= 0)
                            level.PictureBulletId = translatedId;
                        break;
                    case "legacy":
                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "legacy":
                                    level.Legacy = xmlReader.ValueAsBool;
                                    break;
                                case "legacySpace":
                                    level.LegacySpace = xmlReader.GetValueAsTwips(complianceInfo);
                                    break;
                                case "legacyIndent":
                                    level.LegacyIndent = xmlReader.GetValueAsTwips(complianceInfo);
                                    break;
                                default:
                                    Debug.Fail(xmlReader.LocalName);
                                    break;
                            }
                        }
                        break;
                    case "lvlJc":
                        level.Alignment =
                            DocxNumberingEnum.DocxToListLevelAlignment(xmlReader.ReadVal(), complianceInfo);
                        break;
                    case "pPr":
                        mParaPrReader.Read(reader, level.ParaPr, level.RunPr);
                        break;
                    case "rPr":
                        mRunPrReader.Read(reader, level.RunPr);
                        break;
                    case "AlternateContent":
                        ReadAlternateContent(level, reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads an alternate content block. Now it can contain only the numFmt element.
        /// </summary>
        private void ReadAlternateContent(ListLevel level, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            bool choiceWasRead = false;

            while (xmlReader.ReadChild("AlternateContent"))
            {
                // Choice and Fallback, are used to provide alternates for specified content.
                switch (xmlReader.LocalName)
                {
                    case "Choice":
                        choiceWasRead = ReadChoice(level, reader);
                        break;
                    case "Fallback":
                    {
                        if (!choiceWasRead)
                            ReadLevelChildren(level, reader);
                        else
                            xmlReader.IgnoreElementNoWarn();
                        break;
                    }
                    default:
                    {
                        xmlReader.IgnoreElement(WarningType.UnexpectedContent, WarningSource.Docx,
                            xmlReader.LocalName);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads elements of the choice part of an alternate content block.
        /// </summary>
        private bool ReadChoice(ListLevel level, DocxDocumentReaderBase reader)
        {
            bool choiceMatched = false;
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "Requires":
                    {
                        // The Requires attribute specifies a set of space-delimited namespaces that must be
                        // understood in order to select that choice.
                        string prefix = xmlReader.Value;
                        choiceMatched = StringUtil.HasChars(prefix) &&
                            NrxRunReaderBase.IsAllowedChoiceRequirement(xmlReader.LookupNamespace(prefix));
                        break;
                    }
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
            xmlReader.MoveToElement();

            if (choiceMatched)
                ReadLevelChildren(level, reader);
            else
                xmlReader.IgnoreElement();

            return choiceMatched;
        }

        /// <summary>
        /// Reads 'w:list' element from 'w:lists' children.
        /// </summary>
        private void ReadNum(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DocumentBase doc = reader.Document;

            int ilfo = xmlReader.ReadIntAttribute("numId", 0);

            // WORDSNET-28555 If a list with the specified ListId already exists, it shall be used.
            List list = doc.Lists.GetListByListId(ilfo);
            if (list == null)
                list = new List(doc, ilfo);

            list.DurableId = xmlReader.ReadIntAttribute("durableId", 0);

            while (xmlReader.ReadChild("num"))
            {
                switch (xmlReader.LocalName)
                {
                    case "abstractNumId":
                    {
                        ListDef listDef = reader.AbstractNumIdToListDef.GetValueOrNull(xmlReader.ReadIntVal());
                        if (listDef != null)
                            list.ListDefId = listDef.ListDefId;
                        break;
                    }
                    case "lvlOverride":
                        {
                            int ilvl = xmlReader.ReadIntAttribute("ilvl", 0);
                            ListLevelOverride levelOverride = new ListLevelOverride(doc, ilvl);
                            levelOverride.IsStartAt = false;
                            levelOverride.IsFormatting = false;
                            while (xmlReader.ReadChild("lvlOverride"))
                            {
                                switch (xmlReader.LocalName)
                                {
                                    case "startOverride":
                                        levelOverride.IsStartAt = true;
                                        levelOverride.StartAtRaw = xmlReader.ReadIntVal();
                                        break;
                                    case "lvl":
                                        levelOverride.IsFormatting = true;
                                        ReadNumLevel(levelOverride.ListLevel, reader);
                                        break;
                                    default:
                                        xmlReader.IgnoreElement();
                                        break;
                                }
                            }

                            // WORDSNET-21002 Try to set missing level start from the 'lvlOverride' tag.
                            // WORDSNET-21125 Do not set value, that is not fit ListLevel.IsStartAtValid.
                            if (!mIsLevelStartRead && levelOverride.IsStartAt && ListLevel.IsStartAtValid(levelOverride.StartAtRaw))
                                levelOverride.ListLevel.StartAt = levelOverride.StartAtRaw;

                            list.Overrides.Add(levelOverride);
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            ValidateOverrides(list);

            doc.Lists.AddList(list);
        }

        /// <summary>
        /// Returns true if <see cref="NrxDocumentReaderBase.AbstractNumIdToListDef"/> contains specified
        /// <paramref name="abstractNumId"/> and 'abstractNumId' equal to 0 already exists or passed now.
        /// </summary>
        private static bool ContainsAbstractNumId(DocxDocumentReaderBase reader, int abstractNumId)
        {
            return (abstractNumId == 0) &&
                   reader.AbstractNumIdToListDef.ContainsKey(abstractNumId);
        }

        /// <summary>
        /// Validates list level overrides.
        /// </summary>
        /// <remarks>
        /// WORDSNET-14514 If list contains at least one <see cref="ListLevelOverride.IsStartAt"/>
        /// set to 'true', then MS Word sets 0 for 'StartOverride' for all level overrides
        /// that were not explicitly read in DOCX.
        /// </remarks>
        private static void ValidateOverrides(List list)
        {
            if (!HasOverriddenStartAt(list))
                return;

            foreach (ListLevelOverride levelOverride in list.Overrides)
            {
                if (!levelOverride.IsStartAt)
                {
                    levelOverride.StartAtRaw = 0;
                    levelOverride.IsStartAt = true;
                }
            }
        }

        /// <summary>
        /// Returns true if list contains at least one <see cref="ListLevelOverride.IsStartAt"/> set to true.
        /// </summary>
        private static bool HasOverriddenStartAt(List list)
        {
            foreach (ListLevelOverride levelOverride in list.Overrides)
            {
                if (levelOverride.IsStartAt)
                    return true;
            }

            return false;
        }

        private readonly DocxParaPrReader mParaPrReader;
        private readonly DocxRunPrReader mRunPrReader;
        private readonly INrxVmlReader mVmlReader;

        private bool mIsLevelStartRead;
    }
}
