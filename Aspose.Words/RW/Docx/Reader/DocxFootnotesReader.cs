// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2007 by Vladimir Averkin
using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Loading;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads "Footnotes" and "Endnotes" parts.
    /// </summary>
    internal class DocxFootnotesReader
    {
        /// <summary>
        /// Reads all footnotes and endnotes into hashtables.
        /// </summary>
        internal DocxFootnotesReader(DocxDocumentReaderBase reader)
        {
            mReader = reader;
            Read(FootnoteType.Footnote);
            Read(FootnoteType.Endnote);
        }

        /// <summary>
        /// Gets next not-used footnote or endnote by an id. Returns null if the node is not found. Raises an exception
        /// if all footnotes with the specified ID are already inserted into a document. Allows correctly process
        /// documents that has several footnotes with same ID: next not-used footnote/endnote is returned.
        /// </summary>
        internal Footnote GetFootnoteById(FootnoteType footnoteType, int id)
        {
            List<Footnote> footnoteList;
            switch (footnoteType)
            {
                case FootnoteType.Endnote:
                    footnoteList = mEndnoteByIdTable.GetValueOrNull(id);
                    break;
                case FootnoteType.Footnote:
                    footnoteList = mFootnoteByIdTable.GetValueOrNull(id);
                    break;
                default:
                    throw new InvalidOperationException("Unknown footnote type.");
            }

            if (footnoteList == null)
                return null;


            Footnote lastFootnote = null;
            foreach (Footnote footnote in footnoteList)
            {
                lastFootnote = footnote;

                if (footnote.ParentNode == null)
                    return footnote;
            }

            mReader.Warn(WarningType.DataLoss, WarningSource.Docx,
                string.Format(WarningStrings.FootnoteWithSeveralReferences, id));

            switch (mReader.LoadOptions.RecoveryMode)
            {
                case DocumentRecoveryMode.TryRecover:
                    // Returns footnotes with empty content. This is MS Word behavior.
                    return lastFootnote != null
                        ? (Footnote)lastFootnote.Clone(false)
                        : null;
                default:
                    throw new InvalidOperationException(string.Format(WarningStrings.FootnoteWithSeveralReferences, id));
            }
        }

        /// <summary>
        /// Reads a footnotes or endnotes document part.
        /// </summary>
        private void Read(FootnoteType footnoteType)
        {
            string relType;
            string elementName;
            Dictionary<int, List<Footnote>> dictionary;
            bool isEndnote;

            // Select appropriate values to read either footnotes or endnotes.
            switch (footnoteType)
            {
                case FootnoteType.Footnote:
                {
                    relType = mReader.RelTypes.Footnotes;
                    elementName = "footnote";
                    dictionary = mFootnoteByIdTable;
                    isEndnote = false;
                    break;
                }
                case FootnoteType.Endnote:
                {
                    relType = mReader.RelTypes.Endnotes;
                    elementName = "endnote";
                    dictionary = mEndnoteByIdTable;
                    isEndnote = true;
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown footnote type.");
            }

            NrxXmlReader xmlReader = mReader.SwitchToPartReaderByRelType(relType);
            if (xmlReader == null)
                return;

            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                if (xmlReader.LocalName == elementName)
                {
                    // Read attributes.
                    bool isNormalSeparatorType = true;
                    FootnoteSeparatorType separatorType = FootnoteSeparatorType.FootnoteSeparator;
                    string idStr = null;
                    while (xmlReader.MoveToNextAttribute())
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "type":
                                separatorType = DocxEnum.DocxToFootnoteSeparatorType(xmlReader.Value, isEndnote);
                                isNormalSeparatorType = (xmlReader.Value == "normal");
                                break;
                            case "id":
                                idStr = xmlReader.Value;
                                break;
                            default:
                                xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, string.Format(WarningStrings.UnexpectedTagOrAttribute, xmlReader.LocalName));
                                break;
                        }
                    }

                    // Ignore footnotes without an id.
                    if (!StringUtil.HasChars(idStr))
                        continue;

                    if (isNormalSeparatorType)
                    {
                        // Read the footnote and add to the hashtable for later insertion into the model.
                        Footnote footnote = new Footnote(mReader.Document, footnoteType);
                        DocxReaderFactory.StoryReader.Read(mReader, footnote);

                        // MS Word correctly processes documents that have several footnotes with same ID. Do the same.
                        int id = FormatterPal.XmlToInt(idStr);
                        if (dictionary.ContainsKey(id))
                        {
                            dictionary[id].Add(footnote);
                        }
                        else
                        {
                            List<Footnote> footnoteList = new List<Footnote>();
                            footnoteList.Add(footnote);
                            dictionary[id] = footnoteList;
                        }
                    }
                    else
                    {
                        // Read a footnote separator.
                        FootnoteSeparator separator = new FootnoteSeparator(mReader.Document, separatorType);
                        DocxReaderFactory.StoryReader.Read(mReader, separator);
                        // It is not ideal, but we don't follow the relationship from the source
                        // and add separator to the document straight away,
                        mReader.Document.FootnoteSeparators.Add(separator);
                    }
                }
                else
                {
                    xmlReader.IgnoreElement();
                }
            }
            mReader.RestorePartReader();
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxDocumentReaderBase mReader;
        /// <summary>
        /// All endnotes are read here before the main document is read.
        /// Key is endnote id, value is a Footnote object (with type Endnote).
        /// </summary>
        private readonly Dictionary<int, List<Footnote>> mEndnoteByIdTable = new Dictionary<int, List<Footnote>>();
        /// <summary>
        /// All footnotes are read here before the main document is read.
        /// Key is footnote id, value is a Footnote object.
        /// </summary>
        private readonly Dictionary<int, List<Footnote>> mFootnoteByIdTable =
            new Dictionary<int, List<Footnote>>();
    }
}
